using System;
using System.Collections.Generic;
using SongRequestManager.Chat;
using SongRequestManager.Queue;

namespace SongRequestManagerUT.Mocks
{
    public class MockChatProvider : IChatProvider
    {
        public bool IsConnected { get; set; } = true;
        public List<(string Message, string ReplyToID)> Messages = new List<(string Message, string ReplyToID)>();

        public event OnPrioEventHandler OnPrioEvent;
        public event Action<ChatCommand> OnChatCommand;

        public void Disconnect()
        {
            this.IsConnected = false;
        }

        public void SendMessage(string message, string replyToId = null)
        {
            this.Messages.Add((message, replyToId));
        }

        public void SendCommand(ChatCommand command)
        {
            this.OnChatCommand?.Invoke(command);
        }

        public void SendPrioEvent(string username, PriorityEvent payload)
        {
            this.OnPrioEvent?.Invoke(username, payload);
        }

        public void Reset()
        {
            this.Messages.Clear();
        }
    }
}
