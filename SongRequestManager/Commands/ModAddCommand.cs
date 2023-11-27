using System.Collections.Generic;
using System.Threading.Tasks;
using SongRequestManager.Chat;
using SongRequestManager.Queue;
using SongRequestManager.Utils;

namespace SongRequestManager.Commands
{
    public class ModAddCommand : ModeratorCommand
    {
        private static readonly SongRequestRuleConfig Config = new SongRequestRuleConfig
        {
            EnforceDuplicate = false,
            EnforceAlreadyPlayed = false,
            EnforceConcurrentByUser = false,
            EnforceMaxLength = false
        };

        public override List<string> Aliases { get; } = new List<string> { "modadd", "modaddfor" };
        public override string HelpText { get; } = "Force-add a song into the queue, bypassing most request limitations, optionally on behalf of another user.";
        public override string SampleUsage { get; } = "!modadd [id] [username?] [prioValue?]";

        public override async Task<string> ExecuteAsync(ChatCommand command)
        {
            var args = command.Arguments;

            if (args.Count == 0 || args.Count > 3)
            {
                return $"Hmmmm... Try that again? '{this.SampleUsage}'";
            }

            string id = args[0].ToLower();
            string username = args.Count > 1 ? RequestUtils.GetUsernameParameter(args[1]) : command.Username;
            float prioValue = 0f;

            if (args.Count > 2)
            {
                if (!float.TryParse(args[2], out prioValue))
                {
                    return $"Expected the prio value to be a number. '{this.SampleUsage}'";
                }
            }

            GetSongResult result = await RequestUtils.GetRequestableSongAsync(id, username, Config);

            if (result.Song != null)
            {
                if (PriorityTracker.TryRedeemPrio(username, out PriorityItem storedPrio))
                {
                    prioValue += storedPrio.GetTotalValue();
                }

                SongRequest request = new SongRequest(result.Song, username)
                {
                    PriorityValue = prioValue
                };

                var queuePosition = RequestQueue.Current.Add(request);

                return StringUtils.GetSongAddedMessage(result.Song, queuePosition, prioValue > 0f);
            }
            else
            {
                return result.Message;
            }
        }
    }
}
