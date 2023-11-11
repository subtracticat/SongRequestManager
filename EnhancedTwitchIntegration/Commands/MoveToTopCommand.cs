using System.Collections.Generic;
using System.Threading.Tasks;
using SongRequestManager.Queue;
using SongRequestManager.Utils;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public class MoveToTopCommand : ModeratorCommand
    {
        public override List<string> Aliases { get; } = new List<string> { "mtt" };
        public override string HelpText { get; } = "Move an existing request to the top of the request queue.";
        public override string SampleUsage { get; } = "!mtt [id]";

        public override Task<string> ExecuteAsync(ChatCommand command)
        {
            var args = command.ArgumentsAsList;

            if (args.Count != 1)
            {
                return Task.FromResult("Expected a song ID: '!mtt [id]'");
            }

            string id = args[0];

            if (!RequestUtils.IsBeatSaverId(id))
            {
                return Task.FromResult($"That doesn't look like an ID 🤔");
            }

            var request = RequestQueue.Current.Remove(id, RequestStatus.Queued);

            if (request == null)
            {
                return Task.FromResult($"Couldn't find request {id} in the queue.");
            }

            var result = RequestQueue.Current.InsertAt(0, request);
            return Task.FromResult($"{request.Song.Name} requested by {request.RequestedBy} moved to position #{result.Position}");
        }
    }
}
