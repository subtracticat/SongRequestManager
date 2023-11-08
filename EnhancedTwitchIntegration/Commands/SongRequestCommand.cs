using System.Collections.Generic;
using SongRequestManager.Config;
using SongRequestManager.Queue;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public class SongRequestCommand : Command
    {
        public override List<string> Aliases => new List<string> {
            "add",
            "bsr",
            "request",
            "sr",
            "srm"
        };

        public override void Execute(ChatCommand command)
        {
            if (!QueueConfig.Instance.RequestQueueOpen)
            {
                command.Reply("Sorry, the queue is closed :(");
                return;
            }

            if (command.ArgumentsAsList.Count == 0 || command.ArgumentsAsList.Count > 1)
            {
                command.Reply($"Please provide a BeatSaver song ID for your request (something like '!bsr 4e4e')");
                return;
            }

            string id = command.ArgumentsAsList[0];

            if (!CommandUtils.IsBeatSaverId(id))
            {
                command.Reply($"Hmm... I'm looking for a song ID and that doesn't look like one - are you sure you grabbed the right thing?");
                return;
            }

            SongRequest existingRequest = RequestManager.GetRequestById(id);

            if (existingRequest != null)
            {
                QueuePosition position = RequestManager.GetPositionOf(existingRequest);
                command.Reply($"{existingRequest.Song.Name} is already #{position.Position} in the queue, requested by @{existingRequest.RequestedBy}.");
                return;
            }

            if (RequestManager.HasPlayed(id))
            {
                command.Reply($"Sorry, we've already already been played that song! :(");
                return;
            }
        }
    }
}
