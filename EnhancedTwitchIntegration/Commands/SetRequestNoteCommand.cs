using System.Collections.Generic;
using System.Threading.Tasks;
using SongRequestManager.Queue;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public class SetRequestNoteCommand : ModeratorCommand
    {
        public override List<string> Aliases { get; } = new List<string> { "comment", "note", "songmsg" };

        public override Task<string> ExecuteAsync(ChatCommand command)
        {
            var args = command.ArgumentsAsList;

            if (args.Count <= 1)
            {
                return Task.FromResult($"What would you like to say? '!{command.CommandText} [id] [message]");
            }

            string id = args[0];
            string message = string.Join(" ", args.GetRange(1, args.Count - 1));

            SongRequest request = QueueManager.GetRequestById(id);
            if (request == null)
            {
                return Task.FromResult($"No request found for ID {id}");
            }

            request.Comment = message;
            return Task.FromResult($"Comment on request {request.Song.Name} updated!");
        }
    }
}
