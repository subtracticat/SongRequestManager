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

        public override async Task<string> ExecuteAsync(ChatCommand command)
        {
            if (!QueueConfigManager.Instance.Config.RequestQueueOpen)
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
                var queuePosition = QueueManager.Instance.Add(request);

                return StringUtils.GetSongAddedMessage(result.Song, queuePosition);
            }
            else
            {
                return result.Message;
            }
        }
    }
}
