using System;
using System.Collections.Generic;
using System.Text;

namespace ReunionMovementDLL.Download
{
    public class DownloadCompletedEventArgs : EventArgs
    {
        public string Url { get; }
        public string Destination { get; }
        public bool Success { get; }
        public Exception Error { get; }

        public DownloadCompletedEventArgs(string url, string destination, bool success, Exception error = null)
        {
            Url = url;
            Destination = destination;
            Success = success;
            Error = error;
        }
    }
}
