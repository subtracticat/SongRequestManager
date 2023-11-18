using System;

namespace SongRequestManager.Queue
{
    public class SongRequest
    {
        public string RequestedBy { get; set; }
        public RequestStatus Status { get; set; }
        public string Comment { get; set; }
        public DateTime RequestTimestamp { get; set; }
        public DateTime PlayedTimestamp { get; set; }
        public float PriorityValue { get; set; }
        public Song Song { get; set; }
        public bool IsModPromoted { get; set; }

        public SongRequest() { }
        public SongRequest(Song song, string requestedBy)
        {
            this.Song = song;
            this.RequestedBy = requestedBy;
            this.RequestTimestamp = DateTime.Now;
        }
    }
}
