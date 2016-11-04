using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OID.SoapDataProvider;
using OID.SoapDataProvider.SoapDataProvider;

namespace ConsoleApp1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var dp = new DataProvider("Web@OplataIDostavka.ru", "TechnicalPassword", () => new WcfClient<ISoapDataProvider>("http://oplataidostavka.ru/DealWithDelivery/DataProvider.asmx"));

            //var client = new WcfClient<ISoapDataProvider>("http://oplataidostavka.ru/DealWithDelivery/DataProvider.asmx");
            //client.Client.Query(
            //    new QueryRequest(
            //        new QueryRequestBody(
            //            "<string xmlns=\"http://www.oplataidostavka.ru/DataProvider/\"><?xml version=\"1.0\" encoding=\"utf-16\"?>< results> <result QueryGUID=\"AE1EFC62-EF7F-4C96-8599-4108056590CF\" Name=\"AuthSession\" QueryResultCode=\"0\"> <input Login=\"yukhnenko.ao@mail.ru\" PasswordHash=\"0xDCF6CCBB7AF39F0C9FEE92B346EBC1C5\" SocAccount_Id=\"10\" /> <output Session_Id=\"D4D09C9C-8051-40E7-B7E5-6DA4D1BEB43F\" /> </result>< /results></string>")));

            var c = new WcfClient<ISoapDataProvider>("http://oplataidostavka.ru/DealWithDelivery/DataProvider.asmx");
           var r= c.Client.Query(new QueryRequest(new QueryRequestBody(@"<queries>
  <query
    QueryGUID=""553ea9e6-0963-4cab-8257-bd835e1ac3bf""
    Name=""AuthSession""
    QueryResultCode=""-1""
    QueryResultMessage="""">
    <param>
      <input
        Login=""Web@OplataIDostavka.ru""
        PasswordHash=""0x936c339af5a524d20fc1749786a53244""
        SocAccount_Name=""Technical"" />
      <inherited />
      <output />
    </param>
  </query>
  <query
    QueryGUID=""ff331aed-edfc-4da3-85a9-77abc0884d02""
    Name=""AuthSession""
    QueryResultCode=""-1""
    QueryResultMessage="""">
    <parent
      QueryGUID=""553ea9e6-0963-4cab-8257-bd835e1ac3bf"" />
    <param>
      <input
        Login=""wkololo_4ever@mail.ru""
        PasswordHash=""0x05a671c66aefea124cc08b76ea6d30bb""
        SocAccount_Name=""Email"" />
      <inherited />
      <output />
    </param>
  </query>
  <query
    QueryGUID=""696a381c-94fd-4052-87cd-5b02cfc1551e""
    Name=""CloseSession""
    QueryResultCode=""-1""
    QueryResultMessage="""">
    <parent
      QueryGUID=""ff331aed-edfc-4da3-85a9-77abc0884d02"" />
    <parent
      QueryGUID=""553ea9e6-0963-4cab-8257-bd835e1ac3bf"" />
    <param>
      <input />
      <inherited />
      <output />
    </param>
  </query>
</queries>")));

            string message;
            string code;
            string sesionID;
            string userName;
            dp.CallAuthSession("yukhnenko.ao@mail.ru", "0xDCF6CCBB7AF39F0C9FEE92B346EBC1C5", out message, out userName, out sesionID);
        }
    }
}
