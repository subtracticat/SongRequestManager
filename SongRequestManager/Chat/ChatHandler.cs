using System;
using System.Collections.Generic;
using System.Linq;
using SongRequestManager.Commands;
using SongRequestManager.Queue;

namespace SongRequestManager.Chat
{
    public static class ChatHandler
    {
        private static IChatProvider _chatProvider;

        private static readonly List<Command> Commands = new List<Command>
        {
            new AddToTopCommand(),
            new AddPrioCommand(),
            new BlockSongCommand(),
            new BumpCommand(),
            new ClearHistoryCommand(),
            new ClearQueueCommand(),
            new CloseQueueCommand(),
            new CurrentSongCommand(),
            new HasPrioCommand(),
            new HelpCommand(),
            new ModAddCommand(),
            new MoveCommand(),
            new MoveToBottomCommand(),
            new MoveToTopCommand(),
            new MyPrioCommand(),
            new MyRequestCommand(),
            new OopsCommand(),
            new OpenQueueCommand(),
            new PreviousSongCommand(),
            new QueueCommand(),
            new QueueLotteryCommand(),
            new QueueStatusCommand(),
            new RemapCommand(),
            new RemoveRequestCommand(),
            new ReplaceRequestCommand(),
            new SabotageCommand(),
            new SetRequestNoteCommand(),
            new SongRequestCommand(),
            new ToggleAutoPrioCommand(),
            new UnblockSongCommand(),
            new UnmapCommand(),
            new WhoCommand()
        };

        private static readonly Dictionary<string, Command> CommandMap = new Dictionary<string, Command>();

        public static bool TryGetCommand(string alias, out Command command)
        {
            return CommandMap.TryGetValue(alias, out command);
        }

        public static void Initialize(IChatProvider chatProvider = null)
        {
            InitializeCommandMap();

            if (_chatProvider?.IsConnected == true)
            {
                _chatProvider.OnChatCommand -= OnChatCommand;
                _chatProvider.OnPrioEvent -= PriorityTracker.RegisterPriorityEvent;
                _chatProvider.Disconnect();
            }

            if (chatProvider == null)
            {
                chatProvider = ChatProvider.Create();
            }

            chatProvider.OnPrioEvent += PriorityTracker.RegisterPriorityEvent;
            chatProvider.OnChatCommand += OnChatCommand;

            _chatProvider = chatProvider;
        }

        private static async void OnChatCommand(ChatCommand chatCommand)
        {
            if (CommandMap.TryGetValue(chatCommand.Verb.ToLower(), out Command command))
            {
                try
                {
                    if (chatCommand.IsModerator || !command.IsModOnly)
                    {
                        chatCommand.Arguments = chatCommand.Arguments.Where(arg => !string.IsNullOrEmpty(arg)).ToList();
                        string response = await command.ExecuteAsync(chatCommand);
                        if (!string.IsNullOrEmpty(response))
                        {
                            Send(response, chatCommand.MessageId);
                        }
                    }
                }
                catch (Exception e)
                {
                    Plugin.Log($"Error handling command [{chatCommand.FullText}]");
                    Plugin.Log($"Error: {e}");
                }
            }
        }

        private static void InitializeCommandMap()
        {
            CommandMap.Clear();

            foreach (var command in Commands)
            {
                foreach (var alias in command.Aliases)
                {
                    var processedAlias = alias.ToLower();
                    if (CommandMap.ContainsKey(processedAlias))
                    {
                        throw new InvalidOperationException($"Duplicate command alias: {processedAlias}");
                    }

                    CommandMap[processedAlias] = command;
                }
            }
        }

        public static void Send(string message, string replyToId = null)
        {
            _chatProvider.SendMessage(message, replyToId);
        }
    }
}