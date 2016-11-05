namespace OID.DataProvider.Models.Region
{
    public class City
    {
        public City(string code, string name)
        {
            Code = code;
            Name = name;
        }

        public string Code { get; set; }

        public string Name { get; set; }
    }
}
