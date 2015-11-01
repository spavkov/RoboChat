using System.Collections.Generic;
using System.Linq;

namespace RoboChat.Common.Messages.Client
{
    public class ClientRoomListResponseMessage
    {
        public List<ChatRoomDetails> Rooms { get; private set; }

        public ClientRoomListResponseMessage(IEnumerable<ChatRoomDetails> rooms)
        {
            this.Rooms = rooms.ToList();
        }
    }
}