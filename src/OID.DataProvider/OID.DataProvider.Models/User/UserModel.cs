using System;

namespace OID.DataProvider.Models.User
{
    public class UserModel
    {
        public UserModel(string userId, string email, string userName, string lastName, string firstName, string secondName, DateTime? birthDate, string cityCode,
            string localityCode, string regionCode, DeleveryLocationType? deleveryLocationType, string address, DateTime createDate, bool blocked)
        {
            UserId = userId;
            Email = email;
            UserName = userName;
            LastName = lastName;
            FirstName = firstName;
            SecondName = secondName;
            BirthDate = birthDate;
            CityCode = cityCode;
            LocalityCode = localityCode;
            RegionCode = regionCode;
            DeleveryLocationType = deleveryLocationType;
            Address = address;
            CreateDate = createDate;
            Blocked = blocked;
        }

        public string UserId { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string SecondName { get; set; }

        public DateTime? BirthDate { get; set; }

        public string CityCode { get; set; }

        public string LocalityCode { get; set; }

        public string RegionCode { get; set; }

        public DeleveryLocationType? DeleveryLocationType { get; set; }

        public string Address { get; set; }

        public DateTime CreateDate { get; set; }

        public bool Blocked { get; set; }
    }
}
