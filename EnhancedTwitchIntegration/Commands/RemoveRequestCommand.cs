using System;
using System.Collections.Generic;
using SongRequestManager.Queue;
using SongRequestManager.Utils;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public class RemoveRequestCommand : Command
    {
        public override List<string> Aliases { get; } = new List<string> { "remove" };
        public override string HelpText { get; } = "Removes a request from the queue, optionally providing an ID to indicate a specific request. (Note that only Moderators are able to remove requests other than their own).";
        public override string SampleUsage { get; } = "!remove [id?]";

        protected override string Execute(ChatCommand command)
        {
            var args = command.ArgumentsAsList;
            var message = command.ChatMessage;

            if (args.Count == 0)
            {
                SongRequest request = QueueManager.Instance.GetRequestByUsername(message.DisplayName);
                if (request != null)
                {
                    QueueManager.Instance.Remove(request.Song.ID, RequestStatus.Deleted);
                    return $"Request {request.Song.Name} removed";
                }
                else
                {
                    return $"No requests found for user {message.DisplayName}";
                }
            }
            else if (args.Count == 1)
            {
                string id = args[0];
                if (!RequestUtils.IsBeatSaverId(id))
                {
                    return $"Expected a song ID: '!remove [id?]'";
                }

                SongRequest request = QueueManager.Instance.GetRequestById(id);
                if (request == null)
                {
                    return $"No request found for ID {id}";
                }

                if (request.RequestedBy.Equals(message.DisplayName, StringComparison.CurrentCultureIgnoreCase) || message.IsModerator || message.IsBroadcaster)
                {
                    QueueManager.Instance.Remove(id, RequestStatus.Deleted);
                    return $"Request {request.Song.Name} removed";
                }
                else
                {
                    return $"Come on now, you can't remove someone else's request!";
                }
            }
            else
            {
                return $"Too many pieces to this puzzle! '!remove [id?]'";
            }
        }
    }
}
