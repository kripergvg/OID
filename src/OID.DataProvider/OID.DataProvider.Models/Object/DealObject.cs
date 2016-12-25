using System;

namespace OID.DataProvider.Models.Object
{
    public class DealObject
    {
        public DealObject(int dealObjectId, int dealId, int objectId, string objectName, string description, int userId, int checkListId, bool isApprovedByPartner,
            bool isApprovedByMe, DateTime createDate, DateTime changeDate, int checkStatusId, bool blocked, int objectCategoryId, int objectStatusId)
        {
            DealObjectId = dealObjectId;
            DealId = dealId;
            ObjectId = objectId;
            ObjectName = objectName;
            Description = description;
            UserId = userId;
            CheckListId = checkListId;
            IsApprovedByPartner = isApprovedByPartner;
            IsApprovedByMe = isApprovedByMe;
            CreateDate = createDate;
            ChangeDate = changeDate;
            CheckStatusId = checkStatusId;
            Blocked = blocked;
            ObjectCategoryId = objectCategoryId;
            ObjectStatusId = objectStatusId;
        }

        public int DealObjectId { get; set; }

        public int DealId { get; set; }

        public int ObjectId { get; set; }

        public string ObjectName { get; set; }

        public string Description { get; set; }

        public int UserId { get; set; }

        public int CheckListId { get; set; }

        public bool IsApprovedByPartner { get; set; }

        public bool IsApprovedByMe { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime ChangeDate { get; set; }

        public int CheckStatusId { get; set; }

        public bool Blocked { get; set; }

        public int ObjectCategoryId { get; set; }

        public int ObjectStatusId { get; set; }
    }
}
