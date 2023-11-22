using System.Collections.Generic;
using SongRequestManager.Chat;
using SongRequestManager.Config;
using SongRequestManager.Utils;

namespace SongRequestManager.Commands
{
    public class RemapCommand : ModeratorCommand
    {
        public override List<string> Aliases { get; } = new List<string> { "remap" };
        public override string HelpText { get; } = "Creates a redirect from one ID to another for future requests, such that any requests for one ID resolve as if the second ID were given instead.";
        public override string SampleUsage { get; } = "!remap [fromId] [toId]";

        protected override string Execute(ChatCommand command)
        {
            var args = command.Arguments;

            if (args.Count != 2)
            {
                return "Expected two song IDs: '!remap [fromId] [toId]'";
            }

            string from = args[0];
            string to = args[1];

            if (!RequestUtils.IsBeatSaverId(from) || !RequestUtils.IsBeatSaverId(to))
            {
                return "I'm confused, you sure those are song IDs? 🤔";
            }

            SongModerationSettings.Current.Update((config) => config.Remaps[from] = to);

            return $"Remapped id {from} to {to}";
        }
    }

    public class UnmapCommand : ModeratorCommand
    {
        public override List<string> Aliases { get; } = new List<string> { "unmap" };
        public override string HelpText { get; } = "Removes the redirect for an ID, allowing it to be requested without redirecting users to a different map.";
        public override string SampleUsage { get; } = "!unmap [id]";

        protected override string Execute(ChatCommand command)
        {
            var args = command.Arguments;
            if (args.Count != 1)
            {
                return "Expected a song ID: '!unmap [id]'";
            }

            string id = args[0];

            if (!RequestUtils.IsBeatSaverId(id))
            {
                return "I'm confused, you sure that's a song ID? 🤔";
            }

            if (SongModerationSettings.Current.Data.Remaps.ContainsKey(id))
            {
                SongModerationSettings.Current.Update(config => config.Remaps.Remove(id));
                return $"Unmapped {id}. Be free, {id}!";
            }
            else
            {
                return $"{id} wasn't mapped to anything anyway, but it's now extra unmapped. 🙃";
            }
        }
    }
}
