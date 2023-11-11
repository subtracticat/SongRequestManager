using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SongRequestManager.Config;
using SongRequestManager.Queue;
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

            return Task.FromResult($"Queue is {(isOpen ? "open!" : "closed.")} There are {songCount} songs ({CommandUtils.GetDurationString((int)durationSeconds)}) in the queue.");
        }
    }
}
