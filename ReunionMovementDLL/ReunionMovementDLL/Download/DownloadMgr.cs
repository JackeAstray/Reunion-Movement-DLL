using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReunionMovementDLL.Download
{
    public class DownloadMgr : IDisposable
    {
        private static readonly HttpClient httpClient = new HttpClient();

        private readonly SemaphoreSlim overallSemaphore;
        private readonly ConcurrentDictionary<string, Task> activeDownloads = new ConcurrentDictionary<string, Task>();

        // per-download controllers
        private readonly ConcurrentDictionary<string, DownloadControl> controls = new ConcurrentDictionary<string, DownloadControl>();

        public event EventHandler<DownloadProgressChangedEventArgs>? DownloadProgressChanged;
        public event EventHandler<BlockProgressChangedEventArgs>? BlockProgressChanged;
        public event EventHandler<DownloadCompletedEventArgs>? DownloadCompleted;

        public int DefaultParts { get; set; } = 4;
        public int MaxPerFileConcurrency { get; set; } = 4;
        public int MaxRetries { get; set; } = 3;
        public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(2);

        public DownloadMgr(int maxConcurrentDownloads = 2)
        {
            overallSemaphore = new SemaphoreSlim(maxConcurrentDownloads, maxConcurrentDownloads);
        }

        // Overload to download into a Stream (e.g., MemoryStream). If destinationStream is null, behave as file-based.
        public Task EnqueueDownloadAsync(string url, string destinationPath, int? parts = null, long? maxBytesPerSecond = null, Stream? destinationStream = null, bool leaveOpen = false, CancellationToken cancellationToken = default)
        {
            var key = destinationPath + "|" + url;
            var control = controls.GetOrAdd(key, _ => new DownloadControl());
            control.MaxBytesPerSecond = maxBytesPerSecond;
            var t = Task.Run(async () =>
            {
                await overallSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    await StartDownloadInternalAsync(url, destinationPath, parts ?? DefaultParts, destinationStream, leaveOpen, control, cancellationToken).ConfigureAwait(false);
                }
                finally
                {
                    overallSemaphore.Release();
                    controls.TryRemove(key, out _);
                }
            }, cancellationToken);

            activeDownloads.TryAdd(key, t);
            t.ContinueWith(task => activeDownloads.TryRemove(key, out _), TaskScheduler.Default);
            return t;
        }

        public Task PauseAsync(string url, string destinationPath)
        {
            var key = destinationPath + "|" + url;
            if (controls.TryGetValue(key, out var c))
            {
                c.Pause();
            }
            return Task.CompletedTask;
        }

        public Task ResumeAsync(string url, string destinationPath)
        {
            var key = destinationPath + "|" + url;
            if (controls.TryGetValue(key, out var c))
            {
                c.Resume();
            }
            return Task.CompletedTask;
        }

        private async Task StartDownloadInternalAsync(string url, string destinationPath, int parts, Stream? destinationStream, bool leaveOpen, DownloadControl control, CancellationToken cancellationToken)
        {
            Exception finalError = null;
            bool success = false;
            try
            {
                // Get headers first
                using var request = new HttpRequestMessage(HttpMethod.Head, url);
                HttpResponseMessage headResponse = null;
                try
                {
                    headResponse = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
                }
                catch
                {
                    // Some servers don't support HEAD; ignore and try GET later
                }

                long? totalLength = null;
                bool supportsRanges = false;
                if (headResponse != null && headResponse.IsSuccessStatusCode)
                {
                    if (headResponse.Content.Headers.ContentLength.HasValue)
                        totalLength = headResponse.Content.Headers.ContentLength.Value;

                    if (headResponse.Headers.AcceptRanges != null && headResponse.Headers.AcceptRanges.Any() && headResponse.Headers.AcceptRanges.Contains("bytes"))
                        supportsRanges = true;
                }

                // If we don't know length or server doesn't support head info, try a GET for headers only
                if (!totalLength.HasValue || !supportsRanges)
                {
                    using var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url), HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();
                    if (response.Content.Headers.ContentLength.HasValue)
                        totalLength = response.Content.Headers.ContentLength.Value;
                    if (response.Headers.AcceptRanges != null && response.Headers.AcceptRanges.Any() && response.Headers.AcceptRanges.Contains("bytes"))
                        supportsRanges = true;
                }

                if (!supportsRanges || parts <= 1 || !totalLength.HasValue)
                {
                    // Fallback to single-stream download with resume support if possible
                    await SingleStreamDownloadAsync(url, destinationPath, destinationStream, leaveOpen, control, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    await MultiPartDownloadAsync(url, destinationPath, parts, totalLength.Value, destinationStream, leaveOpen, control, cancellationToken).ConfigureAwait(false);
                }

                success = true;
            }
            catch (Exception ex)
            {
                finalError = ex;
            }
            finally
            {
                DownloadCompleted?.Invoke(this, new DownloadCompletedEventArgs(url, destinationPath, success, finalError));
            }
        }

        private async Task SingleStreamDownloadAsync(string url, string destinationPath, Stream? destinationStream, bool leaveOpen, DownloadControl control, CancellationToken cancellationToken)
        {
            var tempPath = destinationPath + ".download";
            var manifestPath = destinationPath + ".manifest";

            int attempt = 0;
            long existing = 0;
            long? total = null;

            // load manifest if exists
            DownloadManifest? manifest = null;
            if (File.Exists(manifestPath))
            {
                try { manifest = LoadManifest(manifestPath); } catch { manifest = null; }
            }

            while (true)
            {
                attempt++;
                try
                {
                    await control.WaitIfPausedAsync(cancellationToken).ConfigureAwait(false);

                    using var request = new HttpRequestMessage(HttpMethod.Get, url);
                    existing = 0;
                    if (destinationStream == null)
                    {
                        if (File.Exists(tempPath))
                        {
                            existing = new FileInfo(tempPath).Length;
                            if (existing > 0)
                            {
                                // Request resume if server accepts ranges
                                request.Headers.Range = new RangeHeaderValue(existing, null);
                            }
                        }
                    }

                    using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();

                    total = response.Content.Headers.ContentLength.HasValue ? response.Content.Headers.ContentLength + existing : null;

                    using var contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

                    if (destinationStream == null)
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(destinationPath)) ?? Directory.GetCurrentDirectory());
                    }

                    Stream targetStream;
                    if (destinationStream != null)
                    {
                        targetStream = destinationStream;
                        if (targetStream.CanSeek && total.HasValue)
                        {
                            try { targetStream.SetLength(total.Value); } catch { }
                        }
                    }
                    else
                    {
                        targetStream = new FileStream(tempPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read, 8192, true);
                        targetStream.Seek(existing, SeekOrigin.Begin);
                    }

                    using (destinationStream == null ? targetStream : null)
                    {
                        var buffer = new byte[81920];
                        int read;
                        long totalRead = existing;
                        var sw = System.Diagnostics.Stopwatch.StartNew();
                        long bytesSinceWindow = 0;
                        var lastReportTime = DateTime.UtcNow;
                        while ((read = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) > 0)
                        {
                            await control.WaitIfPausedAsync(cancellationToken).ConfigureAwait(false);

                            // rate limiting
                            if (control.MaxBytesPerSecond.HasValue && control.MaxBytesPerSecond.Value > 0)
                            {
                                bytesSinceWindow += read;
                                var elapsed = sw.Elapsed.TotalSeconds;
                                if (elapsed > 0)
                                {
                                    var currentRate = bytesSinceWindow / elapsed;
                                    if (currentRate > control.MaxBytesPerSecond.Value)
                                    {
                                        var excess = currentRate / control.MaxBytesPerSecond.Value;
                                        var delay = TimeSpan.FromMilliseconds( Math.Min(500, (excess - 1) * 100));
                                        await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                                        // reset counters
                                        sw.Restart();
                                        bytesSinceWindow = 0;
                                    }
                                }
                            }

                            if (destinationStream != null)
                            {
                                lock (destinationStream)
                                {
                                    if (destinationStream.CanSeek) destinationStream.Seek(totalRead, SeekOrigin.Begin);
                                    destinationStream.Write(buffer, 0, read);
                                }
                            }
                            else
                            {
                                await targetStream.WriteAsync(buffer, 0, read, cancellationToken).ConfigureAwait(false);
                            }

                            totalRead += read;

                            // speed calculation
                            var now = DateTime.UtcNow;
                            var elapsedSinceReport = (now - lastReportTime).TotalSeconds;
                            double? bps = null;
                            if (elapsedSinceReport >= 0.5)
                            {
                                bps = read / elapsedSinceReport;
                                lastReportTime = now;
                            }

                            DownloadProgressChanged?.Invoke(this, new DownloadProgressChangedEventArgs(url, destinationPath, totalRead, total, bps));
                        }

                        // finished, move temp to destination if file-based
                        if (destinationStream == null)
                        {
                            if (File.Exists(destinationPath)) File.Delete(destinationPath);
                            targetStream.Dispose();
                            File.Move(tempPath, destinationPath);
                        }
                    }

                    if (destinationStream == null && File.Exists(manifestPath)) File.Delete(manifestPath);
                    if (destinationStream != null && !leaveOpen) destinationStream.Dispose();
                    return;
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception) when (attempt <= MaxRetries)
                {
                    await Task.Delay(RetryDelay * attempt, cancellationToken).ConfigureAwait(false);
                    continue;
                }
            }
        }

        private async Task MultiPartDownloadAsync(string url, string destinationPath, int parts, long totalLength, Stream? destinationStream, bool leaveOpen, DownloadControl control, CancellationToken cancellationToken)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(destinationPath)) ?? Directory.GetCurrentDirectory());

            var partInfos = new List<(int Index, long Start, long End, long Size)>();
            long partSize = totalLength / parts;
            long cursor = 0;
            for (int i = 0; i < parts; i++)
            {
                long start = cursor;
                long end = (i == parts - 1) ? totalLength - 1 : (cursor + partSize - 1);
                long size = end - start + 1;
                partInfos.Add((i, start, end, size));
                cursor += size;
            }

            var manifestPath = destinationPath + ".manifest";
            DownloadManifest manifest = null;
            if (File.Exists(manifestPath))
            {
                try { manifest = LoadManifest(manifestPath); } catch { manifest = null; }
            }

            if (manifest == null)
            {
                manifest = new DownloadManifest
                {
                    Url = url,
                    TotalLength = totalLength,
                    Parts = parts,
                    PartSizes = partInfos.Select(p => p.Size).ToArray(),
                    PartCompleted = new long[parts]
                };
                WriteManifest(manifestPath, manifest);
            }

            // preallocate temp file
            var tempFinal = destinationPath + ".download";
            Stream? finalStream = null;
            if (destinationStream != null)
            {
                finalStream = destinationStream;
                if (finalStream.CanSeek)
                {
                    try { finalStream.SetLength(totalLength); } catch { }
                }
            }
            else
            {
                finalStream = new FileStream(tempFinal, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite, 8192, true);
                finalStream.SetLength(totalLength);
            }

            var partTasks = new List<Task>();
            var partSemaphore = new SemaphoreSlim(MaxPerFileConcurrency, MaxPerFileConcurrency);
            var partProgress = manifest.PartCompleted ?? new long[parts];

            for (int i = 0; i < parts; i++)
            {
                var pi = partInfos[i];
                await partSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
                var task = Task.Run(async () =>
                {
                    try
                    {
                        await DownloadPartWithResumeAsync(url, destinationPath, pi.Index, pi.Start, pi.End, pi.Size, async (bytes) =>
                        {
                            partProgress[pi.Index] = bytes;
                            manifest.PartCompleted[pi.Index] = bytes;

                            // persist manifest occasionally
                            try
                            {
                                WriteManifest(manifestPath, manifest);
                            }
                            catch { }

                            long totalReceived = partProgress.Sum();
                            // Raise both block and overall progress asynchronously to avoid blocking
                            Task.Run(() => BlockProgressChanged?.Invoke(this, new BlockProgressChangedEventArgs(url, destinationPath, pi.Index, bytes, pi.Size)));

                            // compute simple speed per progress by using control's last bytes
                            var bps = control.ComputeBytesPerSecond(bytes);
                            Task.Run(() => DownloadProgressChanged?.Invoke(this, new DownloadProgressChangedEventArgs(url, destinationPath, totalReceived, totalLength, bps)));

                        }, finalStream, destinationStream != null, control, cancellationToken).ConfigureAwait(false);
                    }
                    finally
                    {
                        partSemaphore.Release();
                    }
                }, cancellationToken);
                partTasks.Add(task);
            }

            await Task.WhenAll(partTasks).ConfigureAwait(false);

            // finalize
            if (destinationStream == null)
            {
                finalStream.Dispose();
                if (File.Exists(destinationPath)) File.Delete(destinationPath);
                File.Move(tempFinal, destinationPath);
                try { if (File.Exists(manifestPath)) File.Delete(manifestPath); } catch { }
            }
            else
            {
                if (!leaveOpen) finalStream.Dispose();
            }
        }

        private string GetPartPath(string destinationPath, int index) => destinationPath + $".part{index}";

        private async Task DownloadPartWithResumeAsync(string url, string destinationPath, int partIndex, long rangeStart, long rangeEnd, long partSize, Action<long> progressCallback, Stream finalStream, bool writingToProvidedStream, DownloadControl control, CancellationToken cancellationToken)
        {
            int attempt = 0;
            while (true)
            {
                attempt++;
                try
                {
                    await control.WaitIfPausedAsync(cancellationToken).ConfigureAwait(false);

                    // check existing bytes from manifest (if present)
                    var manifestPath = destinationPath + ".manifest";
                    long existing = 0;
                    if (File.Exists(manifestPath))
                    {
                        try
                        {
                            var m = LoadManifest(manifestPath);
                            if (m != null && m.PartCompleted != null && partIndex < m.PartCompleted.Length)
                                existing = m.PartCompleted[partIndex];
                        }
                        catch { existing = 0; }
                    }

                    if (existing > partSize)
                    {
                        existing = 0; // corrupted
                    }

                    if (existing == partSize)
                    {
                        progressCallback(existing);
                        return; // already done
                    }

                    var from = rangeStart + existing;
                    var to = rangeEnd;

                    using var request = new HttpRequestMessage(HttpMethod.Get, url);
                    request.Headers.Range = new RangeHeaderValue(from, to);

                    using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();

                    using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

                    var buffer = new byte[65536];
                    int read;
                    long totalRead = existing;
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    long bytesSinceWindow = 0;
                    var lastReportTime = DateTime.UtcNow;

                    while ((read = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) > 0)
                    {
                        await control.WaitIfPausedAsync(cancellationToken).ConfigureAwait(false);

                        // rate limiting
                        if (control.MaxBytesPerSecond.HasValue && control.MaxBytesPerSecond.Value > 0)
                        {
                            bytesSinceWindow += read;
                            var elapsed = sw.Elapsed.TotalSeconds;
                            if (elapsed > 0)
                            {
                                var currentRate = bytesSinceWindow / elapsed;
                                if (currentRate > control.MaxBytesPerSecond.Value)
                                {
                                    var excess = currentRate / control.MaxBytesPerSecond.Value;
                                    var delay = TimeSpan.FromMilliseconds(Math.Min(500, (excess - 1) * 100));
                                    await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                                    sw.Restart();
                                    bytesSinceWindow = 0;
                                }
                            }
                        }

                        // write directly into finalStream at offset
                        var writeOffset = rangeStart + totalRead;
                        if (writingToProvidedStream)
                        {
                            lock (finalStream)
                            {
                                if (finalStream.CanSeek) finalStream.Seek(writeOffset, SeekOrigin.Begin);
                                finalStream.Write(buffer, 0, read);
                            }
                        }
                        else
                        {
                            // open a write stream for the same file region
                            using (var fs = new FileStream(((FileStream)finalStream).Name, FileMode.Open, FileAccess.Write, FileShare.ReadWrite, 65536, true))
                            {
                                fs.Seek(writeOffset, SeekOrigin.Begin);
                                await fs.WriteAsync(buffer, 0, read, cancellationToken).ConfigureAwait(false);
                            }
                        }

                        totalRead += read;

                        // update manifest
                        try
                        {
                            if (File.Exists(manifestPath))
                            {
                                var mm = LoadManifest(manifestPath);
                                if (mm != null)
                                {
                                    mm.PartCompleted[partIndex] = totalRead;
                                    WriteManifest(manifestPath, mm);
                                }
                            }
                        }
                        catch { }

                        progressCallback(totalRead);

                        // speed measurement for control
                        control.AddBytes(read);

                        // short yield to allow cancellation
                        await Task.Yield();
                    }

                    // verify
                    if (totalRead == partSize)
                        return;
                    else
                        throw new IOException("Downloaded part size mismatch");
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception) when (attempt <= MaxRetries)
                {
                    await Task.Delay(RetryDelay * attempt, cancellationToken).ConfigureAwait(false);
                    continue;
                }
            }
        }

        public void Dispose()
        {
            overallSemaphore.Dispose();
        }

        // Manifest for resume
        private class DownloadManifest
        {
            public string Url { get; set; }
            public long TotalLength { get; set; }
            public int Parts { get; set; }
            public long[] PartSizes { get; set; }
            public long[] PartCompleted { get; set; }
        }

        // Control object for pause/resume and simple rate/throughput tracking
        private class DownloadControl
        {
            private readonly object pauseLock = new object();
            private TaskCompletionSource<bool> resumeTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            private long bytesInWindow = 0;
            private readonly TimeSpan window = TimeSpan.FromSeconds(1);
            private DateTime windowStart = DateTime.UtcNow;

            public long? MaxBytesPerSecond { get; set; }

            public void Pause()
            {
                lock (pauseLock)
                {
                    if (!IsPaused)
                    {
                        IsPaused = true;
                        resumeTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                    }
                }
            }

            public void Resume()
            {
                lock (pauseLock)
                {
                    if (IsPaused)
                    {
                        IsPaused = false;
                        resumeTcs.TrySetResult(true);
                    }
                }
            }

            public bool IsPaused { get; private set; }

            public Task WaitIfPausedAsync(CancellationToken cancellationToken)
            {
                lock (pauseLock)
                {
                    if (!IsPaused) return Task.CompletedTask;
                    return resumeTcs.Task.WithCancellation(cancellationToken);
                }
            }

            public void AddBytes(int count)
            {
                var now = DateTime.UtcNow;
                if ((now - windowStart) > window)
                {
                    bytesInWindow = 0;
                    windowStart = now;
                }
                bytesInWindow += count;
            }

            public double? ComputeBytesPerSecond(long latestBytes)
            {
                var now = DateTime.UtcNow;
                var elapsed = (now - windowStart).TotalSeconds;
                if (elapsed <= 0) return null;
                return bytesInWindow / elapsed;
            }
        }

        // Manifest read/write helpers (simple CSV-like format)
        private static DownloadManifest? LoadManifest(string manifestPath)
        {
            try
            {
                var lines = File.ReadAllLines(manifestPath);
                if (lines.Length < 5) return null;
                var m = new DownloadManifest();
                m.Url = lines[0];
                m.TotalLength = long.Parse(lines[1]);
                m.Parts = int.Parse(lines[2]);
                m.PartSizes = lines[3].Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(s => long.Parse(s)).ToArray();
                m.PartCompleted = lines[4].Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(s => long.Parse(s)).ToArray();
                return m;
            }
            catch
            {
                return null;
            }
        }

        private static void WriteManifest(string manifestPath, DownloadManifest m)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine(m.Url ?? string.Empty);
                sb.AppendLine(m.TotalLength.ToString());
                sb.AppendLine(m.Parts.ToString());
                sb.AppendLine(string.Join(",", m.PartSizes ?? new long[0]));
                sb.AppendLine(string.Join(",", m.PartCompleted ?? new long[0]));
                File.WriteAllText(manifestPath, sb.ToString());
            }
            catch { }
        }
    }

    // small helper to add cancellation to TaskCompletionSource awaiting
    internal static class TaskExtensions
    {
        public static async Task WithCancellation(this Task task, CancellationToken cancellationToken)
        {
            using (var ctr = cancellationToken.Register(() => { }))
            {
                var t = await Task.WhenAny(task, Task.Delay(Timeout.Infinite, cancellationToken)).ConfigureAwait(false);
                await t.ConfigureAwait(false);
            }
        }

        public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            using (var ctr = cancellationToken.Register(() => { }))
            {
                var t = await Task.WhenAny(task, Task.Delay(Timeout.Infinite, cancellationToken)).ConfigureAwait(false);
                return await ((Task<T>)t).ConfigureAwait(false);
            }
        }
    }
}
