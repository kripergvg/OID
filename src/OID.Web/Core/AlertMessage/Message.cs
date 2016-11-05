namespace OID.Web.Core.AlertMessage
{
    public class Message
    {
        public Message()
        {
            
        }

        public Message(string text, MessageType messageType)
        {
            Text = text;
            MessageType = messageType;
        }

        public string Text { get; set; }

        public MessageType MessageType { get; set; }
    }
}
