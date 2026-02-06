using System.Threading.Tasks;
using SongDetailsCache;
using SongDetailsCache.Structs;

namespace SongRequestManager.SongData
{
    public static class SongDataManager
    {
        private static SongDetails _songDetailCache = null;
        private const string _customLevelPrefix = "custom_level_";

        public static BeatmapLevel CurrentBeatmap { get; set; }
        public static BeatmapLevel PreviousBeatmap { get; set; }

        public static async Task<Song?> GetSongDataAsync(BeatmapLevel map)
        {
            Plugin.Log($"Attempting to find link for level: {map.levelID}");

            _songDetailCache = _songDetailCache ?? await SongDetails.Init();

            if (map.levelID.StartsWith(_customLevelPrefix))
            {
                if (_songDetailCache.songs.FindByHash(map.levelID.Replace(_customLevelPrefix, string.Empty), out Song song))
                {
                    return song;
                }
            }

            return null;
        }
    }
}
