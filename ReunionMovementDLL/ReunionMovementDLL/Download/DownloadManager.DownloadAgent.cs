using System;
using System.IO;
using System.Threading.Tasks;

namespace ReunionMovementDLL.Download
{
    internal sealed partial class DownloadManager : ReunionMovementModule, IDownloadManager
    {
        /// <summary>
        /// 下载代理。
        /// </summary>
        private sealed class DownloadAgent : ITaskAgent<DownloadTask>, IDisposable
        {
            private readonly IDownloadAgentHelper helper;
            private DownloadTask task;
            private FileStream fileStream;
            private int waitFlushSize;
            private float waitTime;
            private long startLength;
            private long downloadedLength;
            private long savedLength;
            private bool disposed;
            private readonly object fileLock = new object();

            public ReunionMovementAction<DownloadAgent> DownloadAgentStart;
            public ReunionMovementAction<DownloadAgent, int> DownloadAgentUpdate;
            public ReunionMovementAction<DownloadAgent, long> DownloadAgentSuccess;
            public ReunionMovementAction<DownloadAgent, string> DownloadAgentFailure;

            /// <summary>
            /// 初始化下载代理的新实例。
            /// </summary>
            /// <param name="downloadAgentHelper">下载代理辅助器。</param>
            public DownloadAgent(IDownloadAgentHelper downloadAgentHelper)
            {
                if (downloadAgentHelper == null)
                {
                    throw new ReunionMovementException("下载代理辅助器无效。");
                }

                helper = downloadAgentHelper;
                task = null;
                fileStream = null;
                waitFlushSize = 0;
                waitTime = 0f;
                startLength = 0L;
                downloadedLength = 0L;
                savedLength = 0L;
                disposed = false;

                DownloadAgentStart = null;
                DownloadAgentUpdate = null;
                DownloadAgentSuccess = null;
                DownloadAgentFailure = null;
            }

            /// <summary>
            /// 获取下载任务。
            /// </summary>
            public DownloadTask Task
            {
                get
                {
                    return task;
                }
            }

            /// <summary>
            /// 获取已经等待时间。
            /// </summary>
            public float WaitTime
            {
                get
                {
                    return waitTime;
                }
            }

            /// <summary>
            /// 获取开始下载时已经存在的大小。
            /// </summary>
            public long StartLength
            {
                get
                {
                    return startLength;
                }
            }

            /// <summary>
            /// 获取本次已经下载的大小。
            /// </summary>
            public long DownloadedLength
            {
                get
                {
                    return downloadedLength;
                }
            }

            /// <summary>
            /// 获取当前的大小。
            /// </summary>
            public long CurrentLength
            {
                get
                {
                    return startLength + downloadedLength;
                }
            }

            /// <summary>
            /// 获取已经存盘的大小。
            /// </summary>
            public long SavedLength
            {
                get
                {
                    return savedLength;
                }
            }

            /// <summary>
            /// 初始化下载代理。
            /// </summary>
            public void Initialize()
            {
                helper.DownloadAgentHelperUpdateBytes += OnDownloadAgentHelperUpdateBytes;
                helper.DownloadAgentHelperUpdateLength += OnDownloadAgentHelperUpdateLength;
                helper.DownloadAgentHelperComplete += OnDownloadAgentHelperComplete;
                helper.DownloadAgentHelperError += OnDownloadAgentHelperError;
            }

            /// <summary>
            /// 下载代理轮询。
            /// </summary>
            /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
            /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
            public void Update(float elapseSeconds, float realElapseSeconds)
            {
                if (task.Status == DownloadTaskStatus.Doing)
                {
                    waitTime += realElapseSeconds;
                    if (waitTime >= task.Timeout)
                    {
                        DownloadAgentHelperErrorEventArgs downloadAgentHelperErrorEventArgs = DownloadAgentHelperErrorEventArgs.Create(false, "超时");
                        OnDownloadAgentHelperError(this, downloadAgentHelperErrorEventArgs);
                        ReferencePool.Release(downloadAgentHelperErrorEventArgs);
                    }
                }
            }

            /// <summary>
            /// 关闭并清理下载代理。
            /// </summary>
            public void Shutdown()
            {
                Dispose();

                helper.DownloadAgentHelperUpdateBytes -= OnDownloadAgentHelperUpdateBytes;
                helper.DownloadAgentHelperUpdateLength -= OnDownloadAgentHelperUpdateLength;
                helper.DownloadAgentHelperComplete -= OnDownloadAgentHelperComplete;
                helper.DownloadAgentHelperError -= OnDownloadAgentHelperError;
            }

            /// <summary>
            /// 开始处理下载任务。
            /// </summary>
            /// <param name="task">要处理的下载任务。</param>
            /// <returns>开始处理任务的状态。</returns>
            public StartTaskStatus Start(DownloadTask task)
            {
                if (task == null)
                {
                    throw new ReunionMovementException("任务无效。");
                }

                this.task = task;

                this.task.Status = DownloadTaskStatus.Doing;
                string downloadFile = Utility.Text.Format("{0}.download", this.task.DownloadPath);

                try
                {
                    if (File.Exists(downloadFile))
                    {
                        lock (fileLock)
                        {
                            fileStream = File.OpenWrite(downloadFile);
                            fileStream.Seek(0L, SeekOrigin.End);
                            startLength = savedLength = fileStream.Length;
                            downloadedLength = 0L;
                        }
                    }
                    else
                    {
                        string directory = Path.GetDirectoryName(this.task.DownloadPath);
                        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        lock (fileLock)
                        {
                            fileStream = new FileStream(downloadFile, FileMode.Create, FileAccess.Write);
                            startLength = savedLength = downloadedLength = 0L;
                        }
                    }

                    if (DownloadAgentStart != null)
                    {
                        // Dispatch start event asynchronously so subscriber code won't break internal flow.
                        System.Threading.Tasks.Task.Run(() =>
                        {
                            try
                            {
                                DownloadAgentStart(this);
                            }
                            catch (Exception ex)
                            {
                                ReunionMovementLog.Error("DownloadAgentStart 订阅者抛出异常：{0}", ex.ToString());
                            }
                        });
                    }

                    if (startLength > 0L)
                    {
                        helper.Download(this.task.DownloadUri, startLength, this.task.UserData);
                    }
                    else
                    {
                        helper.Download(this.task.DownloadUri, this.task.UserData);
                    }

                    return StartTaskStatus.CanResume;
                }
                catch (Exception exception)
                {
                    ReunionMovementLog.Error("下载代理启动失败：{0}", exception.ToString());
                    DownloadAgentHelperErrorEventArgs downloadAgentHelperErrorEventArgs = DownloadAgentHelperErrorEventArgs.Create(false, exception.ToString());
                    OnDownloadAgentHelperError(this, downloadAgentHelperErrorEventArgs);
                    ReferencePool.Release(downloadAgentHelperErrorEventArgs);
                    return StartTaskStatus.UnknownError;
                }
            }

            /// <summary>
            /// 重置下载代理。
            /// </summary>
            public void Reset()
            {
                helper.Reset();

                lock (fileLock)
                {
                    if (fileStream != null)
                    {
                        fileStream.Close();
                        fileStream = null;
                    }
                }

                task = null;
                waitFlushSize = 0;
                waitTime = 0f;
                startLength = 0L;
                downloadedLength = 0L;
                savedLength = 0L;
            }

            /// <summary>
            /// 释放资源。
            /// </summary>
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// 释放资源。
            /// </summary>
            /// <param name="disposing">释放资源标记。</param>
            private void Dispose(bool disposing)
            {
                if (disposed)
                {
                    return;
                }

                if (disposing)
                {
                    lock (fileLock)
                    {
                        if (fileStream != null)
                        {
                            try
                            {
                                fileStream.Dispose();
                            }
                            catch (Exception ex)
                            {
                                ReunionMovementLog.Error("释放文件流时出错：{0}", ex.ToString());
                            }

                            fileStream = null;
                        }
                    }
                }

                disposed = true;
            }

            private void OnDownloadAgentHelperUpdateBytes(object sender, DownloadAgentHelperUpdateBytesEventArgs e)
            {
                waitTime = 0f;
                try
                {
                    lock (fileLock)
                    {
                        fileStream.Write(e.GetBytes(), e.Offset, e.Length);
                        waitFlushSize += e.Length;
                        savedLength += e.Length;

                        if (waitFlushSize >= task.FlushSize)
                        {
                            fileStream.Flush();
                            waitFlushSize = 0;
                        }
                    }
                }
                catch (Exception exception)
                {
                    ReunionMovementLog.Error("写入下载字节时出错：{0}", exception.ToString());
                    DownloadAgentHelperErrorEventArgs downloadAgentHelperErrorEventArgs = DownloadAgentHelperErrorEventArgs.Create(false, exception.ToString());
                    OnDownloadAgentHelperError(this, downloadAgentHelperErrorEventArgs);
                    ReferencePool.Release(downloadAgentHelperErrorEventArgs);
                }
            }

            private void OnDownloadAgentHelperUpdateLength(object sender, DownloadAgentHelperUpdateLengthEventArgs e)
            {
                waitTime = 0f;
                downloadedLength += e.DeltaLength;
                if (DownloadAgentUpdate != null)
                {
                    System.Threading.Tasks.Task.Run(() =>
                    {
                        try
                        {
                            DownloadAgentUpdate(this, e.DeltaLength);
                        }
                        catch (Exception ex)
                        {
                            ReunionMovementLog.Error("DownloadAgentUpdate 订阅者抛出异常：{0}", ex.ToString());
                        }
                    });
                }
            }

            private void OnDownloadAgentHelperComplete(object sender, DownloadAgentHelperCompleteEventArgs e)
            {
                waitTime = 0f;
                downloadedLength = e.Length;
                if (savedLength != CurrentLength)
                {
                    ReunionMovementLog.Error("已保存长度不匹配。已保存：{0}，当前：{1}", savedLength, CurrentLength);
                    throw new ReunionMovementException("内部下载错误。");
                }

                helper.Reset();

                lock (fileLock)
                {
                    if (fileStream != null)
                    {
                        try
                        {
                            fileStream.Flush();
                        }
                        catch (Exception ex)
                        {
                            ReunionMovementLog.Error("完成时刷新文件流出错：{0}", ex.ToString());
                        }

                        fileStream.Close();
                        fileStream = null;
                    }
                }

                try
                {
                    if (File.Exists(task.DownloadPath))
                    {
                        File.Delete(task.DownloadPath);
                    }

                    File.Move(Utility.Text.Format("{0}.download", task.DownloadPath), task.DownloadPath);
                }
                catch (Exception ex)
                {
                    ReunionMovementLog.Error("移动完成文件到目标位置时出错：{0}", ex.ToString());
                    DownloadAgentHelperErrorEventArgs downloadAgentHelperErrorEventArgs = DownloadAgentHelperErrorEventArgs.Create(true, ex.ToString());
                    OnDownloadAgentHelperError(this, downloadAgentHelperErrorEventArgs);
                    ReferencePool.Release(downloadAgentHelperErrorEventArgs);
                    return;
                }

                task.Status = DownloadTaskStatus.Done;

                if (DownloadAgentSuccess != null)
                {
                    System.Threading.Tasks.Task.Run(() =>
                    {
                        try
                        {
                            DownloadAgentSuccess(this, e.Length);
                        }
                        catch (Exception ex)
                        {
                            ReunionMovementLog.Error("DownloadAgentSuccess 订阅者抛出异常：{0}", ex.ToString());
                        }
                    });
                }

                task.Done = true;
            }

            private void OnDownloadAgentHelperError(object sender, DownloadAgentHelperErrorEventArgs e)
            {
                helper.Reset();

                lock (fileLock)
                {
                    if (fileStream != null)
                    {
                        try
                        {
                            fileStream.Close();
                        }
                        catch (Exception ex)
                        {
                            ReunionMovementLog.Error("发生错误时关闭文件流失败：{0}", ex.ToString());
                        }

                        fileStream = null;
                    }
                }

                if (e.DeleteDownloading)
                {
                    try
                    {
                        File.Delete(Utility.Text.Format("{0}.download", task.DownloadPath));
                    }
                    catch (Exception ex)
                    {
                        ReunionMovementLog.Error("删除下载中临时文件时出错：{0}", ex.ToString());
                    }
                }

                task.Status = DownloadTaskStatus.Error;

                if (DownloadAgentFailure != null)
                {
                    System.Threading.Tasks.Task.Run(() =>
                    {
                        try
                        {
                            DownloadAgentFailure(this, e.ErrorMessage);
                        }
                        catch (Exception ex)
                        {
                            ReunionMovementLog.Error("DownloadAgentFailure 订阅者抛出异常：{0}", ex.ToString());
                        }
                    });
                }

                task.Done = true;
            }
        }
    }
}
