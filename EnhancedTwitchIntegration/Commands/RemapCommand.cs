using System.Collections.Generic;
using System.Threading.Tasks;
using SongRequestManager.Config;
using SongRequestManager.Utils;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public class RemapCommand : ModeratorCommand
    {
        public override List<string> Aliases { get; } = new List<string> { "remap" };

        public override Task<string> ExecuteAsync(ChatCommand command)
        {
            if (command.ArgumentsAsList.Count != 2)
            {
                return Task.FromResult("Expected two song IDs: '!remap [fromId] [toId]'");
            }

            string from = command.ArgumentsAsList[0];
            string to = command.ArgumentsAsList[1];

            if (!RequestUtils.IsBeatSaverId(from) || !RequestUtils.IsBeatSaverId(to))
            {
                return Task.FromResult("I'm confused, you sure those are song IDs? 🤔");
            }

            ListConfigManager.Instance.UpdateSettings((config) => config.Remaps[from] = to);

            return Task.FromResult($"Remapped id {from} to {to}");
        }
    }

    public class UnmapCommand : ModeratorCommand
    {
        public override List<string> Aliases { get; } = new List<string> { "unmap" };

        public override Task<string> ExecuteAsync(ChatCommand command)
        {
            if (command.ArgumentsAsList.Count != 1)
            {
                return Task.FromResult("Expected a song ID: '!unmap [id]'");
            }

            string id = command.ArgumentsAsList[0];

            if (!RequestUtils.IsBeatSaverId(id))
            {
                return Task.FromResult("I'm confused, you sure that's a song ID? 🤔");
            }

            if (ListConfigManager.Instance.Config.Remaps.ContainsKey(id))
            {
                ListConfigManager.Instance.UpdateSettings(config => config.Remaps.Remove(id));
                return Task.FromResult($"Unmapped {id}. Be free, {id}!");
            }
            else
            {
                return Task.FromResult($"{id} wasn't mapped to anything anyway, but it's now extra unmapped. 🙃");
            }
        }
    }
}
