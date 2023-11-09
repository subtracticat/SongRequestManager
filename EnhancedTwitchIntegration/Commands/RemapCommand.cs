using System.Collections.Generic;
using System.Threading.Tasks;
using SongRequestManager.Config;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public class RemapCommand : ModeratorCommand
    {
        public override List<string> Aliases { get; } = new List<string> { "remap" };

        public override Task ExecuteAsync(ChatCommand command)
        {
            if (command.ArgumentsAsList.Count != 2)
            {
                command.Reply("Expected two song IDs: '!remap [fromId] [toId]'");
                return Task.CompletedTask;
            }

            string from = command.ArgumentsAsList[0];
            string to = command.ArgumentsAsList[1];

            if (!CommandUtils.IsBeatSaverId(from) || !CommandUtils.IsBeatSaverId(to))
            {
                command.Reply("I'm confused, you sure those are song IDs? 🤔");
                return Task.CompletedTask;
            }

            RemapConfigManager.Instance.Config[from] = to;
            RemapConfigManager.Instance.Save();

            command.Reply($"Remapped id {from} to {to}");

            return Task.CompletedTask;
        }
    }
}
