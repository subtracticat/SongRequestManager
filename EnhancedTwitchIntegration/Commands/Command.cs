using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public abstract class Command
    {
        public virtual bool IsModOnly => false;
        
        public abstract List<string> Aliases { get; }
        public abstract string HelpText { get; }
        public abstract string SampleUsage { get; }

        public virtual Task<string> ExecuteAsync(ChatCommand command) => Task.FromResult(this.Execute(command));
        protected virtual string Execute(ChatCommand command) => string.Empty;
    }

    public abstract class ModeratorCommand : Command
    {
        public override bool IsModOnly => true;
    }
}
