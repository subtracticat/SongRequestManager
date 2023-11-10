using System.Collections.Generic;
using System.Threading.Tasks;
using SongRequestManager.Config;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public class BlockSongCommand : ModeratorCommand
    {
        public override List<string> Aliases { get; } = new List<string> { "block" };

        public override Task ExecuteAsync(ChatCommand command)
        {
            if (command.ArgumentsAsList.Count != 1)
            {
                command.Reply("Expected a song ID: '!block [id]'");
                return Task.CompletedTask;
            }

            string id = command.ArgumentsAsList[0];

            if (!CommandUtils.IsBeatSaverId(id))
            {
                command.Reply("I'm confused, you sure that's a song ID? 🤔");
                return Task.CompletedTask;
            }

            if (ListConfigManager.Instance.Config.Bans.Contains(id))
            {
                command.Reply($"ID {id} is already blocked!");
            }
            else
            {
                ListConfigManager.Instance.Update(config =>
                {
                    config.Bans.Add(id);
                });
                command.Reply($"ID {id} blocked!");
            }

            return Task.CompletedTask;
        }
    }

    public class UnblockSongCommand : ModeratorCommand
    {
        public override List<string> Aliases { get; } = new List<string> { "unblock" };

        public override Task ExecuteAsync(ChatCommand command)
        {
            if (command.ArgumentsAsList.Count != 1)
            {
                command.Reply("Expected a song ID: '!unblock [id]'");
                return Task.CompletedTask;
            }

            string id = command.ArgumentsAsList[0];

            if (!CommandUtils.IsBeatSaverId(id))
            {
                command.Reply("I'm confused, you sure that's a song ID? 🤔");
                return Task.CompletedTask;
            }

            if (ListConfigManager.Instance.Config.Bans.Contains(id))
            {
                ListConfigManager.Instance.Update(config =>
                {
                    config.Bans.Remove(id);
                });
                command.Reply($"ID {id} unblocked!");
            }
            else
            {
                command.Reply($"ID {id} wasn't blocked? So uh.. we're good? ¯\\_(ツ)_/¯");
            }

            return Task.CompletedTask;
        }
    }
}
