namespace RoboChat.Common.Messages
{
    public class ConnectToHubMessage
    {
        public ConnectToHubMessage(string clientNick)
        {
            ClientNick = clientNick;
        }

        public string ClientNick { get; private set; }
    }
}