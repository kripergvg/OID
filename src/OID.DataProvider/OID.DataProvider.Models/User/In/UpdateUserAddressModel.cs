namespace OID.DataProvider.Models.User.In
{
    public class UpdateUserAddressModel
    {
        public UpdateUserAddressModel(string cityCode,
           string localityCode, string regionCode, DeleveryLocationType? deleveryLocationType, string address)
        {
            CityCode = cityCode;
            LocalityCode = localityCode;
            RegionCode = regionCode;
            DeleveryLocationType = deleveryLocationType;
            Address = address;
        }                                

        public string CityCode { get; set; }

        public string LocalityCode { get; set; }

        public string RegionCode { get; set; }

        public DeleveryLocationType? DeleveryLocationType { get; set; }

        public string Address { get; set; }
    }
}
