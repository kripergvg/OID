namespace OID.DataProvider.Models.Region
{
    public class Locality
    {
        public Locality(string code, string name)
        {
            Code = code;
            Name = name;
        }

        public string Code { get; set; }

        public string Name { get; set; }
    }
}
