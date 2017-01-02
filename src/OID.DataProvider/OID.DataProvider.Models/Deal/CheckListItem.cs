using System;

namespace OID.DataProvider.Models.Deal
{
    public class CheckListItem
    {
        public CheckListItem(int checkId, int checkListId, string task, CheckType checkType, string checkComment,
            string checkLink, DateTime createDate, DateTime changeDate, bool blocked)
        {
            CheckId = checkId;
            CheckListId = checkListId;
            Task = task;
            CheckType = checkType;
            CheckComment = checkComment;
            CheckLink = checkLink;
            CreateDate = createDate;
            ChangeDate = changeDate;
            Blocked = blocked;
        }

        public int CheckId { get; set; }

        public int CheckListId { get; set; }

        public string Task { get; set; }

        public CheckType CheckType { get; set; }        

        public string CheckComment { get; set; }

        public string CheckLink { get; set; }        

        public DateTime CreateDate { get; set; }

        public DateTime ChangeDate { get; set; }

        public bool Blocked { get; set; }
    }

    public enum CheckType
    {
        Function = 1,

        Condition = 2,

        Equipment = 3,

        Custom = 4
    }
}