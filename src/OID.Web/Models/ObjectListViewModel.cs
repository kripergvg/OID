namespace OID.Web.Models
{
    public class ObjectListViewModel
    {
        public string ObjectId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string CategoryName { get; set; }

        public string CategoryId { get; set; }

        public string StatusName { get; set; }

        public string StatusId { get; set; }

        public bool Blocked { get; set; }

        public string CheckListId { get; set; }
    }
}
