using System;
using System.Collections.Generic;
using SongRequestManager.Chat;
using SongRequestManager.Queue;
using SongRequestManager.Utils;

namespace SongRequestManager.Commands
{
    public class RemoveRequestCommand : Command
    {
        public override List<string> Aliases { get; } = new List<string> { "remove" };
        public override string HelpText { get; } = "Removes a request from the queue, optionally providing an ID to indicate a specific request. (Note that only Moderators are able to remove requests other than their own).";
        public override string SampleUsage { get; } = "!remove [id?]";

        protected override string Execute(ChatCommand command)
        {
            var args = command.Arguments;

            if (args.Count == 0)
            {
                SongRequest request = RequestQueue.Current.GetRequestByUsername(command.Username);
                if (request != null)
                {
                    RequestQueue.Current.Remove(request.Song.ID, RequestStatus.Deleted);
                    if (request.PriorityValue > 0)
                    {
                        // Refund any prio points
                        Plugin.Log($"Refunding ${request.PriorityValue} to {request.RequestedBy}");
                        PriorityTracker.RegisterPriorityEvent(request.RequestedBy, new PriorityEvent
                        {
                            Type = PriorityEventType.Unknown,
                            Value = request.PriorityValue,
                            Timestamp = DateTime.Now
                        });
                    }
                    return $"Request {request.Song.Name} removed";
                }
                else
                {
                    return $"No requests found for user {command.Username}";
                }
            }
            else if (args.Count == 1)
            {
                string id = args[0];
                if (!RequestUtils.IsBeatSaverId(id))
                {
                    return $"Expected a song ID: '!remove [id?]'";
                }

                SongRequest request = RequestQueue.Current.GetRequestById(id);
                if (request == null)
                {
                    return $"No request found for ID {id}";
                }

                if (request.RequestedBy.Equals(command.Username, StringComparison.CurrentCultureIgnoreCase) || command.IsModerator)
                {
                    RequestQueue.Current.Remove(id, RequestStatus.Deleted);
                    if (request.PriorityValue > 0)
                    {
                        // Refund any prio points
                        Plugin.Log($"Refunding ${request.PriorityValue} to {request.RequestedBy}");
                        PriorityTracker.RegisterPriorityEvent(request.RequestedBy, new PriorityEvent
                        {
                            Type = PriorityEventType.Unknown,
                            Value = request.PriorityValue,
                            Timestamp = DateTime.Now
                        });
                    }
                    return $"Request {request.Song.Name} ({request.Song.ID}) removed";
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
