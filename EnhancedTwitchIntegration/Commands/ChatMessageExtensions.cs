using TwitchLib.Client.Models;

namespace SongRequestManager.Commands
{
    public static class ChatMessageExtensions
    {
        public static void Reply(this ChatMessage message, string replyText) => ChatHandler.Send(replyText, replyToId: message.Id);
        public static void Reply(this ChatCommand message, string replyText) => ChatHandler.Send(replyText, replyToId: message.ChatMessage.Id);
    }
}
