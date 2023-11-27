using System;
using System.Collections.Generic;
using SongRequestManager.Config;
using SongRequestManager.Queue;
using SongRequestManager.Chat;
using SongRequestManager.Utils;

namespace SongRequestManager.Commands
{
    public class AddPrioCommand : ModeratorCommand
    {
        public override List<string> Aliases { get; } = new List<string> { "addprio" };
        public override string HelpText { get; } = "Adds a new value to a user towards a present or future priority request.";
        public override string SampleUsage { get; } = "!addprio [username] [amount]";

        protected override string Execute(ChatCommand command)
        {
            if (!RequestBotSettings.Current.Data.EnableAutoPrio)
            {
                return $"Auto-prio system is currently disabled.";
            }

            var args = command.Arguments;

            if (args.Count != 2)
            {
                return $"Who? How much? '{this.SampleUsage}";
            }

            string username = RequestUtils.GetUsernameParameter(args[0]);
            string value = args[1];

            if (float.TryParse(value, out float parsedValue))
            {
                ChatHandler.Send($"Registering ${parsedValue:0.00} credit for {username}", command.MessageId);

                PriorityTracker.RegisterPriorityEvent(username, new PriorityEvent
                {
                    Type = PriorityEventType.Unknown,
                    Value = parsedValue,
                    Timestamp = DateTime.Now
                });

                return string.Empty;
            }
            else
            {
                return $"How much credit are we awarding? Either that wasn't a number or I'm SUPER confused...";
            }
        }
    }
}
