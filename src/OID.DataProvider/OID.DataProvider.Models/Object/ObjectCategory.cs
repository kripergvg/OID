namespace OID.DataProvider.Models.Object
{
    public class ObjectCategory
    {
        public ObjectCategory(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; set; }

        public string Name { get; set; }
    }
}
