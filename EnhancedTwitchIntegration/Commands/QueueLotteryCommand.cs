using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SongRequestManager.Config;
using SongRequestManager.Queue;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public class QueueLotteryCommand : ModeratorCommand
    {
        public override List<string> Aliases { get; } = new List<string> { "queuelottery", "lottery" };

        public override Task<string> ExecuteAsync(ChatCommand command)
        {
            var args = command.ArgumentsAsList;

            if (args.Count == 1 && int.TryParse(args[0], out int count))
            {
                if (QueueConfigManager.Instance.Config.RequestQueueOpen)
                {
                    QueueConfigManager.Instance.UpdateSettings(config => config.RequestQueueOpen = false);
                    ChatHandler.Send("Queue is now closed!");
                }

                var queue = QueueManager.Instance.Config.Requests;
                count = Math.Min(count, queue.Count);
                List<SongRequest> savedRequests = new List<SongRequest>();
                var random = new Random();

                for (int i = 0; i < count; i++)
                {
                    int selectedIndex = random.Next(queue.Count);
                    savedRequests.Add(QueueManager.Instance.Remove(queue[selectedIndex].Song.ID, RequestStatus.Queued));
                }

                QueueManager.Instance.ClearQueue();
                QueueManager.Instance.Add(savedRequests);

                return Task.FromResult($"Selected {string.Join(", ", savedRequests.Select(request => $"{request.Song.Metadata.SongName} ({request.RequestedBy})"))}");
            }

            return Task.FromResult("Help me help you! '!queuelottery [count]'");
        }
    }
}
