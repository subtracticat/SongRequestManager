using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SongRequestManager.Queue;
using TwitchLib.Client.Models;

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

        public override Task<string> ExecuteAsync(ChatCommand command)
        {
            var args = command.ArgumentsAsList;
            var message = command.ChatMessage;

            if (args.Count == 2)
            {
                string oldId = args[0];
                string newId = args[1];

                SongRequest currentRequest = QueueManager.Instance.GetRequestById(oldId);

                if (currentRequest != null)
                {
                    return this.ProcessReplace(message, currentRequest, newId);
                }
                else
                {
                    return Task.FromResult($"Couldn't find a request for ID {oldId}");
                }
            }
            else if (args.Count == 1)
            {
                string newId = args[0];

                var currentRequest = QueueManager.Instance.GetRequestByUsername(message.Username);
                if (currentRequest != null)
                {
                    return this.ProcessReplace(message, currentRequest, newId);
                }
                else
                {
                    return Task.FromResult($"Couldn't find a request for @{message.Username}");
                }
            }

            if (message.IsModerator || message.IsBroadcaster)
            {
                return Task.FromResult($"Please provide the new ID that you want to replace your request with! '!replace [oldId?] [newId]'");
            }
            else
            {
                return Task.FromResult($"Please provide the new ID that you want to replace your request with! '!replace [newId]'");
            }
        }

        private async Task<string> ProcessReplace(ChatMessage message, SongRequest currentRequest, string newId)
        {
            if (currentRequest.RequestedBy.Equals(message.Username, StringComparison.CurrentCultureIgnoreCase) || message.IsModerator || message.IsBroadcaster)
            {
                GetSongResult result = await CommandUtils.GetRequestableSongAsync(newId, currentRequest.RequestedBy, Config);
                if (result.Song != null)
                {
                    var originalSong = currentRequest.Song;
                    var position = QueueManager.Instance.ReplaceSong(currentRequest, result.Song);
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
