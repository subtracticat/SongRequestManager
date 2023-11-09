using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public abstract class Command
    {
        public virtual bool IsModOnly => false;
        
        public abstract List<string> Aliases { get; }

        public abstract Task ExecuteAsync(ChatCommand command);
    }

    public abstract class ModeratorCommand : Command
    {
        public override bool IsModOnly => true;
    }
}
