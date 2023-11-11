using System.Collections.Generic;
using System.Threading.Tasks;
using SongRequestManager.Queue;
using SongRequestManager.Utils;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public class ModAddCommand : ModeratorCommand
    {
        private static readonly SongRequestRuleConfig Config = new SongRequestRuleConfig
        {
            EnforceDuplicate = false,
            EnforceAlreadyPlayed = false,
            EnforceConcurrentByUser = false,
            EnforceMaxLength = false
        };

        public override List<string> Aliases { get; } = new List<string> { "modadd", "modaddfor" };

        public override async Task<string> ExecuteAsync(ChatCommand command)
        {
            var args = command.ArgumentsAsList;

            if (args.Count == 0 || args.Count > 2)
            {
                return $"Hmmmm... Try that again? '!modadd [id] [username?]'";
            }

            string id = args[0].ToLower();
            string username = args.Count == 1 ? command.ChatMessage.DisplayName : args[1];

            GetSongResult result = await RequestUtils.GetRequestableSongAsync(id, username, Config);

            if (result.Song != null)
            {
                SongRequest request = new SongRequest(result.Song, username);
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
