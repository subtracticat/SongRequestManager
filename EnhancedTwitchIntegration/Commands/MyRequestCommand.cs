using System.Collections.Generic;
using System.Threading.Tasks;
using SongRequestManager.Queue;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public class MyRequestCommand : Command
    {
        public override List<string> Aliases { get; } = new List<string> { "my", "myqueue", "myrequest", "me" };

        public override Task<string> ExecuteAsync(ChatCommand command)
        {
            var username = command.ChatMessage.Username;
            var request = QueueManager.Instance.GetRequestByUsername(username);

            if (request == null)
            {
                return Task.FromResult($"Couldn't find a request for user @{username} 🤔");
            }

            var position = QueueManager.Instance.GetPositionOf(request);
            string messageRoot = $"Request {request.Song.Name} is in position #{position.Position}";

            if (position.Position == 1)
            {
                return Task.FromResult(messageRoot);
            }
            else
            {
                return Task.FromResult($"{messageRoot} behind {CommandUtils.GetDurationString(position.DurationAheadSeconds)} of requests");
            }
        }
    }
}
