using System.Collections.Generic;
using SongRequestManager.Config;
using SongRequestManager.Utils;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public class BlockSongCommand : ModeratorCommand
    {
        public override List<string> Aliases { get; } = new List<string> { "block" };
        public override string HelpText { get; } = "Block a song, preventing it from being requested again in the future.";
        public override string SampleUsage { get; } = "!block [id]";

        protected override string Execute(ChatCommand command)
        {
            if (command.ArgumentsAsList.Count != 1)
            {
                return "Expected a song ID: '!block [id]'";
            }

            string id = command.ArgumentsAsList[0];

            if (!RequestUtils.IsBeatSaverId(id))
            {
                return "I'm confused, you sure that's a song ID? 🤔";
            }

            if (ListConfigManager.Instance.Config.Bans.Contains(id))
            {
                return $"ID {id} is already blocked!";
            }
            else
            {
                ListConfigManager.Instance.UpdateSettings(config => config.Bans.Add(id));
                return $"ID {id} blocked!";
            }
        }
    }

    public class UnblockSongCommand : ModeratorCommand
    {
        public override List<string> Aliases { get; } = new List<string> { "unblock" };
        public override string HelpText { get; } = "Unblock a song, allowing it to be requested again in the future.";
        public override string SampleUsage { get; } = "!unblock [id]";

        protected override string Execute(ChatCommand command)
        {
            if (command.ArgumentsAsList.Count != 1)
            {
                return "Expected a song ID: '!unblock [id]'";
            }

            string id = command.ArgumentsAsList[0];

            if (!RequestUtils.IsBeatSaverId(id))
            {
                return "I'm confused, you sure that's a song ID? 🤔";
            }

            if (ListConfigManager.Instance.Config.Bans.Contains(id))
            {
                ListConfigManager.Instance.UpdateSettings(config => config.Bans.Remove(id));
                return $"ID {id} unblocked!";
            }
            else
            {
                return $"ID {id} wasn't blocked? So uh.. we're good? ¯\\_(ツ)_/¯";
            }
        }
    }
}
