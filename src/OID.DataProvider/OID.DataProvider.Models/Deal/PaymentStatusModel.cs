namespace OID.DataProvider.Models.Deal
{
    public class PaymentStatusModel 
    {
        public PaymentStatusModel(string code, string name)
        {
            Code = code;
            Name = name;
        }

        public string Code { get; }

        public string Name { get; }
    }
}
