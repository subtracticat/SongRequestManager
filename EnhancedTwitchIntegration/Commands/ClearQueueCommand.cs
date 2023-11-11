using System.Collections.Generic;
using SongRequestManager.Queue;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public class ClearQueueCommand : ModeratorCommand
    {
        public override List<string> Aliases { get; } = new List<string> { "clearqueue" };
        public override string HelpText { get; } = "Clear all requests from the current requests queue.";
        public override string SampleUsage { get; } = "!clearqueue";

        protected override string Execute(ChatCommand command)
        {
            int songCount = QueueManager.Instance.ClearQueue();
            if (songCount > 0)
            {
                return $"Cleared {songCount} song(s).";
            }
            else
            {
                return $"Don't you worry, queue was empty... and still is.";
            }
        }
    }
}
