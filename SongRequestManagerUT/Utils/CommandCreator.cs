using System;
using System.Linq;
using System.Text.RegularExpressions;
using SongRequestManager.Chat;

namespace SongRequestManagerUT.Utils
{
    public class CommandCreator
    {
        private static readonly Regex CommandRegex = new Regex(@"^\!(?<verb>[^\s]+)\s?(?<args>.*)$");

        private static int CommandId = 0;
        private readonly ChatCommand command;

        private CommandCreator(string message, string username = "TestUser")
        {
            var match = CommandRegex.Match(message);

            if (!match.Success)
            {
                throw new ArgumentException($"Failed to parse command: [{message}]");
            }

            this.command = new ChatCommand
            {
                MessageId = $"{CommandId++}",
                Username = username,
                Verb = match.Groups["verb"].Value,
                Arguments = match.Groups["args"].Value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                FullText = message
            };
        }

        public static CommandCreator Create(string message)
        {
            return new CommandCreator(message);
        }

        public CommandCreator AsModerator()
        {
            this.command.IsModerator = true;
            return this;
        }

        public CommandCreator FromUser(string username)
        {
            this.command.Username = username;
            return this;
        }

        public ChatCommand Build()
        {
            return this.command;
        }
    }
}
