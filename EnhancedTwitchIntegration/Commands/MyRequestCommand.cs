using System.Collections.Generic;
using SongRequestManager.Queue;
using SongRequestManager.Utils;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public class MyRequestCommand : Command
    {
        public override List<string> Aliases { get; } = new List<string> { "my", "myqueue", "myrequest", "me" };
        public override string HelpText { get; } = "View the current position of your own song request in the request queue.";
        public override string SampleUsage { get; } = "!my";

        protected override string Execute(ChatCommand command)
        {
            var username = command.ChatMessage.DisplayName;
            var request = RequestQueue.Current.GetRequestByUsername(username);

            if (request == null)
            {
                return $"Couldn't find a request for @{username} 🤔";
            }

            var position = RequestQueue.Current.GetPositionOf(request);
            string messageRoot = $"Request {request.Song.Name} is in position #{position.Position}";

            if (position.Position == 1)
            {
                return messageRoot;
            }
            else
            {
                return $"{messageRoot} behind {StringUtils.GetDurationString(position.DurationAheadSeconds)} of requests";
            }
        }
    }
}
