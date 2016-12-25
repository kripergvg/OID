namespace OID.Web.Models.Partials
{
    public class PartialModel
    {
        public PartialModel(string modelPrefix)
        {
            ModelPrefix = modelPrefix;
        }

        public string ModelPrefix { get; }
    }
}
