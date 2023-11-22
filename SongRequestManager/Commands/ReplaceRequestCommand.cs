using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SongRequestManager.Chat;
using SongRequestManager.Queue;
using SongRequestManager.Utils;

namespace SongRequestManager.Commands
{
    public class ReplaceRequestCommand : Command
    {
        private static readonly SongRequestRuleConfig Config = new SongRequestRuleConfig
        {
            EnforceDuplicate = true,
            EnforceAlreadyPlayed = true,
            EnforceConcurrentByUser = false,
            EnforceMaxLength = true
        };

        public override List<string> Aliases => new List<string> { "replace" };
        public override string HelpText { get; } = "Replaces the song attached to the current request in the queue. Optionally, moderators can specify 'oldId' ID to replace a request on another user's behalf.";
        public override string SampleUsage { get; } = "!replace [oldId?] [newId]";

        public override Task<string> ExecuteAsync(ChatCommand command)
        {
            var args = command.Arguments;

            if (args.Count == 2)
            {
                string oldId = args[0];
                string newId = args[1];

                SongRequest currentRequest = RequestQueue.Current.GetRequestById(oldId);

                if (currentRequest != null)
                {
                    return this.ProcessReplace(command, currentRequest, newId);
                }
                else
                {
                    return Task.FromResult($"Couldn't find a request for ID {oldId}");
                }
            }
            else if (args.Count == 1)
            {
                string newId = args[0];

                var currentRequest = RequestQueue.Current.GetRequestByUsername(command.Username);
                if (currentRequest != null)
                {
                    return this.ProcessReplace(command, currentRequest, newId);
                }
                else
                {
                    return Task.FromResult($"Couldn't find a request for @{command.Username}");
                }
            }

            if (command.IsModerator)
            {
                return Task.FromResult($"Please provide the new ID that you want to replace the request with! '!replace [oldId?] [newId]'");
            }
            else
            {
                return Task.FromResult($"Please provide the new ID that you want to replace your request with! '!replace [newId]'");
            }
        }

        private async Task<string> ProcessReplace(ChatCommand command, SongRequest currentRequest, string newId)
        {
            if (currentRequest.RequestedBy.Equals(command.Username, StringComparison.CurrentCultureIgnoreCase) || command.IsModerator)
            {
                GetSongResult result = await RequestUtils.GetRequestableSongAsync(newId, currentRequest.RequestedBy, Config);
                if (result.Song != null)
                {
                    var originalSong = currentRequest.Song;
                    var position = RequestQueue.Current.ReplaceSong(currentRequest, result.Song);
                    return $"Request {originalSong.Name} at position #{position.Position} replaced with {result.Song.Name}";
                }
                else
                {
                    return result.Message;
                }
            }
            else
            {
                return $"Clever, but no, you can't replace someone else's request!";
            }
        }
    }
}
