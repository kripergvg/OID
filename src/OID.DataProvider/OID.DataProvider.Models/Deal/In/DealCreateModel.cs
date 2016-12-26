using System.Collections.Generic;
using OID.DataProvider.Models.User;

namespace OID.DataProvider.Models.Deal.In
{
    public class DealCreateModel
    {
        public DealCreateModel(double price, string comment, int? accountId, int deliveryTypeId, Size size, int weight, string cityCode, string localityCode,
            string regionCode, string address, DeleveryLocationType deleveryLocationType, List<int> objectIdsToAdd)
        {
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
            ObjectIdsToAdd = objectIdsToAdd;
        }

        public double Price { get; set; }

        public string Comment { get; set; }

        public int? AccountId { get; set; }

        public int DeliveryTypeId { get; set; }

        public Size Size { get; set; }

        public int Weight { get; set; }

        public string CityCode { get; set; }

        public string LocalityCode { get; set; }

        public string RegionCode { get; set; }

        public string Address { get; set; }

        public DeleveryLocationType DeleveryLocationType { get; set; }

        public List<int> ObjectIdsToAdd { get; set; }
    }
}