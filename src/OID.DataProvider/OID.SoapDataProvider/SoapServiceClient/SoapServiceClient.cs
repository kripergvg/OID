using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OID.SoapDataProvider.SoapServiceClient
{
    public class SoapServiceClient : ISoapServiceClient
    {
        private readonly string _soapServiceUrl;
        private const int MILLESECONDS_TIMEOUT = 15000;

        public SoapServiceClient(string soapServiceUrl)
        {
            _soapServiceUrl = soapServiceUrl;
        }

        public async Task<string> PostAsync(string queryBody)
        {
            using (var client = new HttpClient())
            {
                var cts = new CancellationTokenSource();
                cts.CancelAfter(MILLESECONDS_TIMEOUT);
                var response = await client.PostAsync(_soapServiceUrl, new StringContent(queryBody), cts.Token).ConfigureAwait(false);
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
        }
    }
}
