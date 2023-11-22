using System;
using System.Collections.Generic;
using System.Linq;
using SongRequestManager.Config;
using SongRequestManager.Queue;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Unity;

namespace SongRequestManager.Chat
{
    public class ChatCommand
    {
        public string MessageId { get; set; }
        public string Username { get; set; }
        public string Verb { get; set; }
        public List<string> Arguments { get; set; } = new List<string>();
        public bool IsModerator { get; set; }
        public string FullText { get; set; }
    }

    public delegate void OnPrioEventHandler(string username, PriorityEvent arg);

    public interface IChatProvider
    {
        bool IsConnected { get; }

        void Disconnect();

        void SendMessage(string message, string replyToId = null);

        event OnPrioEventHandler OnPrioEvent;
        event Action<ChatCommand> OnChatCommand; 
    }

    public class ChatProvider : IChatProvider
    {
        private static List<string> CensorList = new List<string>();

        private readonly TwitchLibUnityClient _chatClient;

        public event OnPrioEventHandler OnPrioEvent;
        public event Action<ChatCommand> OnChatCommand;

        public bool IsConnected => this._chatClient.IsConnected;

        private ChatProvider(ConnectionCredentials credentials, string channelName)
        {
            _chatClient = new TwitchLibUnityClient
            {
                AutoReListenOnException = true
            };

            _chatClient.OnChatCommandReceived += OnChatCommandReceived;
            _chatClient.OnMessageReceived += OnMessageReceived;
            _chatClient.OnNewSubscriber += OnNewSubscriber;
            _chatClient.OnReSubscriber += OnReSubscriber;
            _chatClient.OnGiftedSubscription += OnGiftedSubscription;
            _chatClient.OnConnected += (sender, args) => Plugin.Log("Connected to chat!");
            _chatClient.OnConnectionError += (sender, error) => Plugin.Log($"Error connecting to chat: {error.Error.Message}");

            Plugin.Log("Initializing chat client with credentials");
            _chatClient.Initialize(credentials, channelName);
            _chatClient.Connect();
        }

        public static ChatProvider Create()
        {
            var config = TwitchConnectionSettings.Current.Data;

            Plugin.Log("Configuring chat client");
            if (!string.IsNullOrEmpty(config.ChatToken) && !string.IsNullOrEmpty(config.ChatUsername) && !string.IsNullOrEmpty(config.ChatChannel))
            {
                var username = config.ChatUsername;
                var token = config.ChatToken;
                var channel = config.ChatChannel;

                ConnectionCredentials credentials = new ConnectionCredentials(username, token);

                return new ChatProvider(credentials, channel);
            }
            else
            {
                Plugin.Log($"Skipping Chat Client init due to missing config arguments");
                return null;
            }
        }

        private void OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            var command = e.Command;
            var message = command.ChatMessage;

            this.OnChatCommand?.Invoke(new ChatCommand
            {
                MessageId = message.Id,
                Username = message.DisplayName,
                Verb = command.CommandText,
                Arguments = command.ArgumentsAsList,
                FullText = message.Message,
                IsModerator = message.IsModerator || message.IsBroadcaster
            });
        }

        private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (e.ChatMessage.Bits > 0)
            {
                this.OnPrioEvent?.Invoke(
                    e.ChatMessage.DisplayName,
                    new PriorityEvent
                    {
                        Type = PriorityEventType.Bits,
                        Value = e.ChatMessage.Bits / 100.0f,
                        Timestamp = DateTime.Now
                    });
            }
        }

        private void OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            this.OnPrioEvent?.Invoke(
                e.Subscriber.DisplayName,
                new PriorityEvent
                {
                    Type = PriorityEventType.Subscription,
                    Value = GetSubValue(e.Subscriber.SubscriptionPlan),
                    Timestamp = DateTime.Now
                });
        }

        private void OnReSubscriber(object sender, OnReSubscriberArgs e)
        {
            this.OnPrioEvent?.Invoke(
                e.ReSubscriber.DisplayName,
                new PriorityEvent
                {
                    Type = PriorityEventType.Subscription,
                    Value = GetSubValue(e.ReSubscriber.SubscriptionPlan),
                    Timestamp = DateTime.Now
                });
        }

        private void OnGiftedSubscription(object sender, OnGiftedSubscriptionArgs e)
        {
            this.OnPrioEvent?.Invoke(
                e.GiftedSubscription.DisplayName,
                new PriorityEvent
                {
                    Type = PriorityEventType.GiftSubscription,
                    Value = GetSubValue(e.GiftedSubscription.MsgParamSubPlan),
                    Timestamp = DateTime.Now
                });
        }

        private static float GetSubValue(SubscriptionPlan plan)
        {
            switch (plan)
            {
                case SubscriptionPlan.Tier1:
                case SubscriptionPlan.Prime:
                    return 4.99f;

                case SubscriptionPlan.Tier2:
                    return 9.99f;

                case SubscriptionPlan.Tier3:
                    return 24.99f;

                default:
                    return 0;
            }
        }

        public void Disconnect()
        {
            this._chatClient?.Disconnect();
        }

        public void SendMessage(string message, string replyToId = null)
        {
            if (!_chatClient.IsConnected)
            {
                Plugin.Log($"Attempted to send chat message, but client is not connected to chat.");
                return;
            }

            if (CensorList.Any(word => message.Contains(word)))
            {
                foreach (var word in CensorList)
                {
                    message = message.Replace(word, "***");
                }
            }

            try
            {
                if (!string.IsNullOrEmpty(replyToId))
                {
                    this._chatClient.SendReply(TwitchConnectionSettings.Current.Data.ChatChannel, replyToId, message);
                }
                else
                {
                    this._chatClient.SendMessage(TwitchConnectionSettings.Current.Data.ChatChannel, message);
                }
            }
            catch (Exception e)
            {
                Plugin.Log($"Exception was caught when trying to send bot message. {e.ToString()}");
            }
        }
    }
}
