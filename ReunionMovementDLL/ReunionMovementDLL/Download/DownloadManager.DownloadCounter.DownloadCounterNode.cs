namespace ReunionMovementDLL.Download
{
    internal sealed partial class DownloadManager : ReunionMovementModule, IDownloadManager
    {
        private sealed partial class DownloadCounter
        {
            private sealed class DownloadCounterNode : IReference
            {
                private long deltaLength;
                private float elapseSeconds;

                public DownloadCounterNode()
                {
                    deltaLength = 0L;
                    elapseSeconds = 0f;
                }

                public long DeltaLength
                {
                    get
                    {
                        return deltaLength;
                    }
                }

                public float ElapseSeconds
                {
                    get
                    {
                        return elapseSeconds;
                    }
                }

                public static DownloadCounterNode Create()
                {
                    return ReferencePool.Acquire<DownloadCounterNode>();
                }

                public void Update(float elapseSeconds, float realElapseSeconds)
                {
                    this.elapseSeconds += realElapseSeconds;
                }

                public void AddDeltaLength(int deltaLength)
                {
                    this.deltaLength += deltaLength;
                }

                public void Clear()
                {
                    deltaLength = 0L;
                    elapseSeconds = 0f;
                }
            }
        }
    }
}
