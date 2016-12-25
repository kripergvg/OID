namespace OID.DataProvider.Models.Deal
{
    public class DeleveryType
    {
        public DeleveryType(int deliveryId, string name)
        {
            DeliveryId = deliveryId;
            Name = name;
        }

        public int DeliveryId { get; set; }

        public string Name { get; set; }
    }
}
