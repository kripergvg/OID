namespace OID.DataProvider.Models.Object
{
    public class UserObject
    {
        public UserObject(string objectId, string name, string description, string categoryName, string categoryId, string statusName, string statusId, bool blocked,
            string checkListId)
        {
            ObjectId = objectId;
            Name = name;
            Description = description;
            CategoryName = categoryName;
            CategoryId = categoryId;
            StatusName = statusName;
            StatusId = statusId;
            Blocked = blocked;
            CheckListId = checkListId;
        }

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
