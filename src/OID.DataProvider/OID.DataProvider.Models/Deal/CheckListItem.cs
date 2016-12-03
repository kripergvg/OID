namespace OID.DataProvider.Models.Deal
{
    public class CheckListItem
    {
        public CheckListItem(string checkId, string checkListId, string task, CheckType checkType, string comment)
        {
            CheckId = checkId;
            CheckListId = checkListId;
            Task = task;
            CheckType = checkType;
            Comment = comment;
        }

        public string CheckId { get; set; }

        public string CheckListId { get; set; }

        public string Task { get; set; }

        public CheckType CheckType { get; set; }

        public string Comment { get; set; }
    }

    public enum CheckType
    {
        Function = 1,

        Condition = 2,

        Equipment = 3,

        Custom = 4
    }
}
