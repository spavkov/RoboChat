namespace RoboChat.Common.Messages
{
    public class NearestHubResponseMessage
    {
        public NearestHubResponseMessage(string hubId, string hubRegion)
        {
            HubId = hubId;
            HubRegion = hubRegion;
        }

        public string HubId { get; private set; }
        public string HubRegion { get; private set; }
    }
}