using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using OID.DataProvider.Interfaces;
using OID.DataProvider.Models;
using OID.DataProvider.Models.Deal;
using OID.DataProvider.Models.Deal.In;
using OID.SoapDataProvider.Providers.Infrastructure;

namespace OID.SoapDataProvider.Providers
{
    public class DealProvider : IDealProvider
    {
        private readonly IUserSessionQueryExecutorDecorator _sessionQueryExecutor;

        public DealProvider(IUserSessionQueryExecutorDecorator sessionQueryExecutor)
        {
            _sessionQueryExecutor = sessionQueryExecutor;
        }

        public async Task<DataSessionProviderVoidModel> Approve(UserModel userModel, string dealId)
        {
            var listQuery = new List<Query>();

            var q1_guid = Guid.NewGuid().ToString();
            var q1 = new Query(q1_guid, "DealApprove");
            q1.Parameters.Add(new QueryParameter("in", "Deal_Id", dealId, SqlDbType.Int));
            q1.Parameters.Add(new QueryParameter("in", "Approve", "Y", SqlDbType.NVarChar));
            listQuery.Add(q1);

            var result = await _sessionQueryExecutor.Execute(listQuery, userModel).ConfigureAwait(false);

            return new DataSessionProviderVoidModel(result.ResultMessage, result.SessionId);
        }

        public async Task<DataSessionProviderVoidModel> Leave(UserModel userModel, string dealId)
        {
            var listQuery = new List<Query>();

            var q1_guid = Guid.NewGuid().ToString();
            var q1 = new Query(q1_guid, "DealLeave");
            q1.Parameters.Add(new QueryParameter("in", "Deal_Id", dealId, SqlDbType.Int));
            listQuery.Add(q1);

            var result = await _sessionQueryExecutor.Execute(listQuery, userModel).ConfigureAwait(false);

            return new DataSessionProviderVoidModel(result.ResultMessage, result.SessionId);
        }

        public async Task<DataSessionProviderVoidModel> UpdateDelevery(UserModel userModel, DeleveryUpdateModel deleveryModel)
        {
            var listQuery = new List<Query>();

            var q1_guid = Guid.NewGuid().ToString();
            var q1 = new Query(q1_guid, "UpdateDealDeliveryDecl");

            q1.Parameters.Add(new QueryParameter("in", "Deal_Id", deleveryModel.DealId, SqlDbType.Int));
            q1.Parameters.Add(new QueryParameter("in", "DeliveryCptyService_Id", deleveryModel.DeleveruCptyServiceId, SqlDbType.Int));
            q1.Parameters.Add(new QueryParameter("in", "Weight_Decl", deleveryModel.Weight, SqlDbType.Int));
            q1.Parameters.Add(new QueryParameter("in", "Length_Decl", deleveryModel.Length, SqlDbType.Int));
            q1.Parameters.Add(new QueryParameter("in", "Height_Decl", deleveryModel.Height, SqlDbType.Int));
            q1.Parameters.Add(new QueryParameter("in", "Width_Decl", deleveryModel.Width, SqlDbType.Int));

            listQuery.Add(q1);

            var result = await _sessionQueryExecutor.Execute(listQuery, userModel).ConfigureAwait(false);

            return new DataSessionProviderVoidModel(result.ResultMessage, result.SessionId);
        }

        public async Task<DataSessionProviderModel<PaymentStatusModel>> ExecutePayment(UserModel userModel, ExecutePaymentModel paymentModel, string paymentStatus)
        {
            var date = paymentModel.Date ?? DateTime.Now;

            var listQuery = new List<Query>();

            var q1_guid = Guid.NewGuid().ToString();
            var transactionNum = paymentModel.BillNumber;

            var q1 = new Query(q1_guid, "ExecutePayment");
            q1.Parameters.Add(new QueryParameter("in", "Payment_Id", paymentModel.PaymentId, SqlDbType.Int));
            //TODO возможно нужно изменить формат даты
            q1.Parameters.Add(new QueryParameter("in", "Date", date.ToString(), SqlDbType.DateTime));
            q1.Parameters.Add(new QueryParameter("in", "CptyService_Id", paymentModel.CptyServiceId, SqlDbType.Int));
            q1.Parameters.Add(new QueryParameter("inout", "PaymentStatus_Code", paymentStatus, SqlDbType.Int));

            if (paymentStatus == "Executed")
            {
                q1.Parameters.Add(new QueryParameter("in", "TransactionNum", transactionNum, SqlDbType.NVarChar));
                q1.Parameters.Add(new QueryParameter("in", "PaymentInfo", paymentModel.Info, SqlDbType.NVarChar));

                if (!String.IsNullOrWhiteSpace(paymentModel.Comment))
                {
                    q1.Parameters.Add(new QueryParameter("in", "Comment", paymentModel.Comment, SqlDbType.NVarChar));
                }
                if (!String.IsNullOrWhiteSpace(paymentModel.Password))
                {
                    q1.Parameters.Add(new QueryParameter("in", "Password", paymentModel.Password, SqlDbType.NVarChar));
                }
            }
            listQuery.Add(q1);

            var result = await _sessionQueryExecutor.Execute(listQuery, userModel).ConfigureAwait(false);

            var model = new DataSessionProviderModel<PaymentStatusModel>(result.ResultMessage, null, result.SessionId);

            foreach (var query in result.Queries)
            {
                if (query.Name == "ExecutePayment")
                {
                    string statusCode = String.Empty;
                    string statusName = String.Empty;

                    if (query.Parameters.Exists(x => x.Code == "PaymentStatus_Code"))
                    {
                        statusCode = query.Parameters.Find(x => x.Code == "PaymentStatus_Code").Value;
                    }

                    if (query.Parameters.Exists(x => x.Code == "PaymentStatus_Name"))
                    {
                        statusName = query.Parameters.Find(x => x.Code == "PaymentStatus_Name").Value;
                    }

                    model.Model = new PaymentStatusModel(statusCode, statusName);

                    return model;
                }
            }

            return model;
        }

        //public async Task<DataSessionProviderModel<UpsertDealModel>> Upsert(UserModel userModel, UpsertDealModelIn upsertDealModel)
        //{
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
        //}

        //private List<Query> GetDealQueryParam(string Session_Id, UpsertDealModelIn deal)
        //{
        //    List<Query> listQuery = new List<Query>();

        //    string q1_guid = Guid.NewGuid().ToString();
        //    Query q1 = new Query(q1_guid, "UpsertDeal");
        //    q1.Parameters.Add(new QueryParameter("in", "BuySell", deal.BuySell, SqlDbType.Int));
        //    q1.Parameters.Add(new QueryParameter("in", "Price", deal.Price, SqlDbType.Decimal));
        //    q1.Parameters.Add(new QueryParameter("in", "Comment", deal.Comment, SqlDbType.NVarChar));
        //    q1.Parameters.Add(new QueryParameter("in", "Session_Id", Session_Id, SqlDbType.NVarChar));
        //    if (deal.DealId != null)
        //    {
        //        q1.Parameters.Add(new QueryParameter("in", "Deal_Id", deal.DealId, SqlDbType.Int));
        //    }

        //    if (deal.BuySell == "S")
        //    {
        //        //need to save account

        //        //account exist or need to create?
        //        if (deal.UserAccount.AccountId == null)
        //        {
        //            var acc = deal.UserAccount;

        //            Query q = AccountToQuery(acc);
        //            listQuery.Add(q);
        //            q1.ParentQueryGUID.Add(q.GUID.ToString());
        //        }
        //        else
        //        {
        //            q1.Parameters.Add(new QueryParameter("in", "Account_Id", deal.UserAccount.AccountId, SqlDbType.Int));
        //        }
        //    }

        //    listQuery.Add(q1);

        //    foreach (var Object in deal.ObjectList)
        //    {
        //        string q2_guid = Guid.NewGuid().ToString();
        //        Query q;

        //        if (Object.Deleted)
        //        {
        //            q = new Query(q2_guid, "DeleteDealObject");
        //            q.ParentQueryGUID.Add(q1_guid);
        //            q.Parameters.Add(new QueryParameter("in", "Session_Id", Session_Id, SqlDbType.NVarChar));
        //            q.Parameters.Add(new QueryParameter("in", "DealObject_Id", Object.DealObjectId, SqlDbType.Int));
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

        //private Query AccountToQuery(UpsertDealModelIn.Account acc)
        //{
        //    string q_guid = Guid.NewGuid().ToString();
        //    Query q;

        //    if (acc.Deleted)
        //    {
        //        q = new Query(q_guid, "DeleteUserAccount");
        //        q.Parameters.Add(new QueryParameter("in", "UserAccount_Id", acc.UserAccountId, SqlDbType.Int));
        //    }
        //    else
        //    {
        //        q = new Query(q_guid, "InsertUserAccount");
        //        q.Parameters.Add(new QueryParameter("in", "CptyService_Id", acc.PaymentCptyServiceId, SqlDbType.Int));
        //        q.Parameters.Add(new QueryParameter("in", "AccountNumber", acc.AccountNumber, SqlDbType.NVarChar));
        //    }
        //    return q;
        //}
    }
}
