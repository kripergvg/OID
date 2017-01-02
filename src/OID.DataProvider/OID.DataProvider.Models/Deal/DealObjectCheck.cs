using System;

namespace OID.DataProvider.Models.Deal
{
    public class DealObjectCheck : CheckListItem
    {
        public DealObjectCheck(int checkId, int checkListId, string task, CheckType checkType, string checkComment, string checkLink, DateTime createDate,
            DateTime changeDate, bool blocked, CheckStatus checkStatus, int objectId) 
            : base(checkId, checkListId, task, checkType, checkComment, checkLink, createDate, changeDate, blocked)
        {
            CheckStatus = checkStatus;
            ObjectId = objectId;
        }

        public CheckStatus CheckStatus { get; set; }

        public int ObjectId { get; set; }
    }
}
