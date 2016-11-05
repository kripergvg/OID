using System.Collections.Generic;

namespace OID.DataProvider.Models.Object.In
{
    public class DealObject
    {
        public DealObject(string objectId, string objectCategoryId, string objectStatusId, string objectName, string description, string checkListId, bool deleted)
        {
            ObjectId = objectId;
            ObjectCategoryId = objectCategoryId;
            ObjectStatusId = objectStatusId;
            ObjectName = objectName;
            Description = description;
            CheckListId = checkListId;
            Deleted = deleted;
        }

        public string ObjectId { get; }

        public string ObjectCategoryId { get; }

        public string ObjectStatusId { get; }

        public string ObjectName { get; }

        public string Description { get; }

        public string CheckListId { get; }

        public bool Deleted { get; set; }

        public IEnumerable<DealCheck> CheckList { get; set; }

        public class DealCheck
        {
            public string CheckId { get; set; }

            public string CheckTypeId { get; set; }

            public string Task { get; set; }

            public string TaskLink { get; set; }

            public bool Deleted { get; set; }
        }
    }
}
