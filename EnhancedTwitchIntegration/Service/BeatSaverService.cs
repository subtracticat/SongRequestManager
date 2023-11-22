using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SongRequestManager.Queue;

namespace SongRequestManager.Service
{
    public class BeatSaverService
    {
        private static BeatSaverService instance;
        public static BeatSaverService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BeatSaverService();
                }

                return instance;
            }
        }

        public delegate Task<string> FetchHandler(string songId);

        private FetchHandler fetchOverride;

        public async Task<Song> GetSongDataAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }

            string content = await (this.fetchOverride ?? this.FetchSongMetadataAsync)(id);

            if (string.IsNullOrEmpty(content))
            {
                throw new Exception($"No song content found for song {id}");
            }

            return JsonConvert.DeserializeObject<Song>(content);
        }

        private async Task<string> FetchSongMetadataAsync(string id)
        {
            string requestUrl = $"https://api.beatsaver.com/maps/id/{id}";

            var response = await Plugin.WebClient.GetAsync(requestUrl, CancellationToken.None);

            if (response.IsSuccessStatusCode)
            {
                return response.ContentToString();
            }
            else
            {
                Plugin.Log($"Failed to retrieve song info for id [{id}]: {response.StatusCode}");
                return null;
            }
        }

        public void SetFetchOverride(FetchHandler fetchHandler)
        {
            this.fetchOverride = fetchHandler;
        }
    }
}
