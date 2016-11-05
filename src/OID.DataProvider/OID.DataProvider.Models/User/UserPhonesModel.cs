namespace OID.DataProvider.Models.User
{
    public class UserPhonesModel
    {
        public UserPhonesModel(UserPhone mobile, UserPhone work, UserPhone home, UserPhone additional)
        {
            Mobile = mobile;
            Work = work;
            Home = home;
            Additional = additional;
        }

        public UserPhone Mobile { get; set; }

        public UserPhone Work { get; set; }

        public UserPhone Home { get; set; }

        public UserPhone Additional { get; set; }
    }
}
