using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SongRequestManager.Queue;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public class QueueCommand : Command
    {
        public override List<string> Aliases { get; } = new List<string> { "queue", "q" };
        public override string HelpText { get; } = "Context-dependent command to view status of the request queue. If the user has a request currently, its status is provided, otherwise the overall queue status is returned.";
        public override string SampleUsage { get; } = "!queue";

        public override Task<string> ExecuteAsync(ChatCommand command)
        {
            var message = command.ChatMessage;

            if (message.IsModerator || message.IsBroadcaster)
            {
                var queue = QueueManager.Instance.Config.Requests;
                if (queue.Count == 0)
                {
                    return Task.FromResult("The queue is empty!");
                }

                var entries = queue.Select((request, index) => $"{index + 1}{(request.PriorityValue > 0 ? "!" : string.Empty)}: {request.Song.ID} [{request.RequestedBy}]");
                return Task.FromResult(string.Join(", ", entries));
            }

            var currentRequest = QueueManager.Instance.GetRequestByUsername(message.DisplayName);

            if (currentRequest != null)
            {
                return new MyRequestCommand().ExecuteAsync(command);
            }
            else
            {
                return new QueueStatusCommand().ExecuteAsync(command);
            }
        }
    }
}
