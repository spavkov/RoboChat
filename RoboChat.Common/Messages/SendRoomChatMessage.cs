using System;

namespace RoboChat.Common.Messages
{
    public class SendRoomChatMessage
    {
        public SendRoomChatMessage(string roomId, string senderNick, string messageText, DateTime sentDateTimeUtc)
        {
            RoomId = roomId;
            SenderNick = senderNick;
            MessageText = messageText;
            SentUtc = sentDateTimeUtc;
        }
        public string RoomId { get; private set; }
        public string SenderNick { get; private set; }
        public string MessageText { get; private set; }
        public DateTime SentUtc { get; private set; }
    }
}