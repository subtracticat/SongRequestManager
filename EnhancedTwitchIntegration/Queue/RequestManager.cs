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

    public class RequestManager
    {
        private static RequestManager instance;

        private static RequestManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RequestManager();
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
                    DurationAheadSeconds = Instance.Requests.GetRange(0, index).Sum(r => r.Song.DurationSeconds)
                };
            }

            return null;
        }

        public static QueuePosition Add(SongRequest request)
        {
            int previousCount = Instance.Requests.Count;
            int previousDuration = Instance.Requests.Sum(r => r.Song.DurationSeconds);

            Instance.Requests.Add(request);

            return new QueuePosition
            {
                Position = previousCount + 1,
                DurationAheadSeconds = previousDuration
            };
        }

        public static QueuePosition Replace(SongRequest request, Song newSong)
        {
            int index = Instance.Requests.IndexOf(request);
            if (index >= 0)
            {
                Instance.Requests[index].Song = newSong;

                return new QueuePosition
                {
                    Position = index + 1,
                    DurationAheadSeconds = Instance.Requests.GetRange(0, index).Sum(r => r.Song.DurationSeconds)
                };
            }

            return null;
        }

        public static SongRequest Remove(string id)
        {
            int index = Instance.Requests.FindIndex(request => request.Song.ID.Equals(id, StringComparison.OrdinalIgnoreCase));
            if (index >= 0)
            {
                var request = Instance.Requests[index];
                Instance.Requests.RemoveAt(index);
                return request;
            }

            return null;
        }

        public static bool HasPlayed(string id)
        {
            return Instance.History.Any(request => request.Status == RequestStatus.Played && request.Song.ID.Equals(id, StringComparison.OrdinalIgnoreCase));
        }
    }
}
