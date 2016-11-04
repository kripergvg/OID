using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using OID.DataProvider.Models;
using OID.DataProvider.Models.Session;
using OID.SoapDataProvider.SoapDataProvider;

namespace OID.SoapDataProvider
{
    public class DataProvider
    {
        private readonly string _login;
        private readonly string _password;
        private readonly ISoapServiceClient _serviceClient;

        public DataProvider(string login, string password, ISoapServiceClient serviceClient)
        {
            _login = login;
            _password = password;
            _serviceClient = serviceClient;
        }

        public string QueryListToXml(List<Query> listQuery, string RootElement, string Level1Element)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            xws.NewLineOnAttributes = true;
            xws.OmitXmlDeclaration = true;
            using (XmlWriter xw = XmlWriter.Create(sb, xws))
            {
                xw.WriteStartElement(RootElement);
                foreach (Query query in listQuery)
                {
                    query.ToXml(xw, Level1Element);
                }
                xw.WriteEndElement();
                xw.WriteEndDocument();
            }
            return sb.ToString();
        }

        /// <summary>
        /// Вызов запросов в технической сессии
        /// </summary>
        /// <param name="listQuery">Список запросов</param>
        /// <returns></returns>
        public string ExecuteQueriesInTechnicalSession(List<Query> listQuery)
        {
            List<Query> fulllistQuery = new List<Query>();

            string q_guid = Guid.NewGuid().ToString();
            Query q_first = new Query(q_guid, "AuthSession");
            Query q_last = new Query(Guid.NewGuid().ToString(), "CloseSession");
            MD5 md5pass = MD5.Create();
            q_first.Parameters.Add(new QueryParameter("in", "Login", _login));
            q_first.Parameters.Add(new QueryParameter("in", "PasswordHash", GetMd5Hash(md5pass, _password, true)));
            q_first.Parameters.Add(new QueryParameter("in", "SocAccount_Name", "Technical"));
            fulllistQuery.Add(q_first);

            foreach (Query q in listQuery)
            {
                q.ParentQueryGUID.Add(q_guid);
                fulllistQuery.Add(q);
                q_last.ParentQueryGUID.Add(q.GUID);
            }

            q_last.ParentQueryGUID.Add(q_guid);
            fulllistQuery.Add(q_last);

            string result = QueryListToXml(fulllistQuery, "queries", "query");

            return result;
        }

        //#region CallServices

        public async Task<DataProviderModel<AuthModel>> CallAuthSession(string email, string password)
        {          
            List<Query> listQuery = new List<Query>();

            string q_guid = Guid.NewGuid().ToString();
            Query q = new Query(q_guid, "AuthSession");
            MD5 md5pass = MD5.Create();
            q.Parameters.Add(new QueryParameter("in", "Login", email));
            q.Parameters.Add(new QueryParameter("in", "PasswordHash", GetMd5Hash(md5pass, password, true)));
            q.Parameters.Add(new QueryParameter("in", "SocAccount_Name", "Email"));
            listQuery.Add(q);

            string result = await _serviceClient.PostAsync(ExecuteQueriesInTechnicalSession(listQuery));

            string message=String.Empty;
            string code = "";
            List<Query> listResults = ParseQueryXml(result, "result", ref code, ref message);

            var model = new DataProviderModel<AuthModel>(message, Int32.Parse(code), null);

            foreach (Query q1 in listResults)
            {
                if (q1.Name == "AuthSession" && q1.Parameters.Exists(x => x.Code == "SocAccount_Name" && x.Value == "Email"))
                {
                    if (q1.Parameters.Exists(x => x.Code == "Session_Id" && x.Direction == "out"))
                    {
                        model.Model = new AuthModel(q1.Parameters.Find(x => x.Code == "UserName" && x.Direction == "out").Value,
                            q1.Parameters.Find(x => x.Code == "Session_Id" && x.Direction == "out").Value);
                    }
                }
            }

            return model;
        }

        //public  bool CallCheckSession(string Session_Id, bool isTechnical)
        //{
        //    string sisTechnical = "";
        //    if (isTechnical)
        //    {
        //        sisTechnical = "Y";
        //    }
        //    else
        //    {
        //        sisTechnical = "N";
        //    }

        //    DealWithDelivery.DataProviderSoapClient d = new DealWithDelivery.DataProviderSoapClient("DataProviderSoap");
        //    List<Query> listQuery = new List<Query>();

        //    string q_guid = Guid.NewGuid().ToString();
        //    Query q = new Query(q_guid, "CheckSession");
        //    q.Parameters.Add(new QueryParameter("in", "Session_Id", Session_Id));
        //    q.Parameters.Add(new QueryParameter("in", "isTechnicalQuery", sisTechnical));
        //    listQuery.Add(q);

        //    string result = d.Query(QueryListToXml(listQuery, "queries", "query"));

        //    d.Close();

        //    string code = "";
        //    string message = "";
        //    List<Query> x = ParseQueryXml(result, "result", ref code, ref message);

        //    if (code == "0")
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }

        //}

        public string CallCloseSession(string Session_Id, out string message)
        {
            string code = "";
            message = "";

            DealWithDelivery.DataProviderSoapClient d = new DealWithDelivery.DataProviderSoapClient("DataProviderSoap");
            List<Query> listQuery = new List<Query>();

            string q_guid = Guid.NewGuid().ToString();
            Query q = new Query(q_guid, "CloseSession");
            q.Parameters.Add(new QueryParameter("in", "Session_Id", Session_Id));
            listQuery.Add(q);

            string result = d.Query(QueryListToXml(listQuery, "queries", "query"));

            d.Close();

            List<Query> listResults = ParseQueryXml(result, "result", ref code, ref message);

            return code;
        }

        public string CallCreateUser(string Email, string UserName, string Password, out string message, out string Session_Id)
        {
            Session_Id = "";
            message = "";
            string code = "";

            DealWithDelivery.DataProviderSoapClient d = new DealWithDelivery.DataProviderSoapClient("DataProviderSoap");
            List<Query> listQuery = new List<Query>();

            string q_guid = Guid.NewGuid().ToString();
            Query q = new Query(q_guid, "CreateUser");
            MD5 md5pass = MD5.Create();
            q.Parameters.Add(new QueryParameter("in", "Email", Email));
            q.Parameters.Add(new QueryParameter("in", "UserName", UserName));
            //q.Parameters.Add(new QueryParameter("in", "BirthDate", BirthDate.Year.ToString() + "-" + ("0" + BirthDate.Month.ToString()).Substring(BirthDate.Month.ToString().Length - 1, 2) + "-" + ("0" + BirthDate.Day.ToString()).Substring(BirthDate.Day.ToString().Length - 1, 2)));
            q.Parameters.Add(new QueryParameter("in", "PasswordHash", GetMd5Hash(md5pass, Password, true)));
            q.Parameters.Add(new QueryParameter("in", "SocAccount_Name", "Email"));
            q.Parameters.Add(new QueryParameter("in", "isNeedToAuth", "Y"));
            listQuery.Add(q);

            string result = d.Query(ExecuteQueriesInTechnicalSession(listQuery));

            d.Close();

            List<Query> listResults = ParseQueryXml(result, "result", ref code, ref message);

            foreach (Query q1 in listResults)
            {
                if (q1.Name == "CreateUser")
                {
                    if (q1.Parameters.Exists(x => x.Code == "UserSession_Id"))
                    {
                        Session_Id = q1.Parameters.Find(x => x.Code == "UserSession_Id").Value;
                        break;
                    }

                }
            }

            return code;
        }

        //public  string CallmlUserActivate(out string message, string Session_Id)
        //{
        //    message = "";
        //    string code = "";

        //    DealWithDelivery.DataProviderSoapClient d = new DealWithDelivery.DataProviderSoapClient("DataProviderSoap");
        //    List<Query> listQuery = new List<Query>();

        //    string q_guid = Guid.NewGuid().ToString();
        //    Query q = new Query(q_guid, "mlUserActivate");
        //    q.Parameters.Add(new QueryParameter("in", "Session_Id", Session_Id, SqlDbType.NVarChar));
        //    listQuery.Add(q);

        //    string qq = QueryListToXml(listQuery, "queries", "query");
        //    string result = d.Query(qq);

        //    //string result = d.Query(ExecuteQueriesInTechnicalSession(listQuery));

        //    d.Close();

        //    List<Query> listResults = ParseQueryXml(result, "result", ref code, ref message);

        //    return code;
        //}

        //public  string CallUpsertObject(string Session_Id, OID_Web.Models.Object obj, out string message, out string Object_Id)
        //{
        //    Object_Id = "";
        //    message = "";
        //    string code = "";

        //    DealWithDelivery.DataProviderSoapClient d = new DealWithDelivery.DataProviderSoapClient("DataProviderSoap");
        //    List<Query> listQuery = new List<Query>();

        //    string q1_guid = Guid.NewGuid().ToString();
        //    Query q1 = new Query(q1_guid, "UpsertUserObject");
        //    q1.Parameters.Add(new QueryParameter("in", "ObjectCategory_Id", obj.ObjectCategory_Id, SqlDbType.Int));
        //    q1.Parameters.Add(new QueryParameter("in", "ObjectStatus_Id", obj.ObjectStatus_Id, SqlDbType.Int));
        //    q1.Parameters.Add(new QueryParameter("in", "ObjectName", obj.ObjectName, SqlDbType.NVarChar));
        //    if (obj.Description != null && obj.Description != "")
        //    {
        //        q1.Parameters.Add(new QueryParameter("in", "Description", obj.Description, SqlDbType.NVarChar));
        //    }
        //    q1.Parameters.Add(new QueryParameter("in", "Session_Id", Session_Id, SqlDbType.NVarChar));
        //    if (obj.Object_Id != null)
        //    {
        //        q1.Parameters.Add(new QueryParameter("in", "Object_Id", obj.Object_Id, SqlDbType.Int));
        //    }
        //    listQuery.Add(q1);

        //    string q2_guid = Guid.NewGuid().ToString();
        //    Query q2 = new Query(q2_guid, "UpsertCheckList");
        //    q2.ParentQueryGUID.Add(q1_guid);
        //    q2.Parameters.Add(new QueryParameter("in", "CheckListName", "Лист проверок", SqlDbType.NVarChar));
        //    q2.Parameters.Add(new QueryParameter("in", "Session_Id", Session_Id, SqlDbType.NVarChar));
        //    if (obj.CheckList_Id != null)
        //    {
        //        q2.Parameters.Add(new QueryParameter("in", "CheckList_Id", obj.CheckList_Id, SqlDbType.Int));
        //    }
        //    listQuery.Add(q2);

        //    foreach (OID_Web.Models.Check Check in obj.CheckList)
        //    {
        //        string q_guid = Guid.NewGuid().ToString();
        //        Query q;

        //        if (Check.Deleted == "1")
        //        {
        //            q = new Query(q_guid, "DeleteCheck");
        //            q.ParentQueryGUID.Add(q2_guid);
        //            q.Parameters.Add(new QueryParameter("in", "Session_Id", Session_Id, SqlDbType.NVarChar));
        //            q.Parameters.Add(new QueryParameter("in", "Check_Id", Check.Check_Id, SqlDbType.Int));
        //        }
        //        else
        //        {

        //            q = new Query(q_guid, "UpsertCheck");
        //            q.ParentQueryGUID.Add(q2_guid);
        //            q.Parameters.Add(new QueryParameter("in", "CheckType_Id", Check.CheckType_Id, SqlDbType.Int));
        //            q.Parameters.Add(new QueryParameter("in", "Task", Check.Task, SqlDbType.NVarChar));
        //            if (!String.IsNullOrWhiteSpace(Check.TaskLink))
        //            {
        //                q.Parameters.Add(new QueryParameter("in", "TaskLink", Check.TaskLink, SqlDbType.NVarChar));
        //            }
        //            q.Parameters.Add(new QueryParameter("in", "Session_Id", Session_Id, SqlDbType.NVarChar));
        //            if (Check.Check_Id != null)
        //            {
        //                q.Parameters.Add(new QueryParameter("in", "Check_Id", Check.Check_Id, SqlDbType.Int));
        //            }
        //        }
        //        listQuery.Add(q);
        //    }

        //    string result = d.Query(QueryListToXml(listQuery, "queries", "query"));

        //    d.Close();

        //    List<Query> listResults = ParseQueryXml(result, "result", ref code, ref message);

        //    foreach (Query q11 in listResults)
        //    {
        //        if (q11.Name == "UpsertUserObject")
        //        {
        //            if (q11.Parameters.Exists(x => x.Code == "Object_Id"))
        //            {
        //                Object_Id = q11.Parameters.Find(x => x.Code == "Object_Id").Value;
        //                break;
        //            }

        //        }
        //    }

        //    return code;
        //}

        //public  string CallUpsertAccounts(string Session_Id, OID_Web.Models.UserAccount a, out string message)
        //{
        //    message = "";
        //    string code = "";

        //    DealWithDelivery.DataProviderSoapClient d = new DealWithDelivery.DataProviderSoapClient("DataProviderSoap");
        //    List<Query> listQuery = new List<Query>();

        //    foreach (OID_Web.Models.Account acc in a.AccountList)
        //    {
        //        if (acc.Account_Id == null || acc.Deleted == "1")
        //        {

        //            listQuery.Add(AccountToQuery(Session_Id, acc));
        //        }
        //    }

        //    string result = d.Query(QueryListToXml(listQuery, "queries", "query"));

        //    d.Close();

        //    List<Query> listResults = ParseQueryXml(result, "result", ref code, ref message);

        //    return code;
        //}

        //private  Query AccountToQuery(string Session_Id, OID_Web.Models.Account acc)
        //{
        //    string q_guid = Guid.NewGuid().ToString();
        //    Query q;

        //    if (acc.Deleted == "1")
        //    {
        //        q = new Query(q_guid, "DeleteUserAccount");
        //        q.Parameters.Add(new QueryParameter("in", "Session_Id", Session_Id, SqlDbType.NVarChar));
        //        q.Parameters.Add(new QueryParameter("in", "UserAccount_Id", acc.UserAccount_Id, SqlDbType.Int));
        //    }
        //    else
        //    {
        //        q = new Query(q_guid, "InsertUserAccount");
        //        q.Parameters.Add(new QueryParameter("in", "Session_Id", Session_Id, SqlDbType.NVarChar));
        //        q.Parameters.Add(new QueryParameter("in", "CptyService_Id", acc.PaymentCptyService_Id, SqlDbType.Int));
        //        q.Parameters.Add(new QueryParameter("in", "AccountNumber", acc.AccountNumber, SqlDbType.NVarChar));
        //    }
        //    return q;
        //}
        //public  string CallUpsertDeal(string Session_Id, OID_Web.Models.Deal deal, out string message, out string Deal_Id)
        //{
        //    Deal_Id = "";
        //    message = "";
        //    string code = "";

        //    DealWithDelivery.DataProviderSoapClient d = new DealWithDelivery.DataProviderSoapClient("DataProviderSoap");
        //    string result = d.Query(QueryListToXml(GetDealQueryParam(Session_Id, deal), "queries", "query"));

        //    d.Close();

        //    List<Query> listResults = ParseQueryXml(result, "result", ref code, ref message);

        //    foreach (Query q11 in listResults)
        //    {
        //        if (q11.Name == "UpsertDeal")
        //        {
        //            if (q11.Parameters.Exists(x => x.Code == "Deal_Id"))
        //            {
        //                Deal_Id = q11.Parameters.Find(x => x.Code == "Deal_Id").Value;
        //                break;
        //            }

        //        }
        //    }

        //    return code;
        //}

        //private  List<Query> GetDealQueryParam(string Session_Id, OID_Web.Models.Deal deal)
        //{
        //    List<Query> listQuery = new List<Query>();

        //    string q1_guid = Guid.NewGuid().ToString();
        //    Query q1 = new Query(q1_guid, "UpsertDeal");
        //    q1.Parameters.Add(new QueryParameter("in", "BuySell", deal.BuySell, SqlDbType.Int));
        //    q1.Parameters.Add(new QueryParameter("in", "Price", deal.Price, SqlDbType.Decimal));
        //    q1.Parameters.Add(new QueryParameter("in", "Comment", deal.Comment, SqlDbType.NVarChar));
        //    q1.Parameters.Add(new QueryParameter("in", "Session_Id", Session_Id, SqlDbType.NVarChar));
        //    if (deal.Deal_Id != null)
        //    {
        //        q1.Parameters.Add(new QueryParameter("in", "Deal_Id", deal.Deal_Id, SqlDbType.Int));
        //    }

        //    if (deal.BuySell == "S")
        //    {
        //        //need to save account

        //        //account exist or need to create?
        //        if (deal.Account.Account_Id == null)
        //        {
        //            OID_Web.Models.Account acc = deal.Account;

        //            Query q = AccountToQuery(Session_Id, acc);
        //            listQuery.Add(q);
        //            q1.ParentQueryGUID.Add(q.GUID.ToString());
        //        }
        //        else
        //        {
        //            q1.Parameters.Add(new QueryParameter("in", "Account_Id", deal.Account.Account_Id, SqlDbType.Int));
        //        }
        //    }

        //    listQuery.Add(q1);

        //    foreach (OID_Web.Models.DealObject Object in deal.ObjectList)
        //    {
        //        string q2_guid = Guid.NewGuid().ToString();
        //        Query q;

        //        if (Object.Deleted == "1")
        //        {
        //            q = new Query(q2_guid, "DeleteDealObject");
        //            q.ParentQueryGUID.Add(q1_guid);
        //            q.Parameters.Add(new QueryParameter("in", "Session_Id", Session_Id, SqlDbType.NVarChar));
        //            q.Parameters.Add(new QueryParameter("in", "DealObject_Id", Object.DealObject_Id, SqlDbType.Int));
        //        }
        //        else
        //        {
        //            q = new Query(q2_guid, "UpsertDealObject");
        //            q.ParentQueryGUID.Add(q1_guid);
        //            q.Parameters.Add(new QueryParameter("in", "Object_Id", Object.Object_Id, SqlDbType.Int));
        //            q.Parameters.Add(new QueryParameter("in", "Session_Id", Session_Id, SqlDbType.NVarChar));
        //            if (Object.DealObject_Id != null)
        //            {
        //                q.Parameters.Add(new QueryParameter("in", "DealObject_Id", Object.DealObject_Id, SqlDbType.Int));
        //            }
        //        }
        //        listQuery.Add(q);
        //    }

        //    return listQuery;
        //}

        //public  string CallUpsertDealDelivery(string Session_Id, OID_Web.Models.Deal deal, OID_Web.Models.DealDeliveryModel dd, bool? isNeedToSavePhone, bool? isNeedToSaveAddress, bool? isNeedToSaveUserName, out string message, out string Deal_Id)
        //{
        //    Deal_Id = "";
        //    message = "";
        //    string code = "";

        //    DealWithDelivery.DataProviderSoapClient d = new DealWithDelivery.DataProviderSoapClient("DataProviderSoap");
        //    List<Query> listQuery = new List<Query>();

        //    string q1_guid = Guid.NewGuid().ToString();
        //    Query q1 = new Query(q1_guid, "UpsertDealDelivery");
        //    if (deal.Price != null)
        //    {
        //        listQuery = GetDealQueryParam(Session_Id, deal);
        //        q1.ParentQueryGUID.Add(listQuery.Find(x => x.Name == "UpsertDeal").GUID);
        //    }
        //    else
        //    {
        //        q1.Parameters.Add(new QueryParameter("in", "Deal_Id", dd.Deal_Id, SqlDbType.Int));
        //    }
        //    q1.Parameters.Add(new QueryParameter("in", "DeliveryCptyService_Id", dd.DeliveryCptyService_Id, SqlDbType.Int));
        //    q1.Parameters.Add(new QueryParameter("in", "Weight_Decl", dd.Weight_Decl, SqlDbType.Int));
        //    q1.Parameters.Add(new QueryParameter("in", "Length_Decl", dd.Length_Decl, SqlDbType.Int));
        //    q1.Parameters.Add(new QueryParameter("in", "Height_Decl", dd.Height_Decl, SqlDbType.Int));
        //    q1.Parameters.Add(new QueryParameter("in", "Width_Decl", dd.Width_Decl, SqlDbType.Int));

        //    if (dd.DeliveryLocationType == "City")
        //    {
        //        q1.Parameters.Add(new QueryParameter("in", "CityCode", dd.CityCode, SqlDbType.NVarChar));
        //    }
        //    else
        //    {
        //        q1.Parameters.Add(new QueryParameter("in", "CityCode", dd.LocationCode, SqlDbType.NVarChar));
        //    }
        //    q1.Parameters.Add(new QueryParameter("in", "Address", dd.Address, SqlDbType.NVarChar));
        //    q1.Parameters.Add(new QueryParameter("in", "Session_Id", Session_Id, SqlDbType.NVarChar));

        //    listQuery.Add(q1);

        //    if (isNeedToSavePhone == true)
        //    {
        //        GetPhoneSaveQuery(Session_Id, dd.PhoneMobile, null, "Mobile", ref listQuery);
        //    }

        //    if (isNeedToSaveAddress == true && !String.IsNullOrWhiteSpace(dd.Address)
        //        ||
        //        isNeedToSaveUserName == true && !String.IsNullOrWhiteSpace(dd.LastName)
        //        )
        //    {
        //        string q2_guid = Guid.NewGuid().ToString();
        //        Query q2 = new Query(q2_guid, "UpdateUser");
        //        if (isNeedToSaveAddress == true && !String.IsNullOrWhiteSpace(dd.Address))
        //        {
        //            q2.Parameters.Add(new QueryParameter("in", "Address", dd.Address, SqlDbType.NVarChar));
        //        }
        //        if (isNeedToSaveUserName == true && !String.IsNullOrWhiteSpace(dd.LastName))
        //        {
        //            q2.Parameters.Add(new QueryParameter("in", "LastName", dd.LastName, SqlDbType.NVarChar));
        //            q2.Parameters.Add(new QueryParameter("in", "FirstName", dd.FirstName, SqlDbType.NVarChar));
        //            q2.Parameters.Add(new QueryParameter("in", "SecondName", dd.SecondName, SqlDbType.NVarChar));
        //        }

        //        q2.Parameters.Add(new QueryParameter("in", "Session_Id", Session_Id, SqlDbType.NVarChar));
        //        listQuery.Add(q2);
        //    }

        //    string result = d.Query(QueryListToXml(listQuery, "queries", "query"));

        //    d.Close();

        //    List<Query> listResults = ParseQueryXml(result, "result", ref code, ref message);

        //    foreach (Query q11 in listResults)
        //    {
        //        if (q11.Name == "UpsertDealDelivery")
        //        {
        //            if (q11.Parameters.Exists(x => x.Code == "Deal_Id"))
        //            {
        //                Deal_Id = q11.Parameters.Find(x => x.Code == "Deal_Id").Value;
        //                break;
        //            }

        //        }
        //    }

        //    return code;
        //}

        //public  string CallUpdateDealDeliveryDecl(string Session_Id, OID_Web.Models.DealViewModel dd, out string message)
        //{
        //    message = "";
        //    string code = "";

        //    DealWithDelivery.DataProviderSoapClient d = new DealWithDelivery.DataProviderSoapClient("DataProviderSoap");
        //    List<Query> listQuery = new List<Query>();

        //    string q1_guid = Guid.NewGuid().ToString();
        //    Query q1 = new Query(q1_guid, "UpdateDealDeliveryDecl");

        //    q1.Parameters.Add(new QueryParameter("in", "Deal_Id", dd.Deal_Id, SqlDbType.Int));
        //    q1.Parameters.Add(new QueryParameter("in", "DeliveryCptyService_Id", dd.DeliveryCptyService_Id, SqlDbType.Int));
        //    q1.Parameters.Add(new QueryParameter("in", "Weight_Decl", dd.Weight_Decl, SqlDbType.Int));
        //    q1.Parameters.Add(new QueryParameter("in", "Length_Decl", dd.Length_Decl, SqlDbType.Int));
        //    q1.Parameters.Add(new QueryParameter("in", "Height_Decl", dd.Height_Decl, SqlDbType.Int));
        //    q1.Parameters.Add(new QueryParameter("in", "Width_Decl", dd.Width_Decl, SqlDbType.Int));

        //    q1.Parameters.Add(new QueryParameter("in", "Session_Id", Session_Id, SqlDbType.NVarChar));

        //    listQuery.Add(q1);

        //    string result = d.Query(QueryListToXml(listQuery, "queries", "query"));

        //    d.Close();

        //    List<Query> listResults = ParseQueryXml(result, "result", ref code, ref message);

        //    return code;
        //}

        //public  string CallUpsertUserContact(string Session_Id, OID_Web.Models.UserContactModel model, OID_Web.Models.UserContactModel old_model, out string message)
        //{
        //    message = "";
        //    string code = "";

        //    DealWithDelivery.DataProviderSoapClient d = new DealWithDelivery.DataProviderSoapClient("DataProviderSoap");
        //    List<Query> listQuery = new List<Query>();

        //    GetPhoneSaveQuery(Session_Id, model.PhoneMobile, old_model.PhoneMobile, "Mobile", ref listQuery);
        //    GetPhoneSaveQuery(Session_Id, model.PhoneWork, old_model.PhoneWork, "Work", ref listQuery);
        //    GetPhoneSaveQuery(Session_Id, model.PhoneHome, old_model.PhoneHome, "Home", ref listQuery);
        //    GetPhoneSaveQuery(Session_Id, model.PhoneAdditional, old_model.PhoneAdditional, "Additional", ref listQuery);

        //    if ((model.DeliveryLocationType == "City" && model.CityCode != null)
        //        || (model.DeliveryLocationType == "Location" && model.LocalityCode != null)
        //        || !String.IsNullOrWhiteSpace(model.Address))
        //    {
        //        string q1_guid = Guid.NewGuid().ToString();
        //        Query q1 = new Query(q1_guid, "UpdateUser");
        //        if (model.Address != null && model.Address.Length > 0)
        //        {
        //            q1.Parameters.Add(new QueryParameter("in", "Address", model.Address, SqlDbType.NVarChar));
        //        }
        //        else
        //        {
        //            q1.Parameters.Add(new QueryParameter("in", "Address", "", SqlDbType.NVarChar));
        //        }

        //        if (model.DeliveryLocationType == "City" && model.CityCode != null)
        //        {
        //            q1.Parameters.Add(new QueryParameter("in", "CityCode", model.CityCode, SqlDbType.NVarChar));
        //        }
        //        if (model.DeliveryLocationType == "Location" && model.LocalityCode != null)
        //        {
        //            q1.Parameters.Add(new QueryParameter("in", "CityCode", model.LocationCode, SqlDbType.NVarChar));
        //        }


        //        q1.Parameters.Add(new QueryParameter("in", "Session_Id", Session_Id, SqlDbType.NVarChar));
        //        listQuery.Add(q1);
        //    }

        //    string result = d.Query(QueryListToXml(listQuery, "queries", "query"));

        //    d.Close();

        //    List<Query> listResults = ParseQueryXml(result, "result", ref code, ref message);

        //    return code;
        //}

        //public  string CallUpdateUser(string Session_Id, OID_Web.Models.UserProfileModel model, OID_Web.Models.UserProfileModel prev_model, out string message)
        //{
        //    message = "";
        //    string code = "";

        //    DealWithDelivery.DataProviderSoapClient d = new DealWithDelivery.DataProviderSoapClient("DataProviderSoap");
        //    List<Query> listQuery = new List<Query>();

        //    string q1_guid = Guid.NewGuid().ToString();
        //    Query q1 = new Query(q1_guid, "UpdateUser");

        //    if ((model.LastName ?? "") != (prev_model.LastName ?? ""))
        //    {
        //        q1.Parameters.Add(new QueryParameter("in", "LastName", model.LastName ?? "", SqlDbType.NVarChar));
        //    }
        //    if ((model.FirstName ?? "") != (prev_model.FirstName ?? ""))
        //    {
        //        q1.Parameters.Add(new QueryParameter("in", "FirstName", model.FirstName ?? "", SqlDbType.NVarChar));
        //    }
        //    if ((model.SecondName ?? "") != (prev_model.SecondName ?? ""))
        //    {
        //        q1.Parameters.Add(new QueryParameter("in", "SecondName", model.SecondName ?? "", SqlDbType.NVarChar));
        //    }
        //    if ((model.BirthDate ?? "") != (prev_model.BirthDate.PadRight(10).Substring(0, 10) ?? ""))
        //    {
        //        q1.Parameters.Add(new QueryParameter("in", "BirthDate", model.BirthDate, SqlDbType.DateTime));
        //    }
        //    if ((model.UserName ?? "") != (prev_model.UserName ?? ""))
        //    {
        //        q1.Parameters.Add(new QueryParameter("in", "UserName", model.UserName, SqlDbType.NVarChar));
        //    }

        //    q1.Parameters.Add(new QueryParameter("in", "Session_Id", Session_Id, SqlDbType.NVarChar));
        //    listQuery.Add(q1);

        //    string result = d.Query(QueryListToXml(listQuery, "queries", "query"));

        //    d.Close();

        //    List<Query> listResults = ParseQueryXml(result, "result", ref code, ref message);

        //    return code;
        //}

        //public  string CallUpdateUser_ChangePassword(string Session_Id, OID_Web.Models.UserProfileModel model, out string message)
        //{
        //    message = "";
        //    string code = "";

        //    DealWithDelivery.DataProviderSoapClient d = new DealWithDelivery.DataProviderSoapClient("DataProviderSoap");
        //    List<Query> listQuery = new List<Query>();

        //    string q1_guid = Guid.NewGuid().ToString();
        //    Query q1 = new Query(q1_guid, "UpdateUser_ChangePassword");

        //    MD5 md5pass = MD5.Create();
        //    q1.Parameters.Add(new QueryParameter("in", "OldPasswordHash", GetMd5Hash(md5pass, model.OldPassword, true), SqlDbType.NVarChar));
        //    q1.Parameters.Add(new QueryParameter("in", "NewPasswordHash", GetMd5Hash(md5pass, model.NewPassword, true), SqlDbType.NVarChar));

        //    q1.Parameters.Add(new QueryParameter("in", "Session_Id", Session_Id, SqlDbType.NVarChar));
        //    listQuery.Add(q1);

        //    string result = d.Query(QueryListToXml(listQuery, "queries", "query"));

        //    d.Close();

        //    List<Query> listResults = ParseQueryXml(result, "result", ref code, ref message);

        //    return code;
        //}

        //public  string CallDealAccept(string Session_Id, OID_Web.Models.DealAcceptModel model, bool? isNeedToSavePhone, bool? isNeedToSaveAddress, bool? isNeedToSaveUserName, out string message)
        //{
        //    message = "";
        //    string code = "";

        //    DealWithDelivery.DataProviderSoapClient d = new DealWithDelivery.DataProviderSoapClient("DataProviderSoap");
        //    List<Query> listQuery = new List<Query>();

        //    string q1_guid = Guid.NewGuid().ToString();
        //    Query q1 = new Query(q1_guid, "DealAccept");
        //    if (model.BuySell != null)
        //    {
        //        q1.Parameters.Add(new QueryParameter("in", "BS", model.BuySell, SqlDbType.NVarChar));
        //    }
        //    q1.Parameters.Add(new QueryParameter("in", "Deal_Id", model.Deal_Id, SqlDbType.Int));
        //    q1.Parameters.Add(new QueryParameter("in", "Pin", model.Pin, SqlDbType.Int));
        //    if (model.DeliveryLocationType == "City")
        //    {
        //        q1.Parameters.Add(new QueryParameter("in", "CityCode", model.CityCode, SqlDbType.NVarChar));
        //    }
        //    else
        //    {
        //        q1.Parameters.Add(new QueryParameter("in", "CityCode", model.LocationCode, SqlDbType.NVarChar));
        //    }
        //    q1.Parameters.Add(new QueryParameter("in", "Address", model.Address, SqlDbType.NVarChar));
        //    q1.Parameters.Add(new QueryParameter("in", "Session_Id", Session_Id, SqlDbType.NVarChar));

        //    if (isNeedToSavePhone == true)
        //    {
        //        GetPhoneSaveQuery(Session_Id, model.PhoneMobile, null, "Mobile", ref listQuery);
        //    }

        //    if (isNeedToSaveAddress == true && !String.IsNullOrWhiteSpace(model.Address)
        //        ||
        //        isNeedToSaveUserName == true && !String.IsNullOrWhiteSpace(model.LastName)
        //        )
        //    {
        //        string q2_guid = Guid.NewGuid().ToString();
        //        Query q2 = new Query(q2_guid, "UpdateUser");
        //        if (isNeedToSaveAddress == true && !String.IsNullOrWhiteSpace(model.Address))
        //        {
        //            q2.Parameters.Add(new QueryParameter("in", "Address", model.Address, SqlDbType.NVarChar));
        //        }
        //        if (isNeedToSaveUserName == true && !String.IsNullOrWhiteSpace(model.LastName))
        //        {
        //            q2.Parameters.Add(new QueryParameter("in", "LastName", model.LastName, SqlDbType.NVarChar));
        //            q2.Parameters.Add(new QueryParameter("in", "FirstName", model.FirstName, SqlDbType.NVarChar));
        //            q2.Parameters.Add(new QueryParameter("in", "SecondName", model.SecondName, SqlDbType.NVarChar));
        //        }
        //        q2.Parameters.Add(new QueryParameter("in", "Session_Id", Session_Id, SqlDbType.NVarChar));
        //        listQuery.Add(q2);
        //    }

        //    if (model.BuySell == "S")
        //    {
        //        //need to save account

        //        //account exist or need to create?
        //        if (model.AccountInputType == "New")
        //        {
        //            OID_Web.Models.Account acc = new Account();
        //            acc.GUID = Guid.NewGuid();
        //            acc.Deleted = "0";
        //            acc.AccountNumber = model.AccountNumber;
        //            acc.PaymentCptyService_Id = model.PaymentCptyService_Id;

        //            Query q = AccountToQuery(Session_Id, acc);
        //            listQuery.Add(q);
        //            q1.ParentQueryGUID.Add(q.GUID.ToString());
        //        }
        //        else
        //        {
        //            q1.Parameters.Add(new QueryParameter("in", "Account_Id", model.Account_Id, SqlDbType.Int));
        //        }
        //    }
        //    listQuery.Add(q1);

        //    string result = d.Query(QueryListToXml(listQuery, "queries", "query"));

        //    d.Close();

        //    List<Query> listResults = ParseQueryXml(result, "result", ref code, ref message);

        //    return code;
        //}

        //public  string CallDealObjectApprove(string Session_Id, OID_Web.Models.DealObject model, out string message)
        //{
        //    message = "";
        //    string code = "";

        //    DealWithDelivery.DataProviderSoapClient d = new DealWithDelivery.DataProviderSoapClient("DataProviderSoap");
        //    List<Query> listQuery = new List<Query>();

        //    string q1_guid = Guid.NewGuid().ToString();
        //    Query q1 = new Query(q1_guid, "DealObjectApprove");
        //    q1.Parameters.Add(new QueryParameter("in", "Session_Id", Session_Id, SqlDbType.NVarChar));
        //    q1.Parameters.Add(new QueryParameter("in", "DealObject_Id", model.DealObject_Id, SqlDbType.Int));
        //    q1.Parameters.Add(new QueryParameter("in", "Approve", "Y", SqlDbType.NVarChar));
        //    listQuery.Add(q1);

        //    string result = d.Query(QueryListToXml(listQuery, "queries", "query"));

        //    d.Close();

        //    List<Query> listResults = ParseQueryXml(result, "result", ref code, ref message);

        //    return code;
        //}

        //public  string CallDealApprove(string Session_Id, OID_Web.Models.DealViewModel model, out string message)
        //{
        //    message = "";
        //    string code = "";

        //    DealWithDelivery.DataProviderSoapClient d = new DealWithDelivery.DataProviderSoapClient("DataProviderSoap");
        //    List<Query> listQuery = new List<Query>();

        //    string q1_guid = Guid.NewGuid().ToString();
        //    Query q1 = new Query(q1_guid, "DealApprove");
        //    q1.Parameters.Add(new QueryParameter("in", "Session_Id", Session_Id, SqlDbType.NVarChar));
        //    q1.Parameters.Add(new QueryParameter("in", "Deal_Id", model.Deal_Id, SqlDbType.Int));
        //    q1.Parameters.Add(new QueryParameter("in", "Approve", "Y", SqlDbType.NVarChar));
        //    listQuery.Add(q1);

        //    string result = d.Query(QueryListToXml(listQuery, "queries", "query"));

        //    d.Close();

        //    List<Query> listResults = ParseQueryXml(result, "result", ref code, ref message);

        //    return code;
        //}

        //public  string CallDealLeave(string Session_Id, string Deal_Id, out string message)
        //{
        //    message = "";
        //    string code = "";

        //    DealWithDelivery.DataProviderSoapClient d = new DealWithDelivery.DataProviderSoapClient("DataProviderSoap");
        //    List<Query> listQuery = new List<Query>();

        //    string q1_guid = Guid.NewGuid().ToString();
        //    Query q1 = new Query(q1_guid, "DealLeave");
        //    q1.Parameters.Add(new QueryParameter("in", "Session_Id", Session_Id, SqlDbType.NVarChar));
        //    q1.Parameters.Add(new QueryParameter("in", "Deal_Id", Deal_Id, SqlDbType.Int));
        //    listQuery.Add(q1);

        //    string result = d.Query(QueryListToXml(listQuery, "queries", "query"));

        //    d.Close();

        //    List<Query> listResults = ParseQueryXml(result, "result", ref code, ref message);

        //    return code;
        //}

        //public  string CallExecutePayment(string Session_Id, OID_Web.Models.Payment p, ref string PaymentStatus_Code, out string PaymentStatus_Name, out string message)
        //{
        //    message = "";
        //    PaymentStatus_Name = "";

        //    if (p.Date == null || p.Date == "")
        //    {
        //        p.Date = System.DateTime.Now.Date.Year.ToString()
        //            + "-" + ("0" + System.DateTime.Now.Date.Month.ToString()).Substring(System.DateTime.Now.Date.Month.ToString().Length - 1, 2)
        //            + "-" + ("0" + System.DateTime.Now.Date.Day.ToString()).Substring(System.DateTime.Now.Date.Day.ToString().Length - 1, 2)
        //            + " " + System.DateTime.Now.ToShortTimeString();
        //    }
        //    else
        //    if (p.Date.Contains("."))
        //    {
        //        p.Date = p.Date.Substring(6, 4) + "-" + p.Date.Substring(3, 2) + p.Date.Substring(0, 2) + " " + p.Date.Substring(11);
        //    }

        //    string code = "";

        //    DealWithDelivery.DataProviderSoapClient d = new DealWithDelivery.DataProviderSoapClient("DataProviderSoap");
        //    List<Query> listQuery = new List<Query>();

        //    string q1_guid = Guid.NewGuid().ToString();
        //    p.TransactionNum = p.BillNumber;

        //    Query q1 = new Query(q1_guid, "ExecutePayment");
        //    q1.Parameters.Add(new QueryParameter("in", "Session_Id", Session_Id, SqlDbType.NVarChar));
        //    q1.Parameters.Add(new QueryParameter("in", "Payment_Id", p.Payment_Id, SqlDbType.Int));
        //    q1.Parameters.Add(new QueryParameter("in", "Date", p.Date, SqlDbType.DateTime));
        //    q1.Parameters.Add(new QueryParameter("in", "CptyService_Id", p.CptyService_Id, SqlDbType.Int));
        //    q1.Parameters.Add(new QueryParameter("inout", "PaymentStatus_Code", PaymentStatus_Code, SqlDbType.Int));

        //    if (PaymentStatus_Code == "Executed")
        //    {
        //        q1.Parameters.Add(new QueryParameter("in", "TransactionNum", p.TransactionNum, SqlDbType.NVarChar));
        //        q1.Parameters.Add(new QueryParameter("in", "PaymentInfo", p.PaymentInfo, SqlDbType.NVarChar));

        //        if (!String.IsNullOrWhiteSpace(p.Comment))
        //        {
        //            q1.Parameters.Add(new QueryParameter("in", "Comment", p.Comment, SqlDbType.NVarChar));
        //        }
        //        if (!String.IsNullOrWhiteSpace(p.Password))
        //        {
        //            q1.Parameters.Add(new QueryParameter("in", "Password", p.Password, SqlDbType.NVarChar));
        //        }
        //    }
        //    listQuery.Add(q1);

        //    string result = d.Query(QueryListToXml(listQuery, "queries", "query"));

        //    d.Close();

        //    List<Query> listResults = ParseQueryXml(result, "result", ref code, ref message);

        //    foreach (Query q11 in listResults)
        //    {
        //        if (q11.Name == "ExecutePayment")
        //        {
        //            if (q11.Parameters.Exists(x => x.Code == "PaymentStatus_Code"))
        //            {
        //                PaymentStatus_Code = q11.Parameters.Find(x => x.Code == "PaymentStatus_Code").Value;
        //            }
        //            if (q11.Parameters.Exists(x => x.Code == "PaymentStatus_Name"))
        //            {
        //                PaymentStatus_Name = q11.Parameters.Find(x => x.Code == "PaymentStatus_Name").Value;
        //            }
        //            break;
        //        }
        //    }

        //    return code;
        //}



        //public  string CallDeleteService(string serviceName, List<QueryParameter> qpl, out string message)
        //{
        //    string code = "";
        //    message = "";

        //    DealWithDelivery.DataProviderSoapClient d = new DealWithDelivery.DataProviderSoapClient("DataProviderSoap");
        //    List<Query> listQuery = new List<Query>();

        //    string q_guid = Guid.NewGuid().ToString();
        //    Query q = new Query(q_guid, serviceName);
        //    q.Parameters = qpl;
        //    listQuery.Add(q);

        //    string result = "";

        //    result = d.Query(QueryListToXml(listQuery, "queries", "query"));

        //    d.Close();

        //    List<Query> listResults = ParseQueryXml(result, "result", ref code, ref message);

        //    return code;
        //}

        //public  string CallGetService(string serviceName, bool isTechnical, out string message, out DataTable dt)
        //{
        //    List<QueryParameter> qpl = new List<QueryParameter>();
        //    return CallGetService(serviceName, qpl, isTechnical, out message, out dt);
        //}

        //public  string CallGetService(string serviceName, List<QueryParameter> qpl, bool isTechnical, out string message, out DataTable dt)
        //{
        //    string code = "";
        //    message = "";
        //    dt = new DataTable();

        //    DealWithDelivery.DataProviderSoapClient d = new DealWithDelivery.DataProviderSoapClient("DataProviderSoap");
        //    List<Query> listQuery = new List<Query>();

        //    string q_guid = Guid.NewGuid().ToString();
        //    Query q = new Query(q_guid, serviceName);
        //    q.Parameters = qpl;
        //    listQuery.Add(q);

        //    string result = "";

        //    if (isTechnical)
        //    {
        //        result = d.Query(ExecuteQueriesInTechnicalSession(listQuery));
        //    }
        //    else
        //    {
        //        result = d.Query(QueryListToXml(listQuery, "queries", "query"));
        //    }

        //    d.Close();

        //    List<Query> listResults = ParseQueryXml(result, "result", ref code, ref message);

        //    if (code == "0")
        //    {
        //        foreach (Query q1 in listResults)
        //        {
        //            if (q1.Name == serviceName)
        //            {
        //                dt = q1.RetTable;
        //                break;
        //            }
        //        }

        //    }
        //    return code;
        //}

        //#endregion


        public string GetValue(DataTable dt, string key, string keyvalue, string fkey)
        {
            foreach (DataRow dr in dt.Rows)
            {
                if (dr[key].ToString() == keyvalue)
                {
                    return dr[fkey].ToString();
                }
            }
            return null;
        }

        public string GetMd5Hash(MD5 md5Hash, string input, bool Add0x)
        {

            // Convert the input string to a byte array and compute the hash. 
            byte[] data = md5Hash.ComputeHash(Encoding.Default.GetBytes(input));

            string s = ByteArrayToString(data);

            // Return the hexadecimal string.
            if (Add0x)
            {
                return "0x" + s.ToString();
            }
            else
            {
                return s.ToString();
            }
        }

        public string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
        /*
        public  byte[] StringToByteArray(String hex)
        {

            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];

            //byte[] bytes = Encoding.Default.GetBytes(hex);
            
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
        */
        public List<Query> ParseQueryXml(string queryAsXml, string Level1Element, ref string code, ref string message)
        {
            int count = 0;
            List<Query> queries = new List<Query>();

            using (StringReader sr = new StringReader(queryAsXml))
            {
                XmlReader reader = XmlReader.Create(sr);
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);

                foreach (XmlNode node in doc.GetElementsByTagName("status"))
                {
                    foreach (XmlAttribute attr in node.Attributes)
                    {
                        if (attr.Name == "code")
                        {
                            code = attr.Value;
                        }

                        if (attr.Name == "message")
                        {
                            message = attr.Value;
                        }
                    }
                }

                foreach (XmlNode node in doc.GetElementsByTagName(Level1Element))
                {
                    Query query;
                    string QueryGUID = "";
                    string Name = "";
                    string QueryResult = "";
                    string QueryMessage = "";

                    foreach (XmlAttribute attr in node.Attributes)
                    {
                        if (attr.Name == "QueryGUID")
                        {
                            QueryGUID = attr.Value;
                        }

                        if (attr.Name == "Name")
                        {
                            Name = attr.Value;
                        }

                        if (attr.Name == "QueryResultCode")
                        {
                            QueryResult = attr.Value;
                        }

                        if (attr.Name == "QueryResultMessage")
                        {
                            QueryMessage = attr.Value;
                        }
                    }

                    if (QueryGUID != "" && Name != "")
                    {
                        query = new Query(QueryGUID, Name);
                        query.QueryResultCode = Convert.ToInt32(QueryResult);
                        query.QueryResultMessage = QueryMessage;
                        queries.Add(query);
                        count++;
                    }
                    else
                    {
                        throw new Exception("Invalid query param: QueryGUID or Name not found");
                    }

                    foreach (XmlNode node2 in node.ChildNodes)
                    {
                        if (node2.Name == "param")
                        {
                            foreach (XmlNode param in node2.ChildNodes)
                            {
                                foreach (XmlAttribute a in param.Attributes)
                                {
                                    string dir = "";
                                    if (param.Name == "input")
                                    {
                                        dir = "in";
                                    }
                                    if (param.Name == "output")
                                    {
                                        dir = "out";
                                    }
                                    if (param.Name == "inherited")
                                    {
                                        dir = "inh";
                                    }
                                    query.Parameters.Add(new QueryParameter(dir, a.Name, a.Value));
                                }
                            }
                        }
                        if (node2.Name == "parent")
                        {
                            XmlAttribute a = node2.Attributes["QueryGUID"];
                            if (a != null)
                            {
                                query.ParentQueryGUID.Add(a.Value);
                            }
                        }
                        if (node2.Name == "rettable")
                        {
                            DataTable dt = new DataTable();

                            foreach (XmlNode row in node2.ChildNodes)
                            {
                                if (dt.Rows.Count == 0)
                                {
                                    foreach (XmlAttribute a in row.Attributes)
                                    {
                                        dt.Columns.Add(a.Name);
                                    }
                                }
                                List<string> arr = new List<string>();
                                foreach (XmlAttribute a in row.Attributes)
                                {
                                    arr.Add(a.Value);
                                }

                                dt.Rows.Add(arr.ToArray());
                            }

                            query.RetTable = dt;
                        }
                    }
                }
            }
            if (count == 0)
            {
                throw new Exception("Queries not found");
            }

            return queries;
        }

        public string GetAttributeValue(Type t, string Attribute, string Argument)
        {
            string s = "";
            IList<CustomAttributeData> list_attr = t.GetProperty(Attribute).GetCustomAttributesData();

            foreach (CustomAttributeData attr in list_attr)
            {
                if (attr.AttributeType.Name == "DisplayAttribute")
                {
                    foreach (CustomAttributeNamedArgument arg in attr.NamedArguments)
                    {
                        if (arg.MemberName == Argument)
                        {
                            s = arg.TypedValue.Value.ToString();
                        }
                    }
                }
            }

            return s;
        }

        //public  List<SelectListItem> DataTableToListSelectListItem(DataTable dt, string IdColumn, string NameColumn)
        //{
        //    List<SelectListItem> items = new List<SelectListItem>();

        //    foreach (DataRow dr in dt.Rows)
        //    {

        //        SelectListItem item = new SelectListItem();
        //        item.Value = dr[IdColumn].ToString();
        //        item.Text = dr[NameColumn].ToString();

        //        items.Add(item);
        //    }
        //    return items;
        //}

        public List<T> DataTableToList<T>(DataTable dt) where T : new()
        {
            var items = new List<T>();

            var properties = typeof(T).GetProperties().Where(p => p.CanWrite && dt.Columns.Contains(p.Name)).ToArray();

            foreach (DataRow dr in dt.Rows)
            {
                var item = new T();
                foreach (var p in properties)
                {
                    if (p.PropertyType.FullName == typeof(DateTime).FullName && dr[p.Name].ToString() == "")
                    {
                        p.SetValue(item, DateTime.MinValue.AddYears(1969));
                    }
                    else
                    {
                        p.SetValue(item, dr[p.Name]);
                    }
                }
                items.Add(item);
            }

            return items;
        }

        public string GetPhoneOnlyDigits(string s)
        {
            return s.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Replace("+", "");
        }

        //public  void GetPhoneSaveQuery(string Session_Id, UserPhone phone, UserPhone old_phone, string PhoneType, ref List<Query> listQuery)
        //{
        //    string guid = Guid.NewGuid().ToString();

        //    if (phone != null)
        //    {
        //        if (old_phone != null && String.IsNullOrWhiteSpace(phone.Number))
        //        {
        //            //Delete phone
        //            Query q2 = new Query(guid, "DeleteUserPhone");
        //            q2.Parameters.Add(new QueryParameter("in", "PhoneType_Name", PhoneType, SqlDbType.NVarChar));
        //            q2.Parameters.Add(new QueryParameter("in", "PhoneNumber", GetPhoneOnlyDigits(old_phone.Number), SqlDbType.Int));
        //            q2.Parameters.Add(new QueryParameter("in", "Session_Id", Session_Id, SqlDbType.NVarChar));

        //            listQuery.Add(q2);
        //        }
        //        else if (!String.IsNullOrWhiteSpace(phone.Number))
        //        {
        //            Query q1 = new Query(guid, "UpsertUserPhone");
        //            q1.Parameters.Add(new QueryParameter("in", "PhoneType_Name", PhoneType, SqlDbType.NVarChar));
        //            q1.Parameters.Add(new QueryParameter("in", "PhoneNumber", GetPhoneOnlyDigits(phone.Number), SqlDbType.Int));
        //            q1.Parameters.Add(new QueryParameter("in", "Comment", phone.Comment, SqlDbType.NVarChar));
        //            q1.Parameters.Add(new QueryParameter("in", "Session_Id", Session_Id, SqlDbType.NVarChar));

        //            listQuery.Add(q1);

        //        }
        //    }
        //}

        //public  string SaveImage(string ClientId, HttpPostedFileBase file)
        //{
        //    //var imgService = new ImgUrImageService();
        //    byte[] fileBytes = new byte[file.ContentLength];
        //    file.InputStream.Read(fileBytes, 0, fileBytes.Length);
        //    file.InputStream.Close();
        //    string fileContent = Convert.ToBase64String(fileBytes);
        //    return UploadImage(ClientId, fileContent);
        //}

        //public  string UploadImage(string ClientId, string imageAsBase64String)
        //{
        //    string result = "";
        //    using (var webClient = new WebClient())
        //    {
        //        webClient.Headers.Add("Authorization", "Client-ID " + ClientId);
        //        //webClient.Proxy = defaultWebProxy;
        //        var values = new NameValueCollection
        //            {
        //                { "image", imageAsBase64String }
        //            };
        //        byte[] response = webClient.UploadValues("https://api.imgur.com/3/image", values);
        //        result = Encoding.ASCII.GetString(response);
        //        System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("link\":\"(.*?)\"");
        //        System.Text.RegularExpressions.Match match = reg.Match(result);
        //        string url = match.ToString().Replace("link\":\"", "").Replace("\"", "").Replace("\\/", "/");
        //        return url;
        //    }
        //}



        //public class MyPolicy : ICertificatePolicy
        //{
        //    public bool CheckValidationResult(
        //          ServicePoint srvPoint
        //        , X509Certificate certificate
        //        , WebRequest request
        //        , int certificateProblem)
        //    {

        //        //Return True to force the certificate to be accepted.
        //        return true;

        //    } // end CheckValidationResult
        //} // class MyPolicy

        //public  string Protect(string text, string purpose)
        //{
        //    if (string.IsNullOrEmpty(text))
        //        return null;

        //    byte[] stream = Encoding.UTF8.GetBytes(text);
        //    byte[] encodedValue = MachineKey.Protect(stream, purpose);
        //    return HttpServerUtility.UrlTokenEncode(encodedValue);
        //}

        //public  string Unprotect(string text, string purpose)
        //{
        //    if (string.IsNullOrEmpty(text))
        //        return null;

        //    byte[] stream = HttpServerUtility.UrlTokenDecode(text);
        //    byte[] decodedValue = MachineKey.Unprotect(stream, purpose);
        //    return Encoding.UTF8.GetString(decodedValue);
        //}
    }
}

