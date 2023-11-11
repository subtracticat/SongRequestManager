using System.Collections.Generic;
using SongRequestManager.Queue;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public class ClearHistoryCommand : ModeratorCommand
    {
        public override List<string> Aliases { get; } = new List<string> { "clearhistory", "clearalreadyplayed" };
        public override string HelpText { get; } = "Clear all played/skipped songs from the previous request history.";
        public override string SampleUsage { get; } = "!clearhistory";

        protected override string Execute(ChatCommand command)
        {
            int songCount = RequestQueue.Current.ClearHistory();
            if (songCount > 0)
            {
                return $"Wiped {songCount} song(s) from history. They're gone. Poof.";
            }
            else
            {
                return $"Don't you worry, history was empty... and still is.";
            }
        }
    }
}
