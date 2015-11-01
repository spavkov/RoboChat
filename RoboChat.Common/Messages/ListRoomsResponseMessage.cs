using System.Collections.Generic;

namespace RoboChat.Common.Messages
{
    public class ListRoomsResponseMessage
    {
        public List<ChatRoomDetails> Rooms { get; private set; }

        public string HubId { get; private set; }

        public ListRoomsResponseMessage(string hubId, List<ChatRoomDetails> rooms)
        {
            HubId = hubId;
            Rooms = rooms;
        }
    }

    public class ChatRoomDetails
    {
        public ChatRoomDetails(string id, string name, int participantsCount)
        {
            Id = id;
            Name = name;
            ParticipantsCount = participantsCount;
        }

        public string Id { get; private set; }

        public string Name { get; private set; }

        public int ParticipantsCount { get; private set; }
    }
}