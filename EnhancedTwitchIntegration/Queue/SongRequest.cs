using System;

namespace SongRequestManager.Queue
{
    public class SongRequest
    {
        public string RequestedBy { get; set; }
        public RequestStatus Status { get; set; }
        public string Comment { get; set; }
        public DateTime Timestamp { get; set; }
        public double PriorityValue { get; set; }
        public Song Song { get; set; }
    }
}
