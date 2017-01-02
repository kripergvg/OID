using System;
using OID.DataProvider.Models.User;

namespace OID.DataProvider.Models.Deal
{
    public class DealDelevery
    {
        public DealDelevery(int dealId, int deliveryCptyServiceId, string deliveryCptyServiceName, Size sizeDeclire, Size sizeFact, int weightDeclire, int? weightFact,
            int? trackingNumber, double? deliveryPriceCalculated, double? deliveryPriceFact, double? insurancePriceCalculated, double? insurancePriceFact, string address,
            string addressFrom, string addressTo, string cityCodeFrom, string cityCodeTo, string cityCode, string localityCode, string regionCode,
            DeleveryLocationType deleveryLocationType, DateTime createDate, DateTime changeDate, string currencyCode, bool blocked,string buyerUserName,
            string sellerUserName)
        {
            DealId = dealId;
            DeliveryCptyServiceId = deliveryCptyServiceId;
            DeliveryCptyServiceName = deliveryCptyServiceName;
            SizeDeclire = sizeDeclire;
            SizeFact = sizeFact;
            WeightDeclire = weightDeclire;
            WeightFact = weightFact;
            TrackingNumber = trackingNumber;
            DeliveryPriceCalculated = deliveryPriceCalculated;
            DeliveryPriceFact = deliveryPriceFact;
            InsurancePriceCalculated = insurancePriceCalculated;
            InsurancePriceFact = insurancePriceFact;
            Address = address;
            AddressFrom = addressFrom;
            AddressTo = addressTo;
            CityCodeFrom = cityCodeFrom;
            CityCodeTo = cityCodeTo;
            CityCode = cityCode;
            LocalityCode = localityCode;
            RegionCode = regionCode;
            DeleveryLocationType = deleveryLocationType;
            CreateDate = createDate;
            ChangeDate = changeDate;
            CurrencyCode = currencyCode;
            Blocked = blocked;
            BuyerUserName = buyerUserName;
            SellerUserName = sellerUserName;
        }

        public int DealId { get; set; }

        public int DeliveryCptyServiceId { get; set; }

        public string DeliveryCptyServiceName { get; set; }

        public Size SizeDeclire { get; set; }

        public Size SizeFact { get; set; }

        public int WeightDeclire { get; set; }

        public int? WeightFact { get; set; }

        public int? TrackingNumber { get; set; }

        public double? DeliveryPriceCalculated { get; set; }

        public double? DeliveryPriceFact { get; set; }

        public double? InsurancePriceCalculated { get; set; }

        public double? InsurancePriceFact { get; set; }

        public string Address { get; set; }

        public string AddressFrom { get; set; }

        public string AddressTo { get; set; }

        public string CityCodeFrom { get; set; }

        public string CityCodeTo { get; set; }

        public string CityCode { get; set; }

        public string LocalityCode { get; set; }

        public string RegionCode { get; set; }

        public DeleveryLocationType DeleveryLocationType { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime ChangeDate { get; set; }

        public string CurrencyCode { get; set; }

        public bool Blocked { get; set; }

        public string BuyerUserName { get; set; }

        public string SellerUserName { get; set; }
    }
}
