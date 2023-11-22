using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SongRequestManager.Queue;
using SongRequestManager.Chat;

namespace SongRequestManager.Commands
{
    public class QueueCommand : Command
    {
        public override List<string> Aliases { get; } = new List<string> { "queue", "q" };
        public override string HelpText { get; } = "Context-dependent command to view status of the request queue. If the user has a request currently, its status is provided, otherwise the overall queue status is returned.";
        public override string SampleUsage { get; } = "!queue";

        public override Task<string> ExecuteAsync(ChatCommand command)
        {
            if (command.IsModerator)
            {
                var queue = RequestQueue.Current.Data.Requests;
                if (queue.Count == 0)
                {
                    return Task.FromResult("The queue is empty!");
                }

                var entries = queue.Select((request, index) => $"{index + 1}{(request.PriorityValue > 0 ? "*" : string.Empty)}:[{request.RequestedBy}:{request.Song.ID}]");
                return Task.FromResult(string.Join(", ", entries));
            }

            var currentRequest = RequestQueue.Current.GetRequestByUsername(command.Username);

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
