namespace OID.DataProvider.Models.Deal
{
    public class PaymentStatusModel 
    {
        public PaymentStatusModel(string code, string name, string sessionId)
        {
            Code = code;
            Name = name;
            SessionId = sessionId;
        }

        public string Code { get; }

        public string Name { get; }

        public string SessionId { get; }
    }
}
