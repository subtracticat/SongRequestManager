using System.Collections.Generic;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public abstract class Command
    {
        public bool IsModOnly = false;
        
        public abstract List<string> Aliases { get; }

        public abstract void Execute(ChatCommand command);
    }
}
