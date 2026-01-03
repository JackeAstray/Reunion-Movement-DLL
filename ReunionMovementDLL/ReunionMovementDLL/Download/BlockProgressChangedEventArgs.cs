using System;
using System.Collections.Generic;
using System.Text;

namespace ReunionMovementDLL.Download
{
    public class BlockProgressChangedEventArgs : EventArgs
    {
        public string Url { get; }
        public string Destination { get; }
        public int PartIndex { get; }
        public long BytesReceived { get; }
        public long PartSize { get; }
        public double ProgressPercentage => PartSize > 0 ? (double)BytesReceived / PartSize * 100.0 : 0.0;

        public BlockProgressChangedEventArgs(string url, string destination, int partIndex, long bytesReceived, long partSize)
        {
            Url = url;
            Destination = destination;
            PartIndex = partIndex;
            BytesReceived = bytesReceived;
            PartSize = partSize;
        }
    }
}
