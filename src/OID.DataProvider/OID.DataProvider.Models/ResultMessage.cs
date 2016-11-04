namespace OID.DataProvider.Models
{
    public class ResultMessage
    {
        public ResultMessage(int code, MessageType messageType, string message)
        {
            Code = code;
            MessageType = messageType;
            Message = message;
        }

        public int Code { get; }

        public MessageType MessageType { get; }

        public string Message { get; }
    }
}
