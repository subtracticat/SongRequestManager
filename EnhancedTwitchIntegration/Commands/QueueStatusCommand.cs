using System.Collections.Generic;
using System.Linq;
using SongRequestManager.Config;
using SongRequestManager.Queue;
using SongRequestManager.Utils;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public class QueueStatusCommand : Command
    {
        public override List<string> Aliases { get; } = new List<string> { "queuestatus" };
        public override string HelpText { get; } = "Shows the current status of the queue - whether requests are open and the current size of the queue.";
        public override string SampleUsage { get; } = "!queuestatus";

        protected override string Execute(ChatCommand command)
        {
            bool isOpen = RequestBotSettings.Current.Data.RequestQueueOpen;
            var queue = RequestQueue.Current.Data.Requests;
            int songCount = queue.Count;
            float durationSeconds = queue.Sum(request => request.Song.Metadata.Duration);

            string queueState = $"Queue is {(isOpen ? "open!" : "closed.")}";

            // Include trailing space here instead of the host string to avoid ending up with a double space if this were to be empty.
            string duration = songCount > 0 ? $"({StringUtils.GetDurationString((int)durationSeconds)}) " : string.Empty;

            return $"{queueState} There are {songCount} songs {duration}in the queue.";
        }
    }
}
