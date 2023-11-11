using System.Collections.Generic;
using System.Threading.Tasks;
using SongRequestManager.Queue;
using SongRequestManager.Utils;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public class WhoCommand : ModeratorCommand
    {
        public override List<string> Aliases { get; } = new List<string> { "who" };

        public override Task<string> ExecuteAsync(ChatCommand command)
        {
            if (command.ArgumentsAsList.Count != 1)
            {
                return Task.FromResult("What would you like to know? '!who [id/username]`");
            }

            string arg = command.ArgumentsAsList[0];

            if (RequestUtils.IsBeatSaverId(arg))
            {
                var request = QueueManager.Instance.GetRequestById(arg);

                if (request != null)
                {
                    var position = QueueManager.Instance.GetPositionOf(request);
                    return Task.FromResult($"{request.RequestedBy}: {request.Song.Name} ({request.Song.ID}) at position {position.Position}");
                }

                return Task.FromResult($"Couldn't find a request for ID {arg}");
            }
            else
            {
                var request = QueueManager.Instance.GetRequestByUsername(arg.Replace("@", string.Empty));

                if (request != null)
                {
                    var position = QueueManager.Instance.GetPositionOf(request);
                    return Task.FromResult($"{request.RequestedBy}: {request.Song.Name} ({request.Song.ID}) at position {position.Position}");
                }

                return Task.FromResult($"Couldn't find a request for user ${command.ChatMessage.Username}");
            }
        }
    }
}
