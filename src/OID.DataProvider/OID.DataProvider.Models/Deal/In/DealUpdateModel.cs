using System.Collections.Generic;
using OID.DataProvider.Models.User;

namespace OID.DataProvider.Models.Deal.In
{
    public class DealUpdateModel
    {
        public DealUpdateModel(int dealId, List<int> dealObjectIdsToDelete, List<int> objectIdsToAdd, double price, string comment, int accountId, int deliveryTypeId,
            Size size, int weight, string cityCode, string localityCode, string regionCode, string address, DeleveryLocationType deleveryLocationType)
        {
            DealId = dealId;
            DealObjectIdsToDelete = dealObjectIdsToDelete;
            ObjectIdsToAdd = objectIdsToAdd;
            Price = price;
            Comment = comment;
            AccountId = accountId;
            DeliveryTypeId = deliveryTypeId;
            Size = size;
            Weight = weight;
            CityCode = cityCode;
            LocalityCode = localityCode;
            RegionCode = regionCode;
            Address = address;
            DeleveryLocationType = deleveryLocationType;
        }

        public int DealId { get; set; }

        public List<int> DealObjectIdsToDelete { get; set; }

        public List<int> ObjectIdsToAdd { get; set; }

        public double Price { get; set; }

        public string Comment { get; set; }

        public int AccountId { get; set; }

        public int DeliveryTypeId { get; set; }

        public Size Size { get; set; }

        public int Weight { get; set; }

        public string CityCode { get; set; }

        public string LocalityCode { get; set; }

        public string RegionCode { get; set; }

        public string Address { get; set; }

        public DeleveryLocationType DeleveryLocationType { get; set; }
    }
}
