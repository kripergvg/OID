using System.Threading.Tasks;

namespace OID.SoapDataProvider.SoapServiceClient
{
    public interface ISoapServiceClient
    {
        Task<string> PostAsync(string queryBody);
    }
}
