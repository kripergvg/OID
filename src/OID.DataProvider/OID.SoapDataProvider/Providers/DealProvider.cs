using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using OID.Core;
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

        public async Task<DataProviderVoidModel> Approve(string dealId)
        {
            var listQuery = new List<Query>();

            var q1_guid = Guid.NewGuid().ToString();
            var q1 = new Query(q1_guid, "DealApprove");
            q1.Parameters.Add(new QueryParameter("in", "Deal_Id", dealId, SqlDbType.Int));
            q1.Parameters.Add(new QueryParameter("in", "Approve", "Y", SqlDbType.NVarChar));
            listQuery.Add(q1);

            var result = await _sessionQueryExecutor.Execute(listQuery).ConfigureAwait(false);

            return new DataProviderVoidModel(result.ResultMessage);
        }

        public async Task<DataProviderVoidModel> Leave(string dealId)
        {
            var listQuery = new List<Query>();

            var q1_guid = Guid.NewGuid().ToString();
            var q1 = new Query(q1_guid, "DealLeave");
            q1.Parameters.Add(new QueryParameter("in", "Deal_Id", dealId, SqlDbType.Int));
            listQuery.Add(q1);

            var result = await _sessionQueryExecutor.Execute(listQuery).ConfigureAwait(false);

            return new DataProviderVoidModel(result.ResultMessage);
        }

        public async Task<DataProviderVoidModel> UpdateDelevery(DeleveryUpdateModel deleveryModel)
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

            var result = await _sessionQueryExecutor.Execute(listQuery).ConfigureAwait(false);

            return new DataProviderVoidModel(result.ResultMessage);
        }

        public async Task<DataProviderModel<PaymentStatusModel>> ExecutePayment(ExecutePaymentModel paymentModel, string paymentStatus)
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

            var result = await _sessionQueryExecutor.Execute(listQuery).ConfigureAwait(false);

            var model = new DataProviderModel<PaymentStatusModel>(result.ResultMessage);

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
    }
}
