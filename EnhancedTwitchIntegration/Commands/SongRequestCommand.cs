using System.Collections.Generic;
using System.Threading.Tasks;
using SongRequestManager.Config;
using SongRequestManager.Queue;
using SongRequestManager.Utils;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public class SongRequestCommand : Command
    {
        private static readonly SongRequestRuleConfig Config = new SongRequestRuleConfig
        {
            EnforceDuplicate = true,
            EnforceAlreadyPlayed = true,
            EnforceConcurrentByUser = true,
            EnforceMaxLength = true
        };

        public override List<string> Aliases => new List<string> {
            "add",
            "bsr",
            "request",
            "sr",
            "srm"
        };

        public override string HelpText { get; } = "Adds a new song request to the queue - requests must be less than 6 minutes long, and please try to keep it clean! Search for songs on https://beatsaver.com/ then click the twitch icon to copy the !bsr command! (Please pay attention to the rating to find the best map for your request!)";
        public override string SampleUsage { get; } = "!bsr [id]";

        public override async Task<string> ExecuteAsync(ChatCommand command)
        {
            if (!RequestBotSettings.Current.Data.RequestQueueOpen)
            {
                return "Sorry, the queue is closed :(";
            }

            if (command.ArgumentsAsList.Count == 0 || command.ArgumentsAsList.Count > 1)
            {
                return $"Please provide a BeatSaver song ID for your request (something like '!bsr 4e4e')";
            }

            string id = command.ArgumentsAsList[0].ToLower();

            GetSongResult result = await RequestUtils.GetRequestableSongAsync(id, command.ChatMessage.DisplayName, Config);

            if (result.Song != null)
            {
                SongRequest request = new SongRequest(result.Song, command.ChatMessage.DisplayName);
                var queuePosition = RequestQueue.Current.Add(request);

                return StringUtils.GetSongAddedMessage(result.Song, queuePosition);
            }
            else
            {
                return result.Message;
            }
        }
    }
}
