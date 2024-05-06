using System;
using System.Collections.Generic;
using SongRequestManager.Chat;
using SongRequestManager.Queue;
using SongRequestManager.Utils;

namespace SongRequestManager.Commands
{
    public class BumpCommand : Command
    {
        public override List<string> Aliases => new List<string> { "bump", "transfer" };
        public override string HelpText { get; } = "Applies the user's stored prio value to another user, potentially bumping them up the queue.";
        public override string SampleUsage { get; } = "!bump [username] OR !bump [song id]";

        protected override string Execute(ChatCommand command)
        {
            var args = command.Arguments;

            if (args.Count != 1)
            {
                return "Who/What would you like to bump? '!bump [id/username]`";
            }

            string arg = args[0];

            if (RequestUtils.IsBeatSaverId(arg))
            {
                var request = RequestQueue.Current.GetRequestById(arg);

                if (request == null)
                {
                    return $"Couldn't find request {arg} in the queue.";
                }
                else
                {
                    return this.TransferPrio(command.Username, request.RequestedBy);
                }
            }
            else
            {
                var targetUser = RequestUtils.GetUsernameParameter(arg);
                return this.TransferPrio(command.Username, targetUser);
            }
        }

        private string TransferPrio(string fromUser, string toUser)
        {
            if (PriorityTracker.TryRedeemPrio(fromUser, out var priority))
            {
                var value = priority.GetTotalValue();
                PriorityTracker.RegisterPriorityEvent(toUser, new PriorityEvent
                {
                    Value = value,
                    Type = PriorityEventType.Transfer,
                    Timestamp = DateTime.Now
                });

                return $"Transferred ${value:0.00} prio to {toUser}";
            }

            return $"No stored prio request found for {fromUser} :(";
        }
    }
}
