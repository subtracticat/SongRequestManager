using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SongRequestManager.Queue;
using SongRequestManager.Utils;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public class RemoveRequestCommand : Command
    {
        public override List<string> Aliases { get; } = new List<string> { "remove" };

        public override Task<string> ExecuteAsync(ChatCommand command)
        {
            var args = command.ArgumentsAsList;
            var message = command.ChatMessage;

            if (args.Count == 0)
            {
                SongRequest request = QueueManager.Instance.GetRequestByUsername(message.DisplayName);
                if (request != null)
                {
                    QueueManager.Instance.Remove(request.Song.ID, RequestStatus.Deleted);
                    return Task.FromResult($"Request {request.Song.Name} removed");
                }
                else
                {
                    return Task.FromResult($"No requests found for user {message.DisplayName}");
                }
            }
            else if (args.Count == 1)
            {
                string id = args[0];
                if (!RequestUtils.IsBeatSaverId(id))
                {
                    return Task.FromResult($"Expected a song ID: '!remove [id?]'");
                }

                SongRequest request = QueueManager.Instance.GetRequestById(id);
                if (request == null)
                {
                    return Task.FromResult($"No request found for ID {id}");
                }

                if (request.RequestedBy.Equals(message.DisplayName, StringComparison.CurrentCultureIgnoreCase) || message.IsModerator || message.IsBroadcaster)
                {
                    QueueManager.Instance.Remove(id, RequestStatus.Deleted);
                    return Task.FromResult($"Request {request.Song.Name} removed");
                }
                else
                {
                    return Task.FromResult($"Come on now, you can't remove someone else's request!");
                }
            }
            else
            {
                return Task.FromResult($"Too many pieces to this puzzle! '!remove [id?]'");
            }
        }
    }
}
