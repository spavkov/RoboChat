using System.Collections.Generic;

namespace RoboChat.Common.Messages
{
    public class RoomsHaveChangedMessage
    {
        public RoomsHaveChangedMessage(string hubId, List<ChatRoomDetails> rooms)
        {
            HubId = hubId;
            Rooms = rooms;
        }

        public string HubId { get; private set; }

        public List<ChatRoomDetails> Rooms { get; private set; }
    }
}