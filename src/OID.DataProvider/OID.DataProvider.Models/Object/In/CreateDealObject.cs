using System.Collections.Generic;

namespace OID.DataProvider.Models.Object.In
{
    public class CreateDealObject
    {
        public CreateDealObject(string objectCategoryId, string objectStatusId, string objectName, string description, IEnumerable<DealCheck> checks)
        {
            ObjectCategoryId = objectCategoryId;
            ObjectStatusId = objectStatusId;
            ObjectName = objectName;
            Description = description;
            CheckList = checks;
        }

        public string ObjectCategoryId { get; }

        public string ObjectStatusId { get; }

        public string ObjectName { get; }

        public string Description { get; }

        public IEnumerable<DealCheck> CheckList { get; set; }
    }
}
