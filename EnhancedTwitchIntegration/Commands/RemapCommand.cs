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

            ListConfigManager.Instance.Update((config) =>
            {
                config.Remaps[from] = to;
            });

            command.Reply($"Remapped id {from} to {to}");

            return Task.CompletedTask;
        }
    }

    public class UnmapCommand : ModeratorCommand
    {
        public override List<string> Aliases { get; } = new List<string> { "unmap" };

        public override Task ExecuteAsync(ChatCommand command)
        {
            if (command.ArgumentsAsList.Count != 1)
            {
                command.Reply("Expected a song ID: '!unmap [id]'");
                return Task.CompletedTask;
            }

            string id = command.ArgumentsAsList[0];

            if (!CommandUtils.IsBeatSaverId(id))
            {
                command.Reply("I'm confused, you sure that's a song ID? 🤔");
                return Task.CompletedTask;
            }

            if (ListConfigManager.Instance.Config.Remaps.ContainsKey(id))
            {
                ListConfigManager.Instance.Update(config =>
                {
                    config.Remaps.Remove(id);
                });
                command.Reply($"Unmapped {id}. Be free, {id}!");
                return Task.CompletedTask;
            }
            else
            {
                command.Reply($"{id} wasn't mapped to anything anyway, but it's now extra unmapped. 🙃");
                return Task.CompletedTask;
            }
        }
    }
}
