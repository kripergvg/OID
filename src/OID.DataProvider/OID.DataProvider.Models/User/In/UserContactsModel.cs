namespace OID.DataProvider.Models.User.In
{
    public class UserContactsModel
    {
        public UserPhone PhoneHome { get; set; }
        
        public UserPhone PhoneWork { get; set; }
        
        public UserPhone PhoneMobile { get; set; }
        
        public UserPhone PhoneAdditional { get; set; }
        
        public string Address { get; set; }
        
        public string LocalityCode { get; set; }
        
        public string CityCode { get; set; }
        
        public string LocationCode { get; set; }

        public string DeliveryLocationType { get; set; }
    }
}
