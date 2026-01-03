namespace ReunionMovementDLL.Download
{
    internal sealed partial class DownloadManager : ReunionMovementModule, IDownloadManager
    {
        /// <summary>
        /// 下载任务。
        /// </summary>
        private sealed class DownloadTask : TaskBase
        {
            private static int serial = 0;

            private DownloadTaskStatus status;
            private string downloadPath;
            private string downloadUri;
            private int flushSize;
            private float timeout;

            /// <summary>
            /// 初始化下载任务的新实例。
            /// </summary>
            public DownloadTask()
            {
                status = DownloadTaskStatus.Todo;
                downloadPath = null;
                downloadUri = null;
                flushSize = 0;
                timeout = 0f;
            }

            /// <summary>
            /// 获取或设置下载任务的状态。
            /// </summary>
            public DownloadTaskStatus Status
            {
                get
                {
                    return status;
                }
                set
                {
                    status = value;
                }
            }

            /// <summary>
            /// 获取下载后存放路径。
            /// </summary>
            public string DownloadPath
            {
                get
                {
                    return downloadPath;
                }
            }

            /// <summary>
            /// 获取原始下载地址。
            /// </summary>
            public string DownloadUri
            {
                get
                {
                    return downloadUri;
                }
            }

            /// <summary>
            /// 获取将缓冲区写入磁盘的临界大小。
            /// </summary>
            public int FlushSize
            {
                get
                {
                    return flushSize;
                }
            }

            /// <summary>
            /// 获取下载超时时长，以秒为单位。
            /// </summary>
            public float Timeout
            {
                get
                {
                    return timeout;
                }
            }

            /// <summary>
            /// 获取下载任务的描述。
            /// </summary>
            public override string Description
            {
                get
                {
                    return downloadPath;
                }
            }

            /// <summary>
            /// 创建下载任务。
            /// </summary>
            /// <param name="downloadPath">下载后存放路径。</param>
            /// <param name="downloadUri">原始下载地址。</param>
            /// <param name="tag">下载任务的标签。</param>
            /// <param name="priority">下载任务的优先级。</param>
            /// <param name="flushSize">将缓冲区写入磁盘的临界大小。</param>
            /// <param name="timeout">下载超时时长，以秒为单位。</param>
            /// <param name="userData">用户自定义数据。</param>
            /// <returns>创建的下载任务。</returns>
            public static DownloadTask Create(string downloadPath, string downloadUri, string tag, int priority, int flushSize, float timeout, object userData)
            {
                DownloadTask downloadTask = ReferencePool.Acquire<DownloadTask>();
                // Use Interlocked to ensure serial increment is thread-safe.
                downloadTask.Initialize(System.Threading.Interlocked.Increment(ref serial), tag, priority, userData);
                downloadTask.downloadPath = downloadPath;
                downloadTask.downloadUri = downloadUri;
                downloadTask.flushSize = flushSize;
                downloadTask.timeout = timeout;
                return downloadTask;
            }

            /// <summary>
            /// 清理下载任务。
            /// </summary>
            public override void Clear()
            {
                base.Clear();
                status = DownloadTaskStatus.Todo;
                downloadPath = null;
                downloadUri = null;
                flushSize = 0;
                timeout = 0f;
            }
        }
    }
}
