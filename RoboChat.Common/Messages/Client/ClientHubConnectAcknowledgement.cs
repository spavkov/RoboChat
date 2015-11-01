namespace RoboChat.Common.Messages.Client
{
    public class ClientHubConnectAcknowledgement
    {
        public ClientHubConnectAcknowledgement(string clientId)
        {
            ClientId = clientId;
        }

        public string ClientId { get; private set; }
    }
}