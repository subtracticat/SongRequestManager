using System.Collections.Generic;
using SongRequestManager.Config;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public class ToggleAutoPrioCommand : ModeratorCommand
    {
        public override List<string> Aliases { get; } = new List<string> { "autoprio" };
        public override string HelpText { get; } = "Enable or disable the bot's automatic priority system.";
        public override string SampleUsage { get; } = "!autoprio [on/off]";

        protected override string Execute(ChatCommand command)
        {
            if (command.ArgumentsAsList.Count != 1)
            {
                return "Expected 1 argument: '!autoprio [on/off]'";
            }

            switch (command.ArgumentsAsList[0].ToLower())
            {
                case "on":
                    RequestBotSettings.Current.Update(data => data.EnableAutoPrio = true);
                    return "Auto-prio system enabled!";

                case "off":
                    RequestBotSettings.Current.Update(data => data.EnableAutoPrio = false);
                    return "Auto-prio system disabled.";

                default:
                    return "🤔 Could I interest you in an 'on' or an 'off' instead?";
            }
        }
    }
}
