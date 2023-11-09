using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SongRequestManager.Config;
using SongRequestManager.Queue;
using SongRequestManager.Service;
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

        public override async Task Execute(ChatCommand command)
        {
            if (!QueueConfigManager.Instance.Config.RequestQueueOpen)
            {
                command.Reply("Sorry, the queue is closed :(");
                return;
            }

            if (command.ArgumentsAsList.Count == 0 || command.ArgumentsAsList.Count > 1)
            {
                command.Reply($"Please provide a BeatSaver song ID for your request (something like '!bsr 4e4e')");
                return;
            }

            string id = command.ArgumentsAsList[0].ToLower();

            if (!CommandUtils.IsBeatSaverId(id))
            {
                command.Reply($"Hmm... I'm looking for a song ID and that doesn't look like one - are you sure you grabbed the right thing?");
                return;
            }

            if (RemapConfigManager.Instance.Config.ContainsKey(id))
            {
                id = RemapConfigManager.Instance.Config[id];
            }

            SongRequest existingUserRequest = RequestManager.GetRequestByUsername(command.ChatMessage.Username);
            if (existingUserRequest != null)
            {
                command.Reply($"One request at a time, please! If you'd like to replace your current request, use '!replace {id}'.");
                return;
            }

            SongRequest duplicateRequest = RequestManager.GetRequestById(id);
            if (duplicateRequest != null)
            {
                QueuePosition position = RequestManager.GetPositionOf(duplicateRequest);
                command.Reply($"{duplicateRequest.Song.Name} is already #{position.Position} in the queue, requested by @{duplicateRequest.RequestedBy}.");
                return;
            }

            if (RequestManager.HasPlayed(id))
            {
                command.Reply($"Sorry, we've already already been played that song! :(");
                return;
            }

            Song songData;
            try
            {
                songData = await BeatSaverService.Instance.GetSongDataAsync(id);
            }
            catch (Exception e)
            {
                Plugin.Log($"Exception loading song details for ID ${id}: ${e.ToString()}");
                command.Reply($"Encountered an error loading song details, double check the ID and try again.");
                return;
            }

            float maxLengthMinutes = QueueConfigManager.Instance.Config.MaximumSongLength;
            float songLengthMinutes = songData.Metadata.Duration / 60.0f;
            if (maxLengthMinutes > 0 && songLengthMinutes > maxLengthMinutes)
            {
                command.Reply($"Song length ({songLengthMinutes:0.0}m) is longer than the maximum allowed song length ({maxLengthMinutes}m)");
                return;
            }

            SongRequest request = new SongRequest(songData, command.ChatMessage.Username);
            var queuePosition = RequestManager.Add(request);

            command.Reply($"{songData.Name} added at position #{queuePosition.Position}!");
        }
    }
}
