//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using NUnit.Framework;
//using OID.SoapDataProvider.SoapDataProvider;

//namespace OID.SoapDataProvider.Integration.Tests
//{
//    [TestFixture]
//    public class WcfClientTests
//    {
//        [Test]
//        public void Test()
//        {
//            var dp=new DataProvider("Web@OplataIDostavka.ru", "TechnicalPassword",()=> new WcfClient<ISoapDataProvider>("http://oplataidostavka.ru/DealWithDelivery/DataProvider.asmx"));

//            //var client = new WcfClient<ISoapDataProvider>("http://oplataidostavka.ru/DealWithDelivery/DataProvider.asmx");
//            //client.Client.Query(
//            //    new QueryRequest(
//            //        new QueryRequestBody(
//            //            "<string xmlns=\"http://www.oplataidostavka.ru/DataProvider/\"><?xml version=\"1.0\" encoding=\"utf-16\"?>< results> <result QueryGUID=\"AE1EFC62-EF7F-4C96-8599-4108056590CF\" Name=\"AuthSession\" QueryResultCode=\"0\"> <input Login=\"yukhnenko.ao@mail.ru\" PasswordHash=\"0xDCF6CCBB7AF39F0C9FEE92B346EBC1C5\" SocAccount_Id=\"10\" /> <output Session_Id=\"D4D09C9C-8051-40E7-B7E5-6DA4D1BEB43F\" /> </result>< /results></string>")));

//            string message;
//            string code;
//            string sesionID;
//            string userName;
//            dp.CallAuthSession("yukhnenko.ao@mail.ru", "0xDCF6CCBB7AF39F0C9FEE92B346EBC1C5", out message, out userName, out sesionID);
//        }
//    }
//}
