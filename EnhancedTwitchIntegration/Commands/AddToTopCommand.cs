using System.Collections.Generic;
using System.Threading.Tasks;
using SongRequestManager.Queue;
using SongRequestManager.Utils;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public class AddToTopCommand : ModeratorCommand
    {
        private static readonly SongRequestRuleConfig Config = new SongRequestRuleConfig
        {
            EnforceDuplicate = false,
            EnforceAlreadyPlayed = false,
            EnforceConcurrentByUser = false,
            EnforceMaxLength = false
        };

        public override List<string> Aliases { get; } = new List<string> { "att", "attfor" };

        public override async Task<string> ExecuteAsync(ChatCommand command)
        {
            var args = command.ArgumentsAsList;

            if (args.Count == 0 || args.Count > 2)
            {
                return $"Hmmmm... Try that again? '!att [id] [username?]'";
            }

            string id = args[0].ToLower();
            string username = args.Count == 1 ? command.ChatMessage.Username : args[1];

            GetSongResult result = await RequestUtils.GetRequestableSongAsync(id, username, Config);

            if (result.Song != null)
            {
                SongRequest request = new SongRequest(result.Song, username);
                var queuePosition = QueueManager.Instance.InsertAt(0, request);

                return StringUtils.GetSongAddedMessage(result.Song, queuePosition);
            }
            else
            {
                return result.Message;
            }
        }
    }
}
