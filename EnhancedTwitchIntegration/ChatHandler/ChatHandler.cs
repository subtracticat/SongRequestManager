using System;
using System.Collections.Generic;
using System.Linq;
using SongRequestManager.ChatHandlers;
using SongRequestManager.Commands;
using SongRequestManager.Config;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Unity;

namespace SongRequestManager
{
    public class ChatHandler : PersistentSingleton<ChatHandler>
    {
        bool initialized = false;
        private static TwitchLibUnityClient _chatClient;
        private static List<string> CensorList = new List<string>();
        private static ChatUser _defaultSelf = new ChatUser("0", "SRM", "SRM", true, false, "#FFFFFF", null, false, false, false);

        public static string CurrentUsername => _chatClient?.TwitchUsername;
        public static bool IsConnected => _chatClient?.IsConnected == true;

        private static readonly List<Command> Commands = new List<Command> 
        {
            new RemapCommand(),
            new SetQueueStatusCommand(),
            new SongRequestCommand(),
            new UnmapCommand()
        };
        
        public static ChatUser Self => _defaultSelf;

        private static readonly Dictionary<string, Command> CommandMap = new Dictionary<string, Command>();

        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void Init()
        {
            if (!initialized)
            {
                this.InitializeCommandMap();
                this.ConfigureChatClient(ChatConfigManager.Instance.Config);

                ChatConfigManager.Instance.OnChanged += ConfigureChatClient;

                initialized = true;
            }
        }

        private void InitializeCommandMap()
        {
            CommandMap.Clear();

            foreach (var command in Commands)
            {
                foreach (var alias in command.Aliases)
                {
                    var processedAlias = alias.ToLower();
                    if (CommandMap.ContainsKey(processedAlias))
                    {
                        throw new InvalidOperationException($"Duplicate command alias: ${processedAlias}");
                    }

                    CommandMap[processedAlias] = command;
                }
            }
        }

        private void ConfigureChatClient(ChatConfig config)
        {
            Plugin.Log("Configuring chat client");
            if (!string.IsNullOrEmpty(config.ChatToken) && !string.IsNullOrEmpty(config.ChatUsername) && !string.IsNullOrEmpty(config.ChatChannel))
            {
                var username = config.ChatUsername;
                var token = config.ChatToken;
                var channel = config.ChatChannel;

                ConnectionCredentials credentials = new ConnectionCredentials(username, token);

                if (_chatClient?.IsConnected == true)
                {
                    _chatClient.Disconnect();
                }

                _chatClient = new TwitchLibUnityClient();
                _chatClient.AutoReListenOnException = true;
                _chatClient.OnChatCommandReceived += OnChatCommandReceived;
                _chatClient.OnConnected += (sender, args) => Plugin.Log("Connected to chat!");
                _chatClient.OnConnectionError += (sender, error) => Plugin.Log($"Error connecting to chat: {error.Error.Message}");

                Plugin.Log("Initializing chat client with credentials");
                _chatClient.Initialize(credentials, channel);
                _chatClient.Connect();
            }
            else
            {
                Plugin.Log($"Skipping Chat Client init due to missing config arguments");
            }
        }

        private async void OnChatCommandReceived(object sender, OnChatCommandReceivedArgs chatCommand)
        {
            if (CommandMap.TryGetValue(chatCommand.Command.CommandText.ToLower(), out Command command))
            {
                try
                {
                    var message = chatCommand.Command.ChatMessage;

                    if (message.IsModerator || message.IsBroadcaster || !command.IsModOnly)
                    {
                        await command.ExecuteAsync(chatCommand.Command);
                    }
                }
                catch (Exception e)
                {
                    Plugin.Log($"Error handling command [{chatCommand.Command.ChatMessage.Message}]");
                    Plugin.Log($"Error: {e.ToString()}");
                }
            }
        }

        public static void Send(string message, string replyToId = null)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            if (!IsConnected)
            {
                Plugin.Log($"Attempted to send chat message, but client is not connected to chat.");
                return;
            }

            if (CensorList.Any(word => message.Contains(word)))
                foreach (var word in CensorList)
                {
                    message = message.Replace(word, "***");
                }
            try
            {
                if (!string.IsNullOrEmpty(replyToId))
                {
                    _chatClient.SendReply(ChatConfigManager.Instance.Config.ChatChannel, replyToId, message);
                }
                else
                {
                    _chatClient.SendMessage(ChatConfigManager.Instance.Config.ChatChannel, message);
                }
            }
            catch (Exception e)
            {
                Plugin.Log($"Exception was caught when trying to send bot message. {e.ToString()}");
            }
        }
    }
}