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

        public override async Task<string> ExecuteAsync(ChatCommand command)
        {
            if (!QueueConfigManager.Instance.Config.RequestQueueOpen)
            {
                return "Sorry, the queue is closed :(";
            }

            if (command.ArgumentsAsList.Count == 0 || command.ArgumentsAsList.Count > 1)
            {
                return $"Please provide a BeatSaver song ID for your request (something like '!bsr 4e4e')";
            }

            string id = command.ArgumentsAsList[0].ToLower();

            if (!CommandUtils.IsBeatSaverId(id))
            {
                return $"Hmm... I'm looking for a song ID and that doesn't look like one - are you sure you grabbed the right thing?";
            }

            if (ListConfigManager.Instance.Config.Bans.Contains(id))
            {
                return $"{id} is blocked! 😬 Try picking something else?";
            }

            if (ListConfigManager.Instance.Config.Remaps.ContainsKey(id))
            {
                id = ListConfigManager.Instance.Config.Remaps[id];
            }

            SongRequest existingUserRequest = QueueManager.GetRequestByUsername(command.ChatMessage.Username);
            if (existingUserRequest != null)
            {
                return $"One request at a time, please! If you'd like to replace your current request, use '!replace {id}'.";
            }

            SongRequest duplicateRequest = QueueManager.GetRequestById(id);
            if (duplicateRequest != null)
            {
                QueuePosition position = QueueManager.GetPositionOf(duplicateRequest);
                return $"{duplicateRequest.Song.Name} is already #{position.Position} in the queue, requested by @{duplicateRequest.RequestedBy}.";
            }

            if (QueueManager.HasPlayed(id))
            {
                return $"Sorry, we've already already been played that song! :(";
            }

            Song songData;
            try
            {
                songData = await BeatSaverService.Instance.GetSongDataAsync(id);
            }
            catch (Exception e)
            {
                Plugin.Log($"Exception loading song details for ID ${id}: ${e.ToString()}");
                return $"Encountered an error loading song details, double check the ID and try again.";
            }

            float maxLengthMinutes = QueueConfigManager.Instance.Config.MaximumSongLength;
            float songLengthMinutes = songData.Metadata.Duration / 60.0f;
            if (maxLengthMinutes > 0 && songLengthMinutes > maxLengthMinutes)
            {
                return $"Song length ({songLengthMinutes:0.0}m) is longer than the maximum allowed song length ({maxLengthMinutes}m)";
            }

            SongRequest request = new SongRequest(songData, command.ChatMessage.Username);
            var queuePosition = QueueManager.Add(request);

            return $"{songData.Name} [{songData.Metadata.LevelAuthorName}] {(songData.Stats.Score > 0 ? $"({(int)songData.Stats.Score}%)" : "")} added at position #{queuePosition.Position}!";
        }
    }
}
