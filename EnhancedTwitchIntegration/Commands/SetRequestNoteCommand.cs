using System.Collections.Generic;
using SongRequestManager.Chat;
using SongRequestManager.Queue;

namespace SongRequestManager.Commands
{
    public class SetRequestNoteCommand : ModeratorCommand
    {
        public override List<string> Aliases { get; } = new List<string> { "comment", "note", "songmsg" };
        public override string HelpText { get; } = "Sets a comment on the song request, visible to the streamer on the in-game queue page.";
        public override string SampleUsage { get; } = "!comment [id] [text]";

        protected override string Execute(ChatCommand command)
        {
            var args = command.Arguments;

            if (args.Count <= 1)
            {
                return $"What would you like to say? '!{command.Verb} [id] [message]";
            }

            string id = args[0];
            string message = string.Join(" ", args.GetRange(1, args.Count - 1));

            SongRequest request = RequestQueue.Current.GetRequestById(id);
            if (request == null)
            {
                return $"No request found for ID {id}";
            }

            request.Comment = message;
            return $"{request.Song.Metadata.SongName} comment updated!";
        }
    }
}
