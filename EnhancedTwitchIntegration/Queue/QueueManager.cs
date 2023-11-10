using System;
using System.Collections.Generic;
using System.Linq;

namespace SongRequestManager.Queue
{
    public class QueuePosition
    {
        public int Position { get; set; }
        public int DurationAheadSeconds { get; set; }
    }

    public class QueueManager
    {
        private static QueueManager instance;

        private static QueueManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new QueueManager();
                }

                return instance;
            }
        }

        public List<SongRequest> Requests { get; private set; } = new List<SongRequest>();
        public List<SongRequest> History { get; private set; } = new List<SongRequest>();

        public static SongRequest GetRequestById(string id)
        {
            return Instance.Requests.FirstOrDefault(request => request.Song.ID.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        public static SongRequest GetRequestByUsername(string username)
        {
            return Instance.Requests.FirstOrDefault(request => request.RequestedBy.Equals(username, StringComparison.CurrentCultureIgnoreCase));
        }

        public static QueuePosition GetPositionOf(SongRequest request)
        {
            int index = Instance.Requests.IndexOf(request);
            if (index >= 0)
            {
                return new QueuePosition
                {
                    Position = index + 1,
                    DurationAheadSeconds = (int)Instance.Requests.GetRange(0, index).Sum(r => r.Song.Metadata.Duration)
                };
            }

            return null;
        }

        public static QueuePosition Add(SongRequest request)
        {
            int previousCount = Instance.Requests.Count;
            double previousDuration = Instance.Requests.Sum(r => r.Song.Metadata.Duration);

            Instance.Requests.Add(request);

            return new QueuePosition
            {
                Position = previousCount + 1,
                DurationAheadSeconds = (int)previousDuration
            };
        }

        public static QueuePosition InsertAt(int index, SongRequest request)
        {
            Instance.Requests.Insert(index, request);

            return new QueuePosition
            {
                Position = index + 1,
                DurationAheadSeconds = (int)Instance.Requests.GetRange(0, index).Sum(r => r.Song.Metadata.Duration)
            };
        }

        public static QueuePosition ReplaceSong(SongRequest request, Song newSong)
        {
            int index = Instance.Requests.IndexOf(request);
            if (index >= 0)
            {
                Instance.Requests[index].Song = newSong;

                return new QueuePosition
                {
                    Position = index + 1,
                    DurationAheadSeconds = (int)Instance.Requests.GetRange(0, index).Sum(r => r.Song.Metadata.Duration)
                };
            }

            return null;
        }

        public static SongRequest Remove(string id, RequestStatus newStatus = RequestStatus.Deleted)
        {
            SongRequest request = null;

            int index = Instance.Requests.FindIndex(item => item.Song.ID.Equals(id, StringComparison.OrdinalIgnoreCase));
            if (index >= 0)
            {
                request = Instance.Requests[index];
                Instance.Requests.RemoveAt(index);

                request.Status = newStatus;

                switch (newStatus)
                {
                    case RequestStatus.Played:
                        request.PlayedTimestamp = DateTime.Now;
                        Instance.History.Insert(0, request);
                        break;

                    case RequestStatus.Skipped:
                        Instance.History.Insert(0, request);
                        break;

                    default:
                        break;
                }
            }

            return request;
        }

        public static int ClearQueue()
        {
            int songCount = Instance.Requests.Count;

            foreach (var request in Instance.Requests)
            {
                request.Status = RequestStatus.Skipped;
                Instance.History.Insert(0, request);
            }

            Instance.Requests.Clear();

            return songCount;
        }

        public static int ClearHistory()
        {
            int songCount = Instance.History.Count;
            Instance.History.Clear();

            return songCount;
        }

        public static bool HasPlayed(string id)
        {
            return Instance.History.Any(request => request.Status == RequestStatus.Played && request.Song.ID.Equals(id, StringComparison.OrdinalIgnoreCase));
        }
    }
}
