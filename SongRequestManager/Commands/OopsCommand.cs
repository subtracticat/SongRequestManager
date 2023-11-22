using System.Collections.Generic;
using SongRequestManager.Chat;

namespace SongRequestManager.Commands
{
    public class OopsCommand : Command
    {
        public override List<string> Aliases { get; } = new List<string> { "oops", "wrongsong", "wrong", "ws" };
        public override string HelpText { get; } = "Deprecated: use either !remove to remove your current request or !replace to replace it.";
        public override string SampleUsage { get; } = "!oops";

        protected override string Execute(ChatCommand command)
        {
            return $"Use '!remove' to delete your request or '!replace [id]' to replace it without losing your spot in the queue!";
        }
    }
}
