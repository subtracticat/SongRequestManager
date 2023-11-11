using System;
using System.Collections.Generic;
using System.Linq;
using SongRequestManager.Config;
using SongRequestManager.Queue;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public class QueueLotteryCommand : ModeratorCommand
    {
        public override List<string> Aliases { get; } = new List<string> { "queuelottery", "lottery" };
        public override string HelpText { get; } = "Randomly selects a given number of requests from the queue, while removing the remaining requests.";
        public override string SampleUsage { get; } = "!queuelottery [number]";

        protected override string Execute(ChatCommand command)
        {
            var args = command.ArgumentsAsList;

            if (args.Count == 1 && int.TryParse(args[0], out int count))
            {
                if (RequestBotSettings.Current.Data.RequestQueueOpen)
                {
                    RequestBotSettings.Current.Update(config => config.RequestQueueOpen = false);
                    ChatHandler.Send("Queue is now closed!");
                }

                var queue = RequestQueue.Current.Data.Requests;
                count = Math.Min(count, queue.Count);
                List<SongRequest> savedRequests = new List<SongRequest>();
                var random = new Random();

                for (int i = 0; i < count; i++)
                {
                    int selectedIndex = random.Next(queue.Count);
                    savedRequests.Add(RequestQueue.Current.Remove(queue[selectedIndex].Song.ID, RequestStatus.Queued));
                }

                RequestQueue.Current.ClearQueue();
                RequestQueue.Current.Add(savedRequests);

                return $"Selected {string.Join(", ", savedRequests.Select(request => $"{request.Song.Metadata.SongName} ({request.RequestedBy})"))}";
            }

            return "Help me help you! '!queuelottery [count]'";
        }
    }
}
