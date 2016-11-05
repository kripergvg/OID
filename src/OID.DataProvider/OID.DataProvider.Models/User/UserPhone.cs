namespace OID.DataProvider.Models.User
{
    public class UserPhone
    {
        public UserPhone(string number, string comment)
        {
            Comment = comment;
            Number = number;
        }

        public string Comment { get; set; }

        public string Number { get; set; }
    }
}
