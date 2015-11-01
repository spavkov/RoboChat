namespace RoboChat.Common.Messages.Client
{
    public class ClientCreateRoomMessage
    {
        public ClientCreateRoomMessage(string roomName)
        {
            RoomName = roomName;
        }

        public string RoomName { get; private set; } 
    }
}