using SongRequestManager.Queue;

namespace SongRequestManager.Utils
{
    public static class StringUtils
    {
        public static string GetSongAddedMessage(Song song, QueuePosition position)
        {
            return $"{song.Name} [{song.Metadata.LevelAuthorName}] {(song.Stats.Score > 0 ? $"({(int)(song.Stats.Score * 100)}%)" : "")} added at position #{position.Position}!";
        }

        public static string GetDurationString(int seconds)
        {
            return $"{seconds / 60}:{(seconds % 60):00}";
        }
    }
}
