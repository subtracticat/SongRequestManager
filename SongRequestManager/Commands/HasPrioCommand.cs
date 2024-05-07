using System.Collections.Generic;
using SongRequestManager.Chat;
using SongRequestManager.Queue;
using SongRequestManager.Utils;

namespace SongRequestManager.Commands
{
    public class HasPrioCommand : ModeratorCommand
    {
        public override List<string> Aliases { get; } = new List<string> { "hasprio" };
        public override string HelpText { get; } = "Checks whether or not the specified user currently has an available prio request";
        public override string SampleUsage { get; } = "!hasprio [username]";

        protected override string Execute(ChatCommand command)
        {
            var args = command.Arguments;

            if (args.Count > 1)
            {
                return $"Help me help you - who do you want to know about? {{{SampleUsage}}}";
            }

            var username = RequestUtils.GetUsernameParameter(args[0]);
            return PrioHelpers.CheckPrio(username);
        }
    }

    public class MyPrioCommand : Command
    {
        public override List<string> Aliases { get; } = new List<string> { "myprio" };
        public override string HelpText { get; } = "Checks whether or not the requesting user currently has an available prio request";
        public override string SampleUsage { get; } = "!myprio";

        protected override string Execute(ChatCommand command)
        {
            return PrioHelpers.CheckPrio(command.Username);
        }
    }

    public static class PrioHelpers
    {
        public static string CheckPrio(string username)
        {
            if (PriorityTracker.TryRedeemPrio(username, out var _, dryRun: true))
            {
                return $"Prio found! Yes, @{username} has a stored prio request!";
            }
            else
            {
                return $"As far as I know, it doesn't look like @{username} has a prio available. :(";
            }
        }
    }
}
