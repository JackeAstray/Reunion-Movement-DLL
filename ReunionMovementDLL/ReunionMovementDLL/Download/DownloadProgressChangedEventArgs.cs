using System;
using System.Collections.Generic;
using System.Text;

namespace ReunionMovementDLL.Download
{
    public class DownloadProgressChangedEventArgs : EventArgs
    {
        public string Url { get; }
        public string Destination { get; }
        public long BytesReceived { get; }
        public long? TotalBytes { get; }
        public double? BytesPerSecond { get; }
        public double? ProgressPercentage => TotalBytes.HasValue && TotalBytes > 0 ? (double)BytesReceived / TotalBytes.Value * 100.0 : (double?)null;

        public DownloadProgressChangedEventArgs(string url, string destination, long bytesReceived, long? totalBytes, double? bytesPerSecond = null)
        {
            Url = url;
            Destination = destination;
            BytesReceived = bytesReceived;
            TotalBytes = totalBytes;
            BytesPerSecond = bytesPerSecond;
        }
    }
}
