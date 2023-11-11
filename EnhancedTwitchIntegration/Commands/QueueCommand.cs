using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SongRequestManager.Queue;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public class QueueCommand : Command
    {
        public override List<string> Aliases { get; } = new List<string> { "queue", "q" };

        public override Task<string> ExecuteAsync(ChatCommand command)
        {
            var message = command.ChatMessage;

            if (message.IsModerator || message.IsBroadcaster)
            {
                var entries = QueueManager.Instance.Config.Requests.Select((request, index) => $"{index + 1}{(request.PriorityValue > 0 ? "!" : string.Empty)}: {request.Song.ID} [{request.RequestedBy}]");

                return Task.FromResult(string.Join(", ", entries));
            }

            var currentRequest = QueueManager.Instance.GetRequestByUsername(message.Username);

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
