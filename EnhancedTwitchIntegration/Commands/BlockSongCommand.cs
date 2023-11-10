using System.Collections.Generic;
using System.Threading.Tasks;
using SongRequestManager.Config;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public class BlockSongCommand : ModeratorCommand
    {
        public override List<string> Aliases { get; } = new List<string> { "block" };

        public override Task<string> ExecuteAsync(ChatCommand command)
        {
            if (command.ArgumentsAsList.Count != 1)
            {
                return Task.FromResult("Expected a song ID: '!block [id]'");
            }

            string id = command.ArgumentsAsList[0];

            if (!CommandUtils.IsBeatSaverId(id))
            {
                return Task.FromResult("I'm confused, you sure that's a song ID? 🤔");
            }

            if (ListConfigManager.Instance.Config.Bans.Contains(id))
            {
                return Task.FromResult($"ID {id} is already blocked!");
            }
            else
            {
                ListConfigManager.Instance.UpdateSettings(config => config.Bans.Add(id));
                return Task.FromResult($"ID {id} blocked!");
            }
        }
    }

    public class UnblockSongCommand : ModeratorCommand
    {
        public override List<string> Aliases { get; } = new List<string> { "unblock" };

        public override Task<string> ExecuteAsync(ChatCommand command)
        {
            if (command.ArgumentsAsList.Count != 1)
            {
                return Task.FromResult("Expected a song ID: '!unblock [id]'");
            }

            string id = command.ArgumentsAsList[0];

            if (!CommandUtils.IsBeatSaverId(id))
            {
                return Task.FromResult("I'm confused, you sure that's a song ID? 🤔");
            }

            if (ListConfigManager.Instance.Config.Bans.Contains(id))
            {
                ListConfigManager.Instance.UpdateSettings(config => config.Bans.Remove(id));
                return Task.FromResult($"ID {id} unblocked!");
            }
            else
            {
                return Task.FromResult($"ID {id} wasn't blocked? So uh.. we're good? ¯\\_(ツ)_/¯");
            }
        }
    }
}
