using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SongRequestManager.Config;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public class SetQueueStatusCommand : ModeratorCommand
    {
        public override List<string> Aliases => new List<string> { "close", "open" };

        public override Task ExecuteAsync(ChatCommand command)
        {
            bool targetState = command.CommandText.Equals("open", StringComparison.CurrentCultureIgnoreCase);

            QueueConfigManager.Instance.Config.RequestQueueOpen = targetState;
            QueueConfigManager.Instance.Save();

            command.Reply($"The queue is now {(targetState ? "open" : "closed")}!");
            return Task.CompletedTask;
        }
    }
}
