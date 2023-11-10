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

        public async Task<Song> GetSongDataAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            string requestUrl = $"https://api.beatsaver.com/maps/id/{id}";

            var response = await Plugin.WebClient.GetAsync(requestUrl, System.Threading.CancellationToken.None);

            if (response.IsSuccessStatusCode)
            {
                string content = response.ContentToString();
                Song song = JsonConvert.DeserializeObject<Song>(content);

                return song;
            }
            else
            {
                Plugin.Log($"Failed to retrieve song info for id [{id}]: {response.StatusCode}");
                return null;
            }
        }
    }
}
