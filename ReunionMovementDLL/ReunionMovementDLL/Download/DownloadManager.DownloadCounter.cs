namespace ReunionMovementDLL.Download
{
    internal sealed partial class DownloadManager : ReunionMovementModule, IDownloadManager
    {
        private sealed partial class DownloadCounter
        {
            private readonly ReunionMovementLinkedList<DownloadCounterNode> downloadCounterNodes;
            private float updateInterval;
            private float recordInterval;
            private float currentSpeed;
            private float accumulator;
            private float timeLeft;

            public DownloadCounter(float updateInterval, float recordInterval)
            {
                if (updateInterval <= 0f)
                {
                    throw new ReunionMovementException("更新间隔无效。");
                }

                if (recordInterval <= 0f)
                {
                    throw new ReunionMovementException("记录间隔无效。");
                }

                downloadCounterNodes = new ReunionMovementLinkedList<DownloadCounterNode>();
                this.updateInterval = updateInterval;
                this.recordInterval = recordInterval;
                Reset();
            }

            public float UpdateInterval
            {
                get
                {
                    return updateInterval;
                }
                set
                {
                    if (value <= 0f)
                    {
                        throw new ReunionMovementException("更新间隔无效。");
                    }

                    updateInterval = value;
                    Reset();
                }
            }

            public float RecordInterval
            {
                get
                {
                    return recordInterval;
                }
                set
                {
                    if (value <= 0f)
                    {
                        throw new ReunionMovementException("记录间隔无效。");
                    }

                    recordInterval = value;
                    Reset();
                }
            }

            public float CurrentSpeed
            {
                get
                {
                    return currentSpeed;
                }
            }

            public void Shutdown()
            {
                Reset();
            }

            public void Update(float elapseSeconds, float realElapseSeconds)
            {
                if (downloadCounterNodes.Count <= 0)
                {
                    return;
                }

                accumulator += realElapseSeconds;
                if (accumulator > recordInterval)
                {
                    accumulator = recordInterval;
                }

                timeLeft -= realElapseSeconds;
                foreach (DownloadCounterNode downloadCounterNode in downloadCounterNodes)
                {
                    downloadCounterNode.Update(elapseSeconds, realElapseSeconds);
                }

                while (downloadCounterNodes.Count > 0)
                {
                    DownloadCounterNode downloadCounterNode = downloadCounterNodes.First.Value;
                    if (downloadCounterNode.ElapseSeconds < recordInterval)
                    {
                        break;
                    }

                    ReferencePool.Release(downloadCounterNode);
                    downloadCounterNodes.RemoveFirst();
                }

                if (downloadCounterNodes.Count <= 0)
                {
                    Reset();
                    return;
                }

                if (timeLeft <= 0f)
                {
                    long totalDeltaLength = 0L;
                    foreach (DownloadCounterNode downloadCounterNode in downloadCounterNodes)
                    {
                        totalDeltaLength += downloadCounterNode.DeltaLength;
                    }

                    currentSpeed = accumulator > 0f ? totalDeltaLength / accumulator : 0f;
                    timeLeft += updateInterval;
                }
            }

            public void RecordDeltaLength(int deltaLength)
            {
                if (deltaLength <= 0)
                {
                    return;
                }

                DownloadCounterNode downloadCounterNode = null;
                if (downloadCounterNodes.Count > 0)
                {
                    downloadCounterNode = downloadCounterNodes.Last.Value;
                    if (downloadCounterNode.ElapseSeconds < updateInterval)
                    {
                        downloadCounterNode.AddDeltaLength(deltaLength);
                        return;
                    }
                }

                downloadCounterNode = DownloadCounterNode.Create();
                downloadCounterNode.AddDeltaLength(deltaLength);
                downloadCounterNodes.AddLast(downloadCounterNode);
            }

            private void Reset()
            {
                downloadCounterNodes.Clear();
                currentSpeed = 0f;
                accumulator = 0f;
                timeLeft = 0f;
            }
        }
    }
}
