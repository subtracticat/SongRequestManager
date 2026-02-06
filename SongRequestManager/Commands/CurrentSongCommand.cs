using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SongRequestManager.Chat;
using SongRequestManager.Queue;
using SongRequestManager.SongData;

namespace SongRequestManager.Commands
{
    public abstract class GetSongCommandBase : Command
    {
        protected string GetRecentRequester(string songId)
        {
            return RequestQueue.Current.Data.History.Take(2).FirstOrDefault(req => req.Song.ID.Equals(songId, System.StringComparison.OrdinalIgnoreCase))?.RequestedBy;
        }

        protected async Task<string> ProcessAsync(BeatmapLevel song, string responsePrefix)
        {
            var bsrData = await SongDataManager.GetSongDataAsync(song);

            if (bsrData.HasValue)
            {
                var songDetails = $"{song.songName} - {song.songAuthorName}";
                var requestedBy = GetRecentRequester(bsrData.Value.key);
                var requesterDetails = string.IsNullOrEmpty(requestedBy) ? string.Empty : $", requested by @{requestedBy}";

                return $"{responsePrefix}: {songDetails}{requesterDetails}: https://beatsaver.com/maps/{bsrData.Value.key}";
            }

            return $"Couldn't find a link for song {song.songName} - {song.songAuthorName}";
        }
    }

    public class CurrentSongCommand : GetSongCommandBase
    {
        public override List<string> Aliases { get; } = new List<string> { "link", "current", "song" };
        public override string HelpText { get; } = "Gets the link for the currently playing song (or the most recently played, if no song is in progress).";
        public override string SampleUsage { get; } = "!song";

        public override Task<string> ExecuteAsync(ChatCommand command)
        {
            var song = SongDataManager.CurrentBeatmap;
            if (song != null)
            {
                return this.ProcessAsync(song, "Currently playing");
            }

            return new PreviousSongCommand().ExecuteAsync(command);
        }
    }

    public class PreviousSongCommand : GetSongCommandBase
    {
        public override List<string> Aliases { get; } = new List<string> { "last", "previous" };
        public override string HelpText { get; } = "Gets the link for the most recently completed song.";
        public override string SampleUsage { get; } = "!last";

        public override Task<string> ExecuteAsync(ChatCommand command)
        {
            var song = SongDataManager.PreviousBeatmap;
            if (song != null)
            {
                return this.ProcessAsync(song, "Last played");
            }

            return Task.FromResult("Couldn't find any songs to tell you about 🤔");
        }
    }
}
