namespace OID.DataProvider.Models
{
    public class DataProviderModel<T> : DataProviderVoidModel where T : class
    {
        public DataProviderModel(ResultMessage resultMessage, T model = null)
            : base(resultMessage)
        {
            Model = model;
        }

        public T Model { get; set; }
    }
}
