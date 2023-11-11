using System.Collections.Generic;
using System.Threading.Tasks;
using SongRequestManager.Queue;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public class ClearQueueCommand : ModeratorCommand
    {
        public override List<string> Aliases { get; } = new List<string> { "clearqueue" };

        public override Task<string> ExecuteAsync(ChatCommand command)
        {
            int songCount = QueueManager.Instance.ClearQueue();
            if (songCount > 0)
            {
                return Task.FromResult($"Cleared {songCount} song(s).");
            }
            else
            {
                return Task.FromResult($"Don't you worry, queue was empty... and still is.");
            }
        }
    }
}
