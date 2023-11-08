using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public class OpenQueueCommand : Command
    {
        public override List<string> Aliases => new List<string> { "open" };

        public override void Execute(ChatCommand command)
        {
            
        }
    }
}
