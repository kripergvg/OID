namespace OID.DataProvider.Models.Deal
{
    public class UpsertDealModel 
    {
        public UpsertDealModel(string dealId)
        {
            DealId = dealId;
        }

        public string DealId { get; }
    }
}
