using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public class OopsCommand : Command
    {
        public override List<string> Aliases { get; } = new List<string> { "oops", "wrongsong", "wrong", "ws" };

        public override Task<string> ExecuteAsync(ChatCommand command)
        {
            return Task.FromResult($"Use '!remove' to delete your request or '!replace [id]' to replace it without losing your spot in the queue!");
        }
    }
}
