using System.Collections.Generic;
using System.Threading.Tasks;
using SongRequestManager.Queue;
using SongRequestManager.Utils;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public class MoveCommand : ModeratorCommand
    {
        public override List<string> Aliases { get; } = new List<string> { "move" };

        public override Task<string> ExecuteAsync(ChatCommand command)
        {
            var args = command.ArgumentsAsList;

            if (args.Count != 2)
            {
                return Task.FromResult($"Move what? Where? '!move [id] [position]'");
            }

            string id = args[0];

            if (!RequestUtils.IsBeatSaverId(id))
            {
                return Task.FromResult($"That doesn't look like an ID 🤔");
            }

            string position = args[1];

            if (int.TryParse(position, out int positionInt))
            {
                var request = QueueManager.Instance.Remove(id, RequestStatus.Queued);

                if (request == null)
                {
                    return Task.FromResult($"Couldn't find request {id} in the queue.");
                }

                // -1 to convert to base-0 indexing
                var result = QueueManager.Instance.InsertAt(positionInt - 1, request);
                return Task.FromResult($"{request.Song.Name} requested by {request.RequestedBy} moved to position #{result.Position}");
            }
            else
            {
                return Task.FromResult($"Where did you want me to put it? I'm confused.");
            }
        }
    }
}
