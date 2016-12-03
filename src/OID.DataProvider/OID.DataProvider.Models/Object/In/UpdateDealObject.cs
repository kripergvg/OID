namespace OID.DataProvider.Models.Object.In
{
    public class UpdateDealObject
    {
        public UpdateDealObject(string objectId, string objectCategoryId, string objectStatusId, string objectName, string description)
        {
            ObjectId = objectId;
            ObjectCategoryId = objectCategoryId;
            ObjectStatusId = objectStatusId;
            ObjectName = objectName;
            Description = description;
        }

        public string ObjectId { get; set; }

        public string ObjectCategoryId { get; }

        public string ObjectStatusId { get; }

        public string ObjectName { get; }

        public string Description { get; }
    }
}
