using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SongRequestManager.Config;
using SongRequestManager.Queue;
using SongRequestManager.Utils;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public class QueueStatusCommand : Command
    {
        public override List<string> Aliases { get; } = new List<string> { "queuestatus" };

        public override Task<string> ExecuteAsync(ChatCommand command)
        {
            bool isOpen = QueueConfigManager.Instance.Config.RequestQueueOpen;
            var queue = QueueManager.Instance.Config.Requests;
            int songCount = queue.Count;
            float durationSeconds = queue.Sum(request => request.Song.Metadata.Duration);

            string queueState = $"Queue is {(isOpen ? "open!" : "closed.")}";

            // Include trailing space here instead of the host string to avoid ending up with a double space if this were to be empty.
            string duration = songCount > 0 ? $"({StringUtils.GetDurationString((int)durationSeconds)}) " : string.Empty;

            return Task.FromResult($"${queueState} There are {songCount} songs ${duration}in the queue.");
        }
    }
}
