using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SongRequestManager.Chat;
using SongRequestManager.Config;
using SongRequestManager.Queue;
using SongRequestManager.Service;

namespace SongRequestManager.Utils
{
    public class SongRequestRuleConfig
    {
        public bool EnforceDuplicate { get; set; } = true;
        public bool EnforceAlreadyPlayed { get; set; } = true;
        public bool EnforceConcurrentByUser { get; set; } = true;
        public bool EnforceMaxLength { get; set; } = true;
    }

    public class GetSongResult
    {
        public Song Song { get; private set; }
        public string Message { get; private set; }

        public GetSongResult(Song song)
        {
            this.Song = song;
        }

        public GetSongResult(string message)
        {
            this.Message = message;
        }
    }

    public static class RequestUtils
    {
        private static Regex HexRegex = new Regex("^[0-9a-fA-F]+$", RegexOptions.Compiled);

        public static bool IsBeatSaverId(string arg)
        {
            return HexRegex.Match(arg).Success;
        }

        public static string GetUsernameParameter(string input)
        {
            return input.Trim().Replace("@", string.Empty);
        }

        public static async Task<GetSongResult> GetRequestableSongAsync(string id, string requestedBy, SongRequestRuleConfig config)
        {
            if (!RequestUtils.IsBeatSaverId(id))
            {
                return new GetSongResult($"Hmm... I'm looking for a song ID and that doesn't look like one - are you sure you grabbed the right thing?");
            }

            if (SongModerationSettings.Current.Data.Bans.Contains(id))
            {
                return new GetSongResult($"{id} is blocked! 😬 Try picking something else?");
            }

            if (SongModerationSettings.Current.Data.Remaps.TryGetValue(id, out string remapValue))
            {
                ChatHandler.Send($"Remapping request {id} to {remapValue}");
                id = remapValue;
            }

            if (config.EnforceConcurrentByUser)
            {
                SongRequest existingUserRequest = RequestQueue.Current.GetRequestByUsername(requestedBy);
                if (existingUserRequest != null)
                {
                    return new GetSongResult($"Sorry, just one request at a time, please! If you'd like to replace your current request, use '!replace {id}'.");
                }
            }

            if (config.EnforceDuplicate)
            {
                SongRequest duplicateRequest = RequestQueue.Current.GetRequestById(id);
                if (duplicateRequest != null)
                {
                    QueuePosition position = RequestQueue.Current.GetPositionOf(duplicateRequest);
                    return new GetSongResult($"{duplicateRequest.Song.Name} is already #{position.Position} in the queue, requested by @{duplicateRequest.RequestedBy}.");
                }
            }

            if (config.EnforceAlreadyPlayed)
            {
                if (RequestQueue.Current.HasPlayed(id))
                {
                    return new GetSongResult($"Sorry, we've already already been played that song! :(");
                }
            }

            Song song;
            try
            {
                song = await BeatSaverService.Instance.GetSongDataAsync(id);
            }
            catch (Exception e)
            {
                Plugin.Log($"Exception loading song details for ID {id}: {e.ToString()}");
                return new GetSongResult($"Encountered an error loading song details, double check the ID and try again.");
            }

            if (config.EnforceMaxLength)
            {
                float maxLengthMinutes = RequestBotSettings.Current.Data.MaximumSongLength;
                float songLengthMinutes = song.Metadata.Duration / 60.0f;
                float songSeconds = song.Metadata.Duration % 60;
                if (maxLengthMinutes > 0 && songLengthMinutes > maxLengthMinutes)
                {
                    return new GetSongResult($"Song length ({songLengthMinutes:0}:{songSeconds:00}) is longer than the maximum allowed song length ({maxLengthMinutes}:00)");
                }
            }

            return new GetSongResult(song);
        }
    }
}
