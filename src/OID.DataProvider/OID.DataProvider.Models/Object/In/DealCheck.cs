namespace OID.DataProvider.Models.Object.In
{
    public class DealCheck
    {
        public DealCheck(string checkTypeId, string task, string checkId)
        {
            CheckTypeId = checkTypeId;
            Task = task;
            CheckId = checkId;
        }

        public DealCheck(string checkTypeId, string task)
        {
            CheckTypeId = checkTypeId;
            Task = task;
        }

        public string CheckTypeId { get; set; }

        public string Task { get; set; }

        public string CheckId { get; set; }
    }
}
