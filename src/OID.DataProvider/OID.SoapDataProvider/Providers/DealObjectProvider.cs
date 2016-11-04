using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using OID.DataProvider.Interfaces;
using OID.DataProvider.Models;
using OID.DataProvider.Models.Object;
using OID.SoapDataProvider.Providers.Infrastructure;
using DealObject = OID.DataProvider.Models.Object.In.DealObject;

namespace OID.SoapDataProvider.Providers
{
    public class DealObjectProvider : IDealObjectProvider
    {
        private readonly IUserSessionQueryExecutor _sessionQueryExecutor;

        public DealObjectProvider(IUserSessionQueryExecutor sessionQueryExecutor)
        {
            _sessionQueryExecutor = sessionQueryExecutor;
        }

        public async Task<DataProviderModel<UpsertObjectModel>> Upsert(UserModel userModel, DealObject dealObject)
        {
            var queryIdentifier1 = Guid.NewGuid().ToString();
            var query1 = new Query(queryIdentifier1, "UpsertUserObject");
            query1.Parameters.Add(new QueryParameter("in", "ObjectCategory_Id", dealObject.ObjectCategoryId, SqlDbType.Int));
            query1.Parameters.Add(new QueryParameter("in", "ObjectStatus_Id", dealObject.ObjectStatusId, SqlDbType.Int));
            query1.Parameters.Add(new QueryParameter("in", "ObjectName", dealObject.ObjectName, SqlDbType.NVarChar));

            if (!string.IsNullOrEmpty(dealObject.Description))
            {
                query1.Parameters.Add(new QueryParameter("in", "Description", dealObject.Description, SqlDbType.NVarChar));
            }

            if (dealObject.ObjectId != null)
            {
                query1.Parameters.Add(new QueryParameter("in", "Object_Id", dealObject.ObjectId, SqlDbType.Int));
            }
            var listQuery = new List<Query>
            {
                query1
            };

            var queryIdentifier2 = Guid.NewGuid().ToString();
            var query2 = new Query(queryIdentifier2, "UpsertCheckList");
            query2.ParentQueryGUID.Add(queryIdentifier1);
            query2.Parameters.Add(new QueryParameter("in", "CheckListName", "Лист проверок", SqlDbType.NVarChar));

            if (dealObject.CheckListId != null)
            {
                query2.Parameters.Add(new QueryParameter("in", "CheckList_Id", dealObject.CheckListId, SqlDbType.Int));
            }

            listQuery.Add(query2);

            foreach (var dealCheck in dealObject.CheckList)
            {
                var queryIdentifier = Guid.NewGuid().ToString();
                Query query;

                if (dealCheck.Deleted)
                {
                    query = new Query(queryIdentifier, "DeleteCheck");
                    query.ParentQueryGUID.Add(queryIdentifier2);
                    query.Parameters.Add(new QueryParameter("in", "Check_Id", dealCheck.CheckId, SqlDbType.Int));
                }
                else
                {

                    query = new Query(queryIdentifier, "UpsertCheck");
                    query.ParentQueryGUID.Add(queryIdentifier2);
                    query.Parameters.Add(new QueryParameter("in", "CheckType_Id", dealCheck.CheckTypeId, SqlDbType.Int));
                    query.Parameters.Add(new QueryParameter("in", "Task", dealCheck.Task, SqlDbType.NVarChar));
                    if (!String.IsNullOrWhiteSpace(dealCheck.TaskLink))
                    {
                        query.Parameters.Add(new QueryParameter("in", "TaskLink", dealCheck.TaskLink, SqlDbType.NVarChar));
                    }

                    if (dealCheck.CheckId != null)
                    {
                        query.Parameters.Add(new QueryParameter("in", "Check_Id", dealCheck.CheckId, SqlDbType.Int));
                    }
                }
                listQuery.Add(query);
            }

            var result = await _sessionQueryExecutor.Execute(listQuery, userModel).ConfigureAwait(false);

            var model = new DataProviderModel<UpsertObjectModel>(result.ResultMessage, null);

            foreach (var q11 in result.Queries)
            {
                if (q11.Name == "UpsertUserObject")
                {
                    if (q11.Parameters.Exists(x => x.Code == "Object_Id"))
                    {
                        model.Model = new UpsertObjectModel(q11.Parameters.Find(x => x.Code == "Object_Id").Value, result.SessionId);
                    }

                }
            }

            return model;
        }

        public async Task<DataProviderModel<SessionModel>> Approve(UserModel userModel, string dealId)
        {
            List<Query> listQuery = new List<Query>();

            string q1_guid = Guid.NewGuid().ToString();
            Query q1 = new Query(q1_guid, "DealObjectApprove");
            q1.Parameters.Add(new QueryParameter("in", "DealObject_Id", dealId, SqlDbType.Int));
            q1.Parameters.Add(new QueryParameter("in", "Approve", "Y", SqlDbType.NVarChar));
            listQuery.Add(q1);


            var result = await _sessionQueryExecutor.Execute(listQuery, userModel).ConfigureAwait(false);

            return new DataProviderModel<SessionModel>(result.ResultMessage, new SessionModel(result.SessionId));
        }
    }
}
