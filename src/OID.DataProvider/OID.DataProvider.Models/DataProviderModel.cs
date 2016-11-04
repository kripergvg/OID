namespace OID.DataProvider.Models
{
    public class DataProviderModel<T> : DataProviderVoidModel
    {
        public DataProviderModel(ResultMessage resultMessage, T model)
            : base(resultMessage)
        {
            Model = model;
        }

        public T Model { get; set; }
    }
}
