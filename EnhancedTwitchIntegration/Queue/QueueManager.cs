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

    public class QueueManager : ConfigBase<QueueData>
    {
        protected override string FilePath => Path.Combine(Plugin.DataPath, "SRMQueueData.json");

        private static QueueManager instance;

        public static QueueManager Instance
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

        public override void UpdateSettings(Action<QueueData> updateFunc)
        {
            base.UpdateSettings(updateFunc);

            if (RequestBotListViewController.Instance.isActivated)
            {
                RequestBotListViewController.Instance.UpdateRequestUI(true);
                RequestBotListViewController.Instance.SetUIInteractivity();
            }
        }

        public SongRequest GetRequestById(string id)
        {
            return this.Config.Requests.FirstOrDefault(request => request.Song.ID.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        public SongRequest GetRequestByUsername(string username)
        {
            return this.Config.Requests.FirstOrDefault(request => request.RequestedBy.Equals(username, StringComparison.CurrentCultureIgnoreCase));
        }

        public QueuePosition GetPositionOf(SongRequest request)
        {
            int index = this.Config.Requests.IndexOf(request);
            if (index >= 0)
            {
                return new QueuePosition
                {
                    Position = index + 1,
                    DurationAheadSeconds = (int)this.Config.Requests.GetRange(0, index).Sum(r => r.Song.Metadata.Duration)
                };
            }

            return null;
        }

        public QueuePosition Add(SongRequest request)
        {
            int previousCount = this.Config.Requests.Count;
            double previousDuration = this.Config.Requests.Sum(r => r.Song.Metadata.Duration);

            request.Status = RequestStatus.Queued;
            this.UpdateSettings(config => config.Requests.Add(request));

            return new QueuePosition
            {
                Position = previousCount + 1,
                DurationAheadSeconds = (int)previousDuration
            };
        }

        public void Add(IEnumerable<SongRequest> requests)
        {
            this.UpdateSettings(config => config.Requests.AddRange(requests));
        }

        public QueuePosition InsertAt(int index, SongRequest request)
        {
            this.UpdateSettings(config => config.Requests.Insert(index, request));

            return new QueuePosition
            {
                Position = index + 1,
                DurationAheadSeconds = (int)this.Config.Requests.GetRange(0, index).Sum(r => r.Song.Metadata.Duration)
            };
        }

        public QueuePosition ReplaceSong(SongRequest request, Song newSong)
        {
            int index = this.Config.Requests.IndexOf(request);
            if (index >= 0)
            {
                this.UpdateSettings(config => config.Requests[index].Song = newSong);

                return new QueuePosition
                {
                    Position = index + 1,
                    DurationAheadSeconds = (int)this.Config.Requests.GetRange(0, index).Sum(r => r.Song.Metadata.Duration)
                };
            }

            return null;
        }

        public SongRequest Remove(string id, RequestStatus newStatus = RequestStatus.Deleted)
        {
            SongRequest request = null;

            int index = this.Config.Requests.FindIndex(item => item.Song.ID.Equals(id, StringComparison.OrdinalIgnoreCase));
            if (index >= 0)
            {
                request = this.Config.Requests[index];
                this.UpdateSettings(config =>
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
            int songCount = this.Config.Requests.Count;

            this.UpdateSettings(config =>
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
            int songCount = this.Config.History.Count;
            this.UpdateSettings(config => config.History.Clear());

            return songCount;
        }

        public bool HasPlayed(string id)
        {
            var lastPlayedTimeoutHours = QueueConfigManager.Instance.Config.SessionResetAfterXHours;
            return this.Config.History.Any(request => 
                request.Status == RequestStatus.Played && 
                request.Song.ID.Equals(id, StringComparison.OrdinalIgnoreCase) && 
                request.PlayedTimestamp.AddHours(lastPlayedTimeoutHours) < DateTime.Now
            );
        }
    }
}
