using System.Collections.Generic;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public class SabotageCommand : ModeratorCommand
    {
        public static bool? IsStreamerkitInstalled = null;

        public override List<string> Aliases { get; } = new List<string> { "sabotage" };
        public override string HelpText { get; } = "Sets the status of the !bomb command, enabling or disabling chat bombs.";
        public override string SampleUsage { get; } = "!sabotage [on/off]";

        protected override string Execute(ChatCommand command)
        {
            if (IsStreamerkitInstalled == null)
            {
                IsStreamerkitInstalled = IPA.Loader.PluginManager.GetPlugin("Beat Bits") != null;
            }

            if (IsStreamerkitInstalled == true)
            {
                if (command.ArgumentsAsList.Count != 1)
                {
                    return "Expected 1 argument: '!sabotage [on/off]'";
                }

                switch (command.ArgumentsAsList[0].ToLower())
                {
                    case "on":
                        this.SetSabotage(true);
                        return "Sabotage enabled!";

                    case "off":
                        this.SetSabotage(false);
                        return "Sabotage disabled!";

                    default:
                        return "🤔 Could I interest you in an 'on' or an 'off' instead?";
                }
            }

            Plugin.Log("Sabotage toggle attempted, but StreamerKit not detected");
            return string.Empty;
        }

        private void SetSabotage(bool enabled)
        {
            System.Diagnostics.Process.Start($"liv-streamerkit://gamechanger/beat-saber-sabotage/{(enabled ? "enable" : "disable")}");
        }
    }
}
