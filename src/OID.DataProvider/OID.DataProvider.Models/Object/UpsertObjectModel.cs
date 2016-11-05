namespace OID.DataProvider.Models.Object
{
    public class UpsertObjectModel 
    {
        public UpsertObjectModel(string objectId)
        {
            ObjectId = objectId;
        }

        public string ObjectId { get; }
        
    }
}
