using System;

namespace OID.DataProvider.Models.User.In
{
    public class UserProfileModel
    {
        public string UserName { get; set; }
        
        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string SecondName { get; set; }

        public DateTime? BirthDate { get; set; }
    }
}
