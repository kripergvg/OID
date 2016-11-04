using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace OID.LegacySoapService.Controllers
{
    public class QueryController : ApiController
    {
        // POST api/<controller>
        public HttpResponseMessage Post()
        {
            using (var client = new ServiceReference1.DataProviderSoapClient("DataProviderSoap"))
            {
                var queryBody = Request.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                var response =  client.Query(queryBody);
                return new HttpResponseMessage
                {
                    Content = new StringContent(response, Encoding.UTF8, "application/xml")
                };
            }
        }
    }
}