using System.Collections.Generic;
using SongRequestManager.Config;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public class OpenQueueCommand : ModeratorCommand
    {
        public override List<string> Aliases => new List<string> { "open" };
        public override string HelpText { get; } = "Opens the request queue, allowing users to submit requests.";
        public override string SampleUsage { get; } = "!open";

        protected override string Execute(ChatCommand command)
        {
            QueueConfigManager.Instance.UpdateSettings(config => config.RequestQueueOpen = true);
            return "The queue is now open!";
        }
    }

    public class CloseQueueCommand : ModeratorCommand
    {
        public override List<string> Aliases => new List<string> { "close" };
        public override string HelpText { get; } = "Closes the request queue, preventing the addition of new requests.";
        public override string SampleUsage { get; } = "!close";

        protected override string Execute(ChatCommand command)
        {
            QueueConfigManager.Instance.UpdateSettings(config => config.RequestQueueOpen = false);
            return "The queue is now closed.";
        }
    }
}
