namespace RoboChat.Common.Messages
{
    public class HubNameResponseMessage
    {
        public HubNameResponseMessage(string hubName)
        {
            this.HubName = hubName;
        }

        public string HubName { get; private set; }
    }
}