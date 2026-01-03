using System;
using System.Collections.Generic;

namespace ReunionMovementDLL.Download
{
    /// <summary>
    /// 下载管理器。
    /// </summary>
    internal sealed partial class DownloadManager : ReunionMovementModule, IDownloadManager
    {
        private const int OneMegaBytes = 1024 * 1024;

        private readonly TaskPool<DownloadTask> taskPool;
        private readonly DownloadCounter downloadCounter;
        private int flushSize;
        private float timeout;
        private EventHandler<DownloadStartEventArgs> downloadStartEventHandler;
        private EventHandler<DownloadUpdateEventArgs> downloadUpdateEventHandler;
        private EventHandler<DownloadSuccessEventArgs> downloadSuccessEventHandler;
        private EventHandler<DownloadFailureEventArgs> downloadFailureEventHandler;

        /// <summary>
        /// 初始化下载管理器的新实例。
        /// </summary>
        public DownloadManager()
        {
            taskPool = new TaskPool<DownloadTask>();
            downloadCounter = new DownloadCounter(1f, 10f);
            flushSize = OneMegaBytes;
            timeout = 30f;
            downloadStartEventHandler = null;
            downloadUpdateEventHandler = null;
            downloadSuccessEventHandler = null;
            downloadFailureEventHandler = null;
        }

        /// <summary>
        /// 获取游戏框架模块优先级。
        /// </summary>
        /// <remarks>优先级较高的模块会优先轮询，并且关闭操作会后进行。</remarks>
        internal override int Priority
        {
            get
            {
                return 5;
            }
        }

        /// <summary>
        /// 获取或设置下载是否被暂停。
        /// </summary>
        public bool Paused
        {
            get
            {
                return taskPool.Paused;
            }
            set
            {
                taskPool.Paused = value;
            }
        }

        /// <summary>
        /// 获取下载代理总数量。
        /// </summary>
        public int TotalAgentCount
        {
            get
            {
                return taskPool.TotalAgentCount;
            }
        }

        /// <summary>
        /// 获取可用下载代理数量。
        /// </summary>
        public int FreeAgentCount
        {
            get
            {
                return taskPool.FreeAgentCount;
            }
        }

        /// <summary>
        /// 获取工作中下载代理数量。
        /// </summary>
        public int WorkingAgentCount
        {
            get
            {
                return taskPool.WorkingAgentCount;
            }
        }

        /// <summary>
        /// 获取等待下载任务数量。
        /// </summary>
        public int WaitingTaskCount
        {
            get
            {
                return taskPool.WaitingTaskCount;
            }
        }

        /// <summary>
        /// 获取或设置将缓冲区写入磁盘的临界大小。
        /// </summary>
        public int FlushSize
        {
            get
            {
                return flushSize;
            }
            set
            {
                flushSize = value;
            }
        }

        /// <summary>
        /// 获取或设置下载超时时长，以秒为单位。
        /// </summary>
        public float Timeout
        {
            get
            {
                return timeout;
            }
            set
            {
                timeout = value;
            }
        }

        /// <summary>
        /// 获取当前下载速度。
        /// </summary>
        public float CurrentSpeed
        {
            get
            {
                return downloadCounter.CurrentSpeed;
            }
        }

        /// <summary>
        /// 下载开始事件。
        /// </summary>
        public event EventHandler<DownloadStartEventArgs> DownloadStart
        {
            add
            {
                downloadStartEventHandler += value;
            }
            remove
            {
                downloadStartEventHandler -= value;
            }
        }

        /// <summary>
        /// 下载更新事件。
        /// </summary>
        public event EventHandler<DownloadUpdateEventArgs> DownloadUpdate
        {
            add
            {
                downloadUpdateEventHandler += value;
            }
            remove
            {
                downloadUpdateEventHandler -= value;
            }
        }

        /// <summary>
        /// 下载成功事件。
        /// </summary>
        public event EventHandler<DownloadSuccessEventArgs> DownloadSuccess
        {
            add
            {
                downloadSuccessEventHandler += value;
            }
            remove
            {
                downloadSuccessEventHandler -= value;
            }
        }

        /// <summary>
        /// 下载失败事件。
        /// </summary>
        public event EventHandler<DownloadFailureEventArgs> DownloadFailure
        {
            add
            {
                downloadFailureEventHandler += value;
            }
            remove
            {
                downloadFailureEventHandler -= value;
            }
        }

        /// <summary>
        /// 下载管理器轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
            taskPool.Update(elapseSeconds, realElapseSeconds);
            downloadCounter.Update(elapseSeconds, realElapseSeconds);
        }

        /// <summary>
        /// 关闭并清理下载管理器。
        /// </summary>
        internal override void Shutdown()
        {
            taskPool.Shutdown();
            downloadCounter.Shutdown();
        }

        /// <summary>
        /// 增加下载代理辅助器。
        /// </summary>
        /// <param name="downloadAgentHelper">要增加的下载代理辅助器。</param>
        public void AddDownloadAgentHelper(IDownloadAgentHelper downloadAgentHelper)
        {
            DownloadAgent agent = new DownloadAgent(downloadAgentHelper);
            agent.DownloadAgentStart += OnDownloadAgentStart;
            agent.DownloadAgentUpdate += OnDownloadAgentUpdate;
            agent.DownloadAgentSuccess += OnDownloadAgentSuccess;
            agent.DownloadAgentFailure += OnDownloadAgentFailure;

            taskPool.AddAgent(agent);
        }

        /// <summary>
        /// 根据下载任务的序列编号获取下载任务的信息。
        /// </summary>
        /// <param name="serialId">要获取信息的下载任务的序列编号。</param>
        /// <returns>下载任务的信息。</returns>
        public TaskInfo GetDownloadInfo(int serialId)
        {
            return taskPool.GetTaskInfo(serialId);
        }

        /// <summary>
        /// 根据下载任务的标签获取下载任务的信息。
        /// </summary>
        /// <param name="tag">要获取信息的下载任务的标签。</param>
        /// <returns>下载任务的信息。</returns>
        public TaskInfo[] GetDownloadInfos(string tag)
        {
            return taskPool.GetTaskInfos(tag);
        }

        /// <summary>
        /// 根据下载任务的标签获取下载任务的信息。
        /// </summary>
        /// <param name="tag">要获取信息的下载任务的标签。</param>
        /// <param name="results">下载任务的信息。</param>
        public void GetDownloadInfos(string tag, List<TaskInfo> results)
        {
            taskPool.GetTaskInfos(tag, results);
        }

        /// <summary>
        /// 获取所有下载任务的信息。
        /// </summary>
        /// <returns>所有下载任务的信息。</returns>
        public TaskInfo[] GetAllDownloadInfos()
        {
            return taskPool.GetAllTaskInfos();
        }

        /// <summary>
        /// 获取所有下载任务的信息。
        /// </summary>
        /// <param name="results">所有下载任务的信息。</param>
        public void GetAllDownloadInfos(List<TaskInfo> results)
        {
            taskPool.GetAllTaskInfos(results);
        }

        /// <summary>
        /// 增加下载任务。
        /// </summary>
        /// <param name="downloadPath">下载后存放路径。</param>
        /// <param name="downloadUri">原始下载地址。</param>
        /// <returns>新增下载任务的序列编号。</returns>
        public int AddDownload(string downloadPath, string downloadUri)
        {
            return AddDownload(downloadPath, downloadUri, null, Constant.DefaultPriority, null);
        }

        /// <summary>
        /// 增加下载任务。
        /// </summary>
        /// <param name="downloadPath">下载后存放路径。</param>
        /// <param name="downloadUri">原始下载地址。</param>
        /// <param name="tag">下载任务的标签。</param>
        /// <returns>新增下载任务的序列编号。</returns>
        public int AddDownload(string downloadPath, string downloadUri, string tag)
        {
            return AddDownload(downloadPath, downloadUri, tag, Constant.DefaultPriority, null);
        }

        /// <summary>
        /// 增加下载任务。
        /// </summary>
        /// <param name="downloadPath">下载后存放路径。</param>
        /// <param name="downloadUri">原始下载地址。</param>
        /// <param name="priority">下载任务的优先级。</param>
        /// <returns>新增下载任务的序列编号。</returns>
        public int AddDownload(string downloadPath, string downloadUri, int priority)
        {
            return AddDownload(downloadPath, downloadUri, null, priority, null);
        }

        /// <summary>
        /// 增加下载任务。
        /// </summary>
        /// <param name="downloadPath">下载后存放路径。</param>
        /// <param name="downloadUri">原始下载地址。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>新增下载任务的序列编号。</returns>
        public int AddDownload(string downloadPath, string downloadUri, object userData)
        {
            return AddDownload(downloadPath, downloadUri, null, Constant.DefaultPriority, userData);
        }

        /// <summary>
        /// 增加下载任务。
        /// </summary>
        /// <param name="downloadPath">下载后存放路径。</param>
        /// <param name="downloadUri">原始下载地址。</param>
        /// <param name="tag">下载任务的标签。</param>
        /// <param name="priority">下载任务的优先级。</param>
        /// <returns>新增下载任务的序列编号。</returns>
        public int AddDownload(string downloadPath, string downloadUri, string tag, int priority)
        {
            return AddDownload(downloadPath, downloadUri, tag, priority, null);
        }

        /// <summary>
        /// 增加下载任务。
        /// </summary>
        /// <param name="downloadPath">下载后存放路径。</param>
        /// <param name="downloadUri">原始下载地址。</param>
        /// <param name="tag">下载任务的标签。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>新增下载任务的序列编号。</returns>
        public int AddDownload(string downloadPath, string downloadUri, string tag, object userData)
        {
            return AddDownload(downloadPath, downloadUri, tag, Constant.DefaultPriority, userData);
        }

        /// <summary>
        /// 增加下载任务。
        /// </summary>
        /// <param name="downloadPath">下载后存放路径。</param>
        /// <param name="downloadUri">原始下载地址。</param>
        /// <param name="priority">下载任务的优先级。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>新增下载任务的序列编号。</returns>
        public int AddDownload(string downloadPath, string downloadUri, int priority, object userData)
        {
            return AddDownload(downloadPath, downloadUri, null, priority, userData);
        }

        /// <summary>
        /// 增加下载任务。
        /// </summary>
        /// <param name="downloadPath">下载后存放路径。</param>
        /// <param name="downloadUri">原始下载地址。</param>
        /// <param name="tag">下载任务的标签。</param>
        /// <param name="priority">下载任务的优先级。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>新增下载任务的序列编号。</returns>
        public int AddDownload(string downloadPath, string downloadUri, string tag, int priority, object userData)
        {
            if (string.IsNullOrEmpty(downloadPath))
            {
                throw new ReunionMovementException("下载路径无效。");
            }

            if (string.IsNullOrEmpty(downloadUri))
            {
                throw new ReunionMovementException("下载URI无效。");
            }

            if (TotalAgentCount <= 0)
            {
                throw new ReunionMovementException("您必须先添加下载代理。");
            }

            DownloadTask downloadTask = DownloadTask.Create(downloadPath, downloadUri, tag, priority, flushSize, timeout, userData);
            taskPool.AddTask(downloadTask);
            return downloadTask.SerialId;
        }

        /// <summary>
        /// 根据下载任务的序列编号移除下载任务。
        /// </summary>
        /// <param name="serialId">要移除下载任务的序列编号。</param>
        /// <returns>是否移除下载任务成功。</returns>
        public bool RemoveDownload(int serialId)
        {
            return taskPool.RemoveTask(serialId);
        }

        /// <summary>
        /// 根据下载任务的标签移除下载任务。
        /// </summary>
        /// <param name="tag">要移除下载任务的标签。</param>
        /// <returns>移除下载任务的数量。</returns>
        public int RemoveDownloads(string tag)
        {
            return taskPool.RemoveTasks(tag);
        }

        /// <summary>
        /// 移除所有下载任务。
        /// </summary>
        /// <returns>移除下载任务的数量。</returns>
        public int RemoveAllDownloads()
        {
            return taskPool.RemoveAllTasks();
        }

        private void OnDownloadAgentStart(DownloadAgent sender)
        {
            if (downloadStartEventHandler != null)
            {
                DownloadStartEventArgs downloadStartEventArgs = DownloadStartEventArgs.Create(sender.Task.SerialId, sender.Task.DownloadPath, sender.Task.DownloadUri, sender.CurrentLength, sender.Task.UserData);
                downloadStartEventHandler(this, downloadStartEventArgs);
                ReferencePool.Release(downloadStartEventArgs);
            }
        }

        private void OnDownloadAgentUpdate(DownloadAgent sender, int deltaLength)
        {
            downloadCounter.RecordDeltaLength(deltaLength);
            if (downloadUpdateEventHandler != null)
            {
                DownloadUpdateEventArgs downloadUpdateEventArgs = DownloadUpdateEventArgs.Create(sender.Task.SerialId, sender.Task.DownloadPath, sender.Task.DownloadUri, sender.CurrentLength, sender.Task.UserData);
                downloadUpdateEventHandler(this, downloadUpdateEventArgs);
                ReferencePool.Release(downloadUpdateEventArgs);
            }
        }

        private void OnDownloadAgentSuccess(DownloadAgent sender, long length)
        {
            if (downloadSuccessEventHandler != null)
            {
                DownloadSuccessEventArgs downloadSuccessEventArgs = DownloadSuccessEventArgs.Create(sender.Task.SerialId, sender.Task.DownloadPath, sender.Task.DownloadUri, sender.CurrentLength, sender.Task.UserData);
                downloadSuccessEventHandler(this, downloadSuccessEventArgs);
                ReferencePool.Release(downloadSuccessEventArgs);
            }
        }

        private void OnDownloadAgentFailure(DownloadAgent sender, string errorMessage)
        {
            if (downloadFailureEventHandler != null)
            {
                DownloadFailureEventArgs downloadFailureEventArgs = DownloadFailureEventArgs.Create(sender.Task.SerialId, sender.Task.DownloadPath, sender.Task.DownloadUri, errorMessage, sender.Task.UserData);
                downloadFailureEventHandler(this, downloadFailureEventArgs);
                ReferencePool.Release(downloadFailureEventArgs);
            }
        }
    }
}
