using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using OID.Core;
using OID.DataProvider.Interfaces;
using OID.DataProvider.Models;
using OID.DataProvider.Models.Deal;
using OID.DataProvider.Models.Deal.In;
using OID.DataProvider.Models.User;
using OID.SoapDataProvider.Providers.Infrastructure;

namespace OID.SoapDataProvider.Providers
{
    public class DealProvider : IDealProvider
    {
        private readonly IUserSessionQueryExecutorDecorator _sessionQueryExecutor;
        private readonly ISoapParser _soapParser;
        private readonly IAppQueryExecutorDecorator _appQueryExecutorDecorator;

        public DealProvider(
            IUserSessionQueryExecutorDecorator sessionQueryExecutor,
            ISoapParser soapParser,
            IAppQueryExecutorDecorator appQueryExecutorDecorator)
        {
            _sessionQueryExecutor = sessionQueryExecutor;
            _soapParser = soapParser;
            _appQueryExecutorDecorator = appQueryExecutorDecorator;
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

        public async Task<DataProviderModel<DealModel>> GetDeal(int dealId)
        {
            var deals = await GetDealsList(dealId);
            return new DataProviderModel<DealModel>(deals.ResultMessage, deals.Model.First());
        }

        public async Task<DataProviderModel<List<DealModel>>> GetDeals()
        {
            return await GetDealsList();
        }

        private async Task<DataProviderModel<List<DealModel>>> GetDealsList(int? dealId = null)
        {
            var queryIdentifier1 = Guid.NewGuid().ToString();
            var query1 = new Query(queryIdentifier1, "GetDeals");
            if (dealId.HasValue)
            {
                query1.Parameters = new List<QueryParameter>
                {
                    new QueryParameter("in", "Deal_Id", dealId.ToString())
                };
            }

            var listQuery = new List<Query> { query1 };

            var result = await _sessionQueryExecutor.Execute(listQuery).ConfigureAwait(false);

            var getDealsQuery = result.Queries.FirstOrDefault(q => q.Name == "GetDeals");

            var deals = new List<DealModel>();
            if (getDealsQuery != null)
            {
                var dataTable = getDealsQuery.RetTable;

                foreach (DataRow objectRow in dataTable.Rows)
                {
                    deals.Add(new DealModel(
                        objectRow["Deal_Id"].GetInt(),
                        objectRow["SellerUser_Id"].GetNullableInt(),
                        objectRow["BuyerUser_Id"].GetNullableInt(),
                        objectRow["HostUser_Id"].GetInt(),
                        objectRow["CheckedByUser_Id"].GetNullableInt(),
                        objectRow["CheckStatus_Name"].ToString(),
                        objectRow["CheckComment"].ToString(),
                        objectRow["DealCurrency_Id"].GetInt(),
                        _soapParser.BoolParse(objectRow["IsApprovedBySeller"]),
                        _soapParser.BoolParse(objectRow["IsApprovedByBuyer"]),
                        _soapParser.BoolParse(objectRow["IsApprovedByMe"]),
                        _soapParser.BoolParse(objectRow["IsAccepted"]),
                        objectRow["Price"].GetDouble(),
                        objectRow["LiveTimeInHours"].GetInt(),
                        objectRow["PaymentTimeInMinutes"].GetInt(),
                        objectRow["Comment"].ToString(),
                        objectRow["CloseDate"].GetNullableDateTime(),
                        objectRow["CreateDate"].GetDateTime(),
                        objectRow["ChangeDate"].GetDateTime(),
                        objectRow["BuySell"].ToString() == "S" ? DealType.Sell : DealType.Buy,
                        _soapParser.BoolParse(objectRow["IsMyDeal"]),
                        _soapParser.BoolParse(objectRow["Blocked"]),
                        objectRow["Pin"].GetInt(),
                        objectRow["Account_Id"].GetNullableInt(),
                        objectRow["Objects"].ToString()));
                }
            }

            return new DataProviderModel<List<DealModel>>(result.ResultMessage, deals);
        }

        public async Task<DataProviderModel<DealDelevery>> GetDealDelevery(int dealId)
        {
            var queryIdentifier1 = Guid.NewGuid().ToString();
            var query1 = new Query(queryIdentifier1, "GetDealDelivery")
            {
                Parameters = new List<QueryParameter>
                {
                    new QueryParameter("in", "Deal_Id", dealId.ToString())
                }
            };

            var listQuery = new List<Query> { query1 };

            var result = await _sessionQueryExecutor.Execute(listQuery).ConfigureAwait(false);

            var getDealDeleveryQuery = result.Queries.First(q => q.Name == "GetDealDelivery");

            var dataTable = getDealDeleveryQuery.RetTable;
            var row = dataTable.Rows[0];

            var factSize = new Size();
            if (!String.IsNullOrEmpty(row["Height_Fact"]?.ToString())
                && !String.IsNullOrEmpty(row["Length_Fact"]?.ToString())
                && !String.IsNullOrEmpty(row["Width_Fact"]?.ToString()))
            {
                factSize = new Size
                {
                    Height = row["Height_Fact"].GetInt(),
                    Length = row["Length_Fact"].GetInt(),
                    Width = row["Width_Fact"].GetInt()
                };
            }

            var deleveryType = DeleveryLocationType.City;
            switch (row["DeliveryLocationType"].ToString())
            {
                case "Location":
                    deleveryType = DeleveryLocationType.Location;
                    break;
                case "City":
                    deleveryType = DeleveryLocationType.City;
                    break;
            }

            return new DataProviderModel<DealDelevery>(result.ResultMessage, new DealDelevery(
                row["Deal_Id"].GetInt(),
                row["DeliveryCptyService_Id"].GetInt(),
                row["DeliveryCptyService_Name"].ToString(),
                new Size
                {
                    Height = row["Height_Decl"].GetInt(),
                    Length = row["Length_Decl"].GetInt(),
                    Width = row["Width_Decl"].GetInt()
                },
                factSize,
                row["Weight_Decl"].GetInt(),
                row["Weight_Fact"].GetNullableInt(),
                row["TrackingNumber"].GetNullableInt(),
                row["DeliveryPrice_Calc"].GetNullableDouble(),
                row["DeliveryPrice_Fact"].GetNullableDouble(),
                row["InsurancePrice_Calc"].GetNullableDouble(),
                row["InsurancePrice_Fact"].GetNullableDouble(),
                row["Address"].ToString(),
                row["Address_From"].ToString(),
                row["Address_To"].ToString(),
                row["CityCode_From"].ToString(),
                row["CityCode_To"].ToString(),
                row["CityCode"].ToString(),
                row["LocalityCode"].ToString(),
                row["RegionCode"].ToString(),
                deleveryType,
                row["CreateDate"].GetDateTime(),
                row["ChangeDate"].GetDateTime(),
                row["Currency_Code"].ToString(),
                _soapParser.BoolParse(row["Blocked"])));
        }

        public async Task<DataProviderModel<List<DeleveryType>>> GetDeleveryTypes()
        {
            var q1_guid = Guid.NewGuid().ToString();
            var q1 = new Query(q1_guid, "GetCptyServices");

            q1.Parameters.Add(new QueryParameter("in", "CptyType_Code", "Delivery"));

            var listQuery = new List<Query> { q1 };

            var result = await _appQueryExecutorDecorator.Execute(listQuery).ConfigureAwait(false);

            var getDeliverTypesQuery = result.Queries.FirstOrDefault(q => q.Name == "GetCptyServices");

            var deliveryTypes = new List<DeleveryType>();
            if (getDeliverTypesQuery != null)
            {
                var dataTable = getDeliverTypesQuery.RetTable;

                foreach (DataRow objectRow in dataTable.Rows)
                {
                    deliveryTypes.Add(new DeleveryType(objectRow["CptyService_Id"].GetInt(), objectRow["Name"].ToString()));
                }                
            }

            return new DataProviderModel<List<DeleveryType>>(result.ResultMessage, deliveryTypes);
        }

        public async Task<DataProviderVoidModel> UpdateDeal(DealUpdateModel updateModel, DealType dealType)
        {
            var listQuery = new List<Query>();

            var upsertDealIdentifier= Guid.NewGuid().ToString();
            var upsertDealQuery= new Query(upsertDealIdentifier, "UpsertDeal");
            upsertDealQuery.Parameters.Add(new QueryParameter("in", "Price", updateModel.Price.ToString(), SqlDbType.Decimal));
            upsertDealQuery.Parameters.Add(new QueryParameter("in", "Comment", updateModel.Comment, SqlDbType.NVarChar));
            upsertDealQuery.Parameters.Add(new QueryParameter("in", "Deal_Id", updateModel.DealId.ToString(), SqlDbType.Int));
            if (dealType == DealType.Sell)
            {
                upsertDealQuery.Parameters.Add(new QueryParameter("in", "Account_Id", updateModel.AccountId.ToString(), SqlDbType.Int));
            }
            upsertDealQuery.Parameters.Add(new QueryParameter("in", "BuySell", dealType == DealType.Buy ? "B" : "S", SqlDbType.Int));
            listQuery.Add(upsertDealQuery);

            foreach (var dealObject in updateModel.DealObjectIdsToDelete)
            {
                var deleteDealObjectIdentifier= Guid.NewGuid().ToString();
                var deleteDealObjectQuery = new Query(deleteDealObjectIdentifier, "DeleteDealObject");
                deleteDealObjectQuery.ParentQueryGUID.Add(upsertDealIdentifier);
                deleteDealObjectQuery.Parameters.Add(new QueryParameter("in", "DealObject_Id", dealObject.ToString(), SqlDbType.Int));
                listQuery.Add(deleteDealObjectQuery);
            }

            foreach (var objectId in updateModel.ObjectIdsToAdd)
            {
                var upsertDealObjectIdentifier = Guid.NewGuid().ToString();
                var upsertDealObjectQuery = new Query(upsertDealObjectIdentifier, "UpsertDealObject");
                upsertDealObjectQuery.ParentQueryGUID.Add(upsertDealIdentifier);
                upsertDealObjectQuery.Parameters.Add(new QueryParameter("in", "Object_Id", objectId.ToString(), SqlDbType.Int));
                listQuery.Add(upsertDealObjectQuery);
            }

            var dealDeliveryIdentifier = Guid.NewGuid().ToString();
            var dealDeliveryQuery = new Query(dealDeliveryIdentifier, "UpsertDealDelivery");
            dealDeliveryQuery.ParentQueryGUID.Add(upsertDealIdentifier);
            dealDeliveryQuery.Parameters.Add(new QueryParameter("in", "DeliveryCptyService_Id", updateModel.DeliveryTypeId.ToString(), SqlDbType.Int));
            dealDeliveryQuery.Parameters.Add(new QueryParameter("in", "Weight_Decl", updateModel.Weight.ToString(), SqlDbType.Int));
            dealDeliveryQuery.Parameters.Add(new QueryParameter("in", "Length_Decl", updateModel.Size.Length.ToString(), SqlDbType.Int));
            dealDeliveryQuery.Parameters.Add(new QueryParameter("in", "Height_Decl", updateModel.Size.Height.ToString(), SqlDbType.Int));
            dealDeliveryQuery.Parameters.Add(new QueryParameter("in", "Width_Decl", updateModel.Size.Width.ToString(), SqlDbType.Int));

            var cityCode = updateModel.DeleveryLocationType == DeleveryLocationType.City ? updateModel.CityCode : updateModel.LocalityCode;
            dealDeliveryQuery.Parameters.Add(new QueryParameter("in", "CityCode", cityCode, SqlDbType.NVarChar));
            dealDeliveryQuery.Parameters.Add(new QueryParameter("in", "Address", updateModel.Address, SqlDbType.NVarChar));
            listQuery.Add(dealDeliveryQuery);

            var result = await _sessionQueryExecutor.Execute(listQuery).ConfigureAwait(false);

            return new DataProviderVoidModel(result.ResultMessage);
        }

        public async Task<DataProviderModel<CreateDealModel>> CreateDeal(DealCreateModel createModel, DealType dealType)
        {
            var listQuery = new List<Query>();

            var upsertDealIdentifier = Guid.NewGuid().ToString();
            var upsertDealQuery = new Query(upsertDealIdentifier, "UpsertDeal");
            upsertDealQuery.Parameters.Add(new QueryParameter("in", "Price", createModel.Price.ToString(), SqlDbType.Decimal));
            upsertDealQuery.Parameters.Add(new QueryParameter("in", "Comment", createModel.Comment, SqlDbType.NVarChar));
            if (dealType == DealType.Sell)
            {
                upsertDealQuery.Parameters.Add(new QueryParameter("in", "Account_Id", createModel.AccountId.ToString(), SqlDbType.Int));
            }
            upsertDealQuery.Parameters.Add(new QueryParameter("in", "BuySell", dealType == DealType.Buy ? "B" : "S", SqlDbType.Int));
            listQuery.Add(upsertDealQuery);

            foreach (var objectId in createModel.ObjectIdsToAdd)
            {
                var upsertDealObjectIdentifier = Guid.NewGuid().ToString();
                var upsertDealObjectQuery = new Query(upsertDealObjectIdentifier, "UpsertDealObject");
                upsertDealObjectQuery.ParentQueryGUID.Add(upsertDealIdentifier);
                upsertDealObjectQuery.Parameters.Add(new QueryParameter("in", "Object_Id", objectId.ToString(), SqlDbType.Int));
                listQuery.Add(upsertDealObjectQuery);
            }

            var dealDeliveryIdentifier = Guid.NewGuid().ToString();
            var dealDeliveryQuery = new Query(dealDeliveryIdentifier, "UpsertDealDelivery");
            dealDeliveryQuery.ParentQueryGUID.Add(upsertDealIdentifier);
            dealDeliveryQuery.Parameters.Add(new QueryParameter("in", "DeliveryCptyService_Id", createModel.DeliveryTypeId.ToString(), SqlDbType.Int));
            dealDeliveryQuery.Parameters.Add(new QueryParameter("in", "Weight_Decl", createModel.Weight.ToString(), SqlDbType.Int));
            dealDeliveryQuery.Parameters.Add(new QueryParameter("in", "Length_Decl", createModel.Size.Length.ToString(), SqlDbType.Int));
            dealDeliveryQuery.Parameters.Add(new QueryParameter("in", "Height_Decl", createModel.Size.Height.ToString(), SqlDbType.Int));
            dealDeliveryQuery.Parameters.Add(new QueryParameter("in", "Width_Decl", createModel.Size.Width.ToString(), SqlDbType.Int));

            var cityCode = createModel.DeleveryLocationType == DeleveryLocationType.City ? createModel.CityCode : createModel.LocalityCode;
            dealDeliveryQuery.Parameters.Add(new QueryParameter("in", "CityCode", cityCode, SqlDbType.NVarChar));
            dealDeliveryQuery.Parameters.Add(new QueryParameter("in", "Address", createModel.Address, SqlDbType.NVarChar));
            listQuery.Add(dealDeliveryQuery);

            var result = await _sessionQueryExecutor.Execute(listQuery).ConfigureAwait(false);

            var createDealModel = new CreateDealModel();

            var createDealQuery = result.Queries.FirstOrDefault(q => q.Name == "UpsertDeal");
            var dealIdParametr = createDealQuery?.Parameters.FirstOrDefault(p => p.Direction == "out" && p.Code == "Deal_Id");
            if (dealIdParametr != null)
            {
                createDealModel.DealId = int.Parse(dealIdParametr.Value);
            }

            return new DataProviderModel<CreateDealModel>(result.ResultMessage, createDealModel);
        }

    }
}

