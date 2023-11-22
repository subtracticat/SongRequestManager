using System.Collections.Generic;
using SongRequestManager.Chat;

namespace SongRequestManager.Commands
{
    public class HelpCommand : Command
    {
        public override List<string> Aliases { get; } = new List<string> { "help" };
        public override string HelpText { get; } = "Describes the behavior of one of the available bot commands.";
        public override string SampleUsage { get; } = "!help [command]";

        protected override string Execute(ChatCommand command)
        {
            var args = command.Arguments;
            if (args.Count != 1)
            {
                return $"What can I help you with? '{this.SampleUsage}'";
            }

            if (ChatHandler.TryGetCommand(args[0], out Command targetCommand))
            {
                if (!targetCommand.IsModOnly || command.IsModerator)
                {
                    var modPrefix = targetCommand.IsModOnly ? "[Mod Only] " : string.Empty;
                    return $"{modPrefix}{targetCommand.HelpText} - Usage: \"{targetCommand.SampleUsage}\"";
                }
            }

            return string.Empty;
        }
    }
}
