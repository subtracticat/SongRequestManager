using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public class SabotageCommand : ModeratorCommand
    {
        public static bool? IsStreamerkitInstalled = null;

        public override List<string> Aliases { get; } = new List<string> { "sabotage" };

        public override Task<string> ExecuteAsync(ChatCommand command)
        {
            if (IsStreamerkitInstalled == null)
            {
                IsStreamerkitInstalled = IPA.Loader.PluginManager.GetPlugin("Beat Bits") != null;
            }

            if (IsStreamerkitInstalled == true)
            {
                if (command.ArgumentsAsList.Count != 1)
                {
                    return Task.FromResult("Expected 1 argument: '!sabotage [on/off]'");
                }

                switch (command.ArgumentsAsList[0].ToLower())
                {
                    case "on":
                        this.SetSabotage(true);
                        return Task.FromResult("Sabotage enabled!");

                    case "off":
                        this.SetSabotage(false);
                        return Task.FromResult("Sabotage disabled!");

                    default:
                        return Task.FromResult("🤔 Could I interest you in an 'on' or an 'off' instead?");
                }                
            }

            Plugin.Log("Sabotage toggle attempted, but StreamerKit not detected");
            return Task.FromResult(string.Empty);
        }

        private void SetSabotage(bool enabled)
        {
            System.Diagnostics.Process.Start($"liv-streamerkit://gamechanger/beat-saber-sabotage/{(enabled ? "enable" : "disable")}");
        }
    }
}
