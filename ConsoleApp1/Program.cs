using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OID.SoapDataProvider;
using OID.SoapDataProvider.SoapDataProvider;
using OID.SoapDataProvider.SoapServiceClient;

namespace ConsoleApp1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var dt = new DataProvider("Web@OplataIDostavka.ru", "!533244w", new SoapServiceClient("http://localhost:58680/api/query"));
            var resul = dt.CallAuthSession("wkololo_4ever@mail.ru", "testtest").Result;
        }
    }
}
