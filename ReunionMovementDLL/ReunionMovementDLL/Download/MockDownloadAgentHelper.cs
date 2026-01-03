using System;
using System.Threading;
using System.Threading.Tasks;

namespace ReunionMovementDLL.Download
{
    /// <summary>
    /// 简单的模拟下载辅助器，用于测试并发行为与事件驱动逻辑。
    /// 此辅助器在后台任务中分片触发 UpdateBytes/UpdateLength/Complete 事件。
    /// 调用 Reset() 会取消当前下载。
    /// </summary>
    public sealed class MockDownloadAgentHelper : IDownloadAgentHelper
    {
        public event EventHandler<DownloadAgentHelperUpdateBytesEventArgs> DownloadAgentHelperUpdateBytes;
        public event EventHandler<DownloadAgentHelperUpdateLengthEventArgs> DownloadAgentHelperUpdateLength;
        public event EventHandler<DownloadAgentHelperCompleteEventArgs> DownloadAgentHelperComplete;
        public event EventHandler<DownloadAgentHelperErrorEventArgs> DownloadAgentHelperError;

        private CancellationTokenSource cts;
        private readonly object @lock = new object();

        // 模拟总长度，测试时可调整。
        public long TotalLength { get; set; } = 200 * 1024; // 200KB
        public int ChunkSize { get; set; } = 8 * 1024; // 8KB
        public int DelayPerChunkMs { get; set; } = 10;

        public MockDownloadAgentHelper()
        {
            cts = null;
        }

        public void Download(string downloadUri, object userData)
        {
            Download(downloadUri, 0L, -1L, userData);
        }

        public void Download(string downloadUri, long fromPosition, object userData)
        {
            Download(downloadUri, fromPosition, -1L, userData);
        }

        public void Download(string downloadUri, long fromPosition, long toPosition, object userData)
        {
            lock (@lock)
            {
                if (cts != null)
                {
                    // already running
                    RaiseError(false, "下载已在运行");
                    return;
                }

                cts = new CancellationTokenSource();
            }

            CancellationToken token = cts.Token;
            long start = Math.Max(0L, fromPosition);
            long end = toPosition >= 0 ? Math.Min(toPosition, TotalLength) : TotalLength;

            Task.Run(async () =>
            {
                try
                {
                    long position = start;
                    // if resuming, notify start length via UpdateLength with zero delta? we just proceed to send bytes from position
                    while (position < end)
                    {
                        if (token.IsCancellationRequested)
                        {
                            return;
                        }

                        int remaining = (int)Math.Min(ChunkSize, end - position);
                        byte[] buffer = new byte[remaining];
                        // fill with pseudo data
                        for (int i = 0; i < remaining; i++)
                        {
                            buffer[i] = (byte)(position + i);
                        }

                        // create and raise UpdateBytes
                        var bytesArgs = DownloadAgentHelperUpdateBytesEventArgs.Create(buffer, 0, remaining);
                        try
                        {
                            DownloadAgentHelperUpdateBytes?.Invoke(this, bytesArgs);
                        }
                        catch
                        {
                            // swallowing subscriber exceptions; test harness should detect issues via logs
                        }
                        finally
                        {
                            // helper is responsible to release pooled args
                            ReferencePool.Release(bytesArgs);
                        }

                        // create and raise UpdateLength
                        var lengthArgs = DownloadAgentHelperUpdateLengthEventArgs.Create(remaining);
                        try
                        {
                            DownloadAgentHelperUpdateLength?.Invoke(this, lengthArgs);
                        }
                        catch
                        {
                        }
                        finally
                        {
                            ReferencePool.Release(lengthArgs);
                        }

                        position += remaining;

                        await Task.Delay(DelayPerChunkMs).ConfigureAwait(false);
                    }

                    // complete
                    var completeArgs = DownloadAgentHelperCompleteEventArgs.Create(end - start);
                    try
                    {
                        DownloadAgentHelperComplete?.Invoke(this, completeArgs);
                    }
                    catch
                    {
                    }
                    finally
                    {
                        ReferencePool.Release(completeArgs);
                    }
                }
                catch (Exception ex)
                {
                    RaiseError(true, ex.ToString());
                }
                finally
                {
                    lock (@lock)
                    {
                        if (cts != null)
                        {
                            cts.Dispose();
                            cts = null;
                        }
                    }
                }
            }, token);
        }

        public void Reset()
        {
            lock (@lock)
            {
                if (cts != null)
                {
                    try
                    {
                        cts.Cancel();
                    }
                    catch
                    {
                    }

                    try
                    {
                        cts.Dispose();
                    }
                    catch
                    {
                    }

                    cts = null;
                }
            }
        }

        private void RaiseError(bool deleteDownloading, string message)
        {
            var errorArgs = DownloadAgentHelperErrorEventArgs.Create(deleteDownloading, message);
            try
            {
                DownloadAgentHelperError?.Invoke(this, errorArgs);
            }
            catch
            {
            }
            finally
            {
                ReferencePool.Release(errorArgs);
            }
        }
    }
}
