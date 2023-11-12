using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SongRequestManager.Config;

namespace SongRequestManager.Queue
{
    public class QueuePosition
    {
        public int Position { get; set; }
        public int DurationAheadSeconds { get; set; }
    }

    public class QueueData
    {
        public List<SongRequest> Requests = new List<SongRequest>();
        public List<SongRequest> History = new List<SongRequest>();
    }

    public class RequestQueue : PersistedStateManager<QueueData>
    {
        protected override string FileName => "SRMQueue.json";

        private static RequestQueue instance;
        public static RequestQueue Current
        {
            get
            {
                if (instance == null)
                {
                    instance = new RequestQueue();
                }

                return instance;
            }
        }

        public override void Update(Action<QueueData> updateFunc)
        {
            base.Update(updateFunc);

            if (RequestBotListViewController.Instance.isActivated)
            {
                RequestBotListViewController.Instance.UpdateRequestUI(true);
                RequestBotListViewController.Instance.SetUIInteractivity();
            }
        }

        public SongRequest GetRequestById(string id)
        {
            return this.Data.Requests.FirstOrDefault(request => request.Song.ID.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        public SongRequest GetRequestByUsername(string username)
        {
            return this.Data.Requests.FirstOrDefault(request => request.RequestedBy.Equals(username, StringComparison.CurrentCultureIgnoreCase));
        }

        public QueuePosition GetPositionOf(SongRequest request)
        {
            int index = this.Data.Requests.IndexOf(request);
            if (index >= 0)
            {
                return new QueuePosition
                {
                    Position = index + 1,
                    DurationAheadSeconds = (int)this.Data.Requests.GetRange(0, index).Sum(r => r.Song.Metadata.Duration)
                };
            }

            return null;
        }

        public QueuePosition Add(SongRequest request)
        {
            int previousCount = this.Data.Requests.Count;
            double previousDuration = this.Data.Requests.Sum(r => r.Song.Metadata.Duration);

            request.Status = RequestStatus.Queued;
            this.Update(config => config.Requests.Add(request));

            return new QueuePosition
            {
                Position = previousCount + 1,
                DurationAheadSeconds = (int)previousDuration
            };
        }

        public void Add(IEnumerable<SongRequest> requests)
        {
            this.Update(config => config.Requests.AddRange(requests));
        }

        public QueuePosition InsertAt(int index, SongRequest request)
        {
            this.Update(config => config.Requests.Insert(index, request));

            return new QueuePosition
            {
                Position = index + 1,
                DurationAheadSeconds = (int)this.Data.Requests.GetRange(0, index).Sum(r => r.Song.Metadata.Duration)
            };
        }

        public QueuePosition ReplaceSong(SongRequest request, Song newSong)
        {
            int index = this.Data.Requests.IndexOf(request);
            if (index >= 0)
            {
                this.Update(config => config.Requests[index].Song = newSong);

                return new QueuePosition
                {
                    Position = index + 1,
                    DurationAheadSeconds = (int)this.Data.Requests.GetRange(0, index).Sum(r => r.Song.Metadata.Duration)
                };
            }

            return null;
        }

        public SongRequest Remove(string id, RequestStatus newStatus = RequestStatus.Deleted)
        {
            SongRequest request = null;

            int index = this.Data.Requests.FindIndex(item => item.Song.ID.Equals(id, StringComparison.OrdinalIgnoreCase));
            if (index >= 0)
            {
                request = this.Data.Requests[index];
                this.Update(config =>
                {
                    config.Requests.RemoveAt(index);
                    request.Status = newStatus;

                    switch (newStatus)
                    {
                        case RequestStatus.Played:
                            request.PlayedTimestamp = DateTime.Now;
                            config.History.Insert(0, request);
                            break;

                        case RequestStatus.Skipped:
                            config.History.Insert(0, request);
                            break;

                        default:
                            break;
                    }
                });
            }

            return request;
        }

        public int ClearQueue()
        {
            int songCount = this.Data.Requests.Count;

            this.Update(config =>
            {
                foreach (var request in config.Requests)
                {
                    request.Status = RequestStatus.Skipped;
                    config.History.Insert(0, request);
                }

                config.Requests.Clear();
            });

            return songCount;
        }

        public int ClearHistory()
        {
            int songCount = this.Data.History.Count;
            this.Update(config => config.History.Clear());

            return songCount;
        }

        public bool HasPlayed(string id)
        {
            var lastPlayedTimeoutHours = RequestBotSettings.Current.Data.SessionResetAfterXHours;
            return this.Data.History.Any(request => 
                request.Status == RequestStatus.Played && 
                request.Song.ID.Equals(id, StringComparison.OrdinalIgnoreCase) && 
                request.PlayedTimestamp.AddHours(lastPlayedTimeoutHours) < DateTime.Now
            );
        }
    }
}
