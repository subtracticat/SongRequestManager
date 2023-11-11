using System.Collections.Generic;
using SongRequestManager.Queue;
using SongRequestManager.Utils;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public class WhoCommand : ModeratorCommand
    {
        public override List<string> Aliases { get; } = new List<string> { "who" };
        public override string HelpText { get; } = "Checks to see who requested a specific song in the queue, or what request a specific user has in the queue (if any).";
        public override string SampleUsage { get; } = "!who [id] OR !who [username]";

        protected override string Execute(ChatCommand command)
        {
            if (command.ArgumentsAsList.Count != 1)
            {
                return "What would you like to know? '!who [id/username]`";
            }

            string arg = command.ArgumentsAsList[0];

            if (RequestUtils.IsBeatSaverId(arg))
            {
                var request = QueueManager.Instance.GetRequestById(arg);

                if (request != null)
                {
                    var position = QueueManager.Instance.GetPositionOf(request);
                    return $"{request.RequestedBy}: {request.Song.Name} ({request.Song.ID}) at position {position.Position}";
                }

                return $"Couldn't find request {arg} in the queue.";
            }
            else
            {
                var request = QueueManager.Instance.GetRequestByUsername(arg.Replace("@", string.Empty));

                if (request != null)
                {
                    var position = QueueManager.Instance.GetPositionOf(request);
                    return $"{request.RequestedBy}: {request.Song.Name} ({request.Song.ID}) at position {position.Position}";
                }

                return $"Couldn't find a request for user ${command.ChatMessage.DisplayName} in the queue.";
            }
        }
    }
}
