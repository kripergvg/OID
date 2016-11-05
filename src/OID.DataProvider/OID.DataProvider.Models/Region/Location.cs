namespace OID.DataProvider.Models.Region
{
    public class Location
    {
        public Location(string code, string name)
        {
            Code = code;
            Name = name;
        }

        public string Code { get; set; }

        public string Name { get; set; }
    }
}
