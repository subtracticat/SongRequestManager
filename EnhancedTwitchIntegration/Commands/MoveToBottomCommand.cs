using System.Collections.Generic;
using SongRequestManager.Queue;
using SongRequestManager.Utils;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public class MoveToBottomCommand : ModeratorCommand
    {
        public override List<string> Aliases { get; } = new List<string> { "mtb", "last", "demote" };
        public override string HelpText { get; } = "Move an existing request to the end of the request queue.";
        public override string SampleUsage { get; } = "!demote [id]";

        protected override string Execute(ChatCommand command)
        {
            var args = command.ArgumentsAsList;

            if (args.Count != 1)
            {
                return $"Expected a song ID: '!{command.CommandText} [id]'";
            }

            string id = args[0];

            if (!RequestUtils.IsBeatSaverId(id))
            {
                return $"That doesn't look like an ID 🤔";
            }

            var request = RequestQueue.Current.Remove(id, RequestStatus.Queued);

            if (request == null)
            {
                return $"Couldn't find request {id} in the queue.";
            }

            var result = RequestQueue.Current.Add(request);
            return $"{request.Song.Name} requested by {request.RequestedBy} moved to position #{result.Position}";
        }
    }
}
