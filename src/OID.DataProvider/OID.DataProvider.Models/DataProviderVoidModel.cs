namespace OID.DataProvider.Models
{
    public class DataProviderVoidModel
    {
        public DataProviderVoidModel(ResultMessage resultMessage)
        {
            ResultMessage = resultMessage;
        }

        public ResultMessage ResultMessage { get; }
    }
}
