using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public abstract class Command
    {
        public virtual bool IsModOnly { get; } = false;
        
        public abstract List<string> Aliases { get; }

        public abstract Task Execute(ChatCommand command);
    }
}
