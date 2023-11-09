using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SongRequestManager.Config;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
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

            if (RemapConfigManager.Instance.Config.Remove(id))
            {
                RemapConfigManager.Instance.Save();
                command.Reply($"Unmapped {id}. Be free, {id}!");
            }
            else
            {
                command.Reply($"{id} wasn't mapped to anything anyway, but it's now extra unmapped. 🙃");
            }

            return Task.CompletedTask;
        }
    }
}
