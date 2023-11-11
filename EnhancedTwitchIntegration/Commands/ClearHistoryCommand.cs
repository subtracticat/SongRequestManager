using System.Collections.Generic;
using System.Threading.Tasks;
using SongRequestManager.Queue;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public class ClearHistoryCommand : ModeratorCommand
    {
        public override List<string> Aliases { get; } = new List<string> { "clearhistory", "clearalreadyplayed" };

        public override Task<string> ExecuteAsync(ChatCommand command)
        {
            int songCount = QueueManager.Instance.ClearHistory();
            if (songCount > 0)
            {
                return Task.FromResult($"Wiped {songCount} song(s) from history. They're gone. Poof.");
            }
            else
            {
                return Task.FromResult($"Don't you worry, history was empty... and still is.");
            }
        }
    }
}
