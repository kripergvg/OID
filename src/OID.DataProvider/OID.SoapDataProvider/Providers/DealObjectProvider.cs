using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using OID.DataProvider.Interfaces;
using OID.DataProvider.Models;
using OID.DataProvider.Models.Deal;
using OID.DataProvider.Models.Object;
using OID.DataProvider.Models.Object.In;
using OID.SoapDataProvider.Providers.Infrastructure;
using DealObject = OID.DataProvider.Models.Object.In.DealObject;

namespace OID.SoapDataProvider.Providers
{
    public class DealObjectProvider : IDealObjectProvider
    {
        private readonly IUserSessionQueryExecutorDecorator _sessionQueryExecutor;
        private readonly IAppQueryExecutorDecorator _queryExecutorDecorator;

        public DealObjectProvider(IUserSessionQueryExecutorDecorator sessionQueryExecutor, IAppQueryExecutorDecorator queryExecutorDecorator)
        {
            _sessionQueryExecutor = sessionQueryExecutor;
            _queryExecutorDecorator = queryExecutorDecorator;
        }

        public async Task<DataProviderModel<UpsertObjectModel>> Upsert(DealObject dealObject)
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

            var result = await _sessionQueryExecutor.Execute(listQuery).ConfigureAwait(false);

            var model = new DataProviderModel<UpsertObjectModel>(result.ResultMessage);

            foreach (var q11 in result.Queries)
            {
                if (q11.Name == "UpsertUserObject")
                {
                    if (q11.Parameters.Exists(x => x.Code == "Object_Id"))
                    {
                        model.Model = new UpsertObjectModel(q11.Parameters.Find(x => x.Code == "Object_Id").Value);
                    }

                }
            }

            return model;
        }

        public async Task<DataProviderVoidModel> Approve(string dealId)
        {
            List<Query> listQuery = new List<Query>();

            string q1_guid = Guid.NewGuid().ToString();
            Query q1 = new Query(q1_guid, "DealObjectApprove");
            q1.Parameters.Add(new QueryParameter("in", "DealObject_Id", dealId, SqlDbType.Int));
            q1.Parameters.Add(new QueryParameter("in", "Approve", "Y", SqlDbType.NVarChar));
            listQuery.Add(q1);


            var result = await _sessionQueryExecutor.Execute(listQuery).ConfigureAwait(false);

            return new DataProviderVoidModel(result.ResultMessage);
        }

        public async Task<DataProviderModel<List<UserObject>>> GetUserObjects()
        {
            List<Query> listQuery = new List<Query>();

            string q_guid = Guid.NewGuid().ToString();
            Query query = new Query(q_guid, "GetUserObjects");
            listQuery.Add(query);

            var result = await _sessionQueryExecutor.Execute(listQuery).ConfigureAwait(false);

            var userObjectsQuery = result.Queries.FirstOrDefault(q => q.Name == "GetUserObjects");

            var objects = new List<UserObject>();
            if (userObjectsQuery != null)
            {
                var dataTable = userObjectsQuery.RetTable;

                foreach (DataRow objectRow in dataTable.Rows)
                {
                    objects.Add(new UserObject(
                        objectRow["Object_Id"].ToString(),
                        objectRow["ObjectName"].ToString(),
                        objectRow["Description"].ToString(),
                        objectRow["ObjectCategory_Name"].ToString(),
                        objectRow["ObjectCategory_Id"].ToString(),
                        objectRow["ObjectStatus_Name"].ToString(),
                        objectRow["ObjectStatus_Id"].ToString(),
                        objectRow["Blocked"].ToString() == "1",
                        objectRow["CheckList_Id"].ToString()
                    ));
                }

            }

            return new DataProviderModel<List<UserObject>>(result.ResultMessage, objects);
        }

        public async Task<DataProviderModel<List<ObjectCategory>>> GetCategories()
        {
            List<Query> listQuery = new List<Query>();
            string q_guid = Guid.NewGuid().ToString();
            Query query = new Query(q_guid, "GetObjectCategories");
            listQuery.Add(query);

            var result = await _queryExecutorDecorator.Execute(listQuery).ConfigureAwait(false);

            var getCitiesQuery = result.Queries.FirstOrDefault(q => q.Name == "GetObjectCategories");

            var categories = new List<ObjectCategory>();
            if (getCitiesQuery != null)
            {
                var dataTable = getCitiesQuery.RetTable;

                foreach (DataRow objectRow in dataTable.Rows)
                {
                    categories.Add(new ObjectCategory(
                        objectRow["ObjectCategory_Id"].ToString(),
                        objectRow["ObjectCategory_Name"].ToString()
                    ));
                }
            }

            return new DataProviderModel<List<ObjectCategory>>(result.ResultMessage, categories);
        }

        public async Task<DataProviderModel<string>> CreateObject(CreateDealObject dealObject)
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

            var listQuery = new List<Query>
            {
                query1
            };

            var queryIdentifier2 = Guid.NewGuid().ToString();
            var query2 = new Query(queryIdentifier2, "UpsertCheckList");
            query2.ParentQueryGUID.Add(queryIdentifier1);
            query2.Parameters.Add(new QueryParameter("in", "CheckListName", "Лист проверок", SqlDbType.NVarChar));

            listQuery.Add(query2);

            foreach (var dealCheck in dealObject.CheckList)
            {
                var queryIdentifier = Guid.NewGuid().ToString();


                var query = new Query(queryIdentifier, "UpsertCheck");
                query.ParentQueryGUID.Add(queryIdentifier2);
                query.Parameters.Add(new QueryParameter("in", "CheckType_Id", dealCheck.CheckTypeId, SqlDbType.Int));
                query.Parameters.Add(new QueryParameter("in", "Task", dealCheck.Task, SqlDbType.NVarChar));


                listQuery.Add(query);
            }

            var result = await _sessionQueryExecutor.Execute(listQuery).ConfigureAwait(false);

            var model = new DataProviderModel<string>(result.ResultMessage);

            foreach (var q11 in result.Queries)
            {
                if (q11.Name == "UpsertUserObject")
                {
                    if (q11.Parameters.Exists(x => x.Code == "Object_Id"))
                    {
                        model.Model = q11.Parameters.Find(x => x.Code == "Object_Id").Value;
                    }

                }
            }
            return model;
        }

        public async Task<DataProviderModel<List<CheckListItem>>> GetChecks(string checkListId)
        {
            var queryIdentifier1 = Guid.NewGuid().ToString();
            var query1 = new Query(queryIdentifier1, "GetChecks_FromCheckList");
            query1.Parameters.Add(new QueryParameter("in", "CheckList_Id", checkListId));

            var listQuery = new List<Query> { query1 };

            var result = await _sessionQueryExecutor.Execute(listQuery).ConfigureAwait(false);

            var getChecksQuery = result.Queries.FirstOrDefault(q => q.Name == "GetChecks_FromCheckList");

            var cheks = new List<CheckListItem>();
            if (getChecksQuery != null)
            {
                var dataTable = getChecksQuery.RetTable;

                foreach (DataRow objectRow in dataTable.Rows)
                {
                    cheks.Add(new CheckListItem(
                        objectRow["Check_Id"].ToString(),
                        objectRow["CheckList_Id"].ToString(),
                        objectRow["Task"].ToString(),
                        (CheckType) Int32.Parse(objectRow["CheckType_Id"].ToString()),
                        objectRow["CheckComment"].ToString()));
                }
            }

            return new DataProviderModel<List<CheckListItem>>(result.ResultMessage, cheks);
        }

        public async Task<DataProviderModel<UserObject>> GetUserObject(string objectID)
        {
            var userObjects = await GetUserObjects();
            return new DataProviderModel<UserObject>(userObjects.ResultMessage, userObjects.Model.FirstOrDefault(o => o.ObjectId == objectID));
        }

        public async Task<DataProviderVoidModel> UpdateObject(UpdateDealObject dealObject)
        {
            var queryIdentifier1 = Guid.NewGuid().ToString();
            var query1 = new Query(queryIdentifier1, "UpsertUserObject");
            query1.Parameters.Add(new QueryParameter("in", "ObjectCategory_Id", dealObject.ObjectCategoryId, SqlDbType.Int));
            query1.Parameters.Add(new QueryParameter("in", "ObjectStatus_Id", dealObject.ObjectStatusId, SqlDbType.Int));
            query1.Parameters.Add(new QueryParameter("in", "ObjectName", dealObject.ObjectName, SqlDbType.NVarChar));
            query1.Parameters.Add(new QueryParameter("in", "Object_Id", dealObject.ObjectId, SqlDbType.Int));


            if (!string.IsNullOrEmpty(dealObject.Description))
            {
                query1.Parameters.Add(new QueryParameter("in", "Description", dealObject.Description, SqlDbType.NVarChar));
            }

            var listQuery = new List<Query>
            {
                query1
            };

            var result = await _sessionQueryExecutor.Execute(listQuery).ConfigureAwait(false);

            var model = new DataProviderVoidModel(result.ResultMessage);

            return model;
        }

        public async Task<DataProviderVoidModel> DeleteChecks(List<string> checkIds, string checkListId, string objectId)
        {
            var model = new DataProviderVoidModel(new ResultMessage(0, DataProvider.Models.MessageType.Information, ""));
            if (checkIds.Any())
            {
                var listQuery = new List<Query>();
                var queryIdentifier2 = Guid.NewGuid().ToString();
                var query2 = new Query(queryIdentifier2, "UpsertCheckList");
                query2.Parameters.Add(new QueryParameter("in", "CheckListName", "Лист проверок", SqlDbType.NVarChar));
                query2.Parameters.Add(new QueryParameter("in", "CheckList_Id", checkListId, SqlDbType.Int));
                query2.Parameters.Add(new QueryParameter("in", "Object_Id", objectId, SqlDbType.Int));

                listQuery.Add(query2);

                foreach (var checkId in checkIds)
                {
                    var queryIdentifier = Guid.NewGuid().ToString();
                    var query = new Query(queryIdentifier, "DeleteCheck");
                    query.ParentQueryGUID.Add(queryIdentifier2);
                    query.Parameters.Add(new QueryParameter("in", "Check_Id", checkId, SqlDbType.Int));

                    listQuery.Add(query);
                }

                var result = await _sessionQueryExecutor.Execute(listQuery).ConfigureAwait(false);
                model = new DataProviderVoidModel(result.ResultMessage);
            }

            return model;
        }

        public async Task<DataProviderVoidModel> CreateChecks(List<DealCheck> checks, string checkListId, string objectId)
        {
            var model = new DataProviderVoidModel(new ResultMessage(0, DataProvider.Models.MessageType.Information, ""));
            if (checks.Any())
            {
                var listQuery = new List<Query>();
                var queryIdentifier2 = Guid.NewGuid().ToString();
                var query2 = new Query(queryIdentifier2, "UpsertCheckList");
                query2.Parameters.Add(new QueryParameter("in", "CheckListName", "Лист проверок", SqlDbType.NVarChar));
                query2.Parameters.Add(new QueryParameter("in", "CheckList_Id", checkListId, SqlDbType.Int));
                query2.Parameters.Add(new QueryParameter("in", "Object_Id", objectId, SqlDbType.Int));

                listQuery.Add(query2);

                foreach (var check in checks)
                {
                    var queryIdentifier = Guid.NewGuid().ToString();
                    var query = new Query(queryIdentifier, "UpsertCheck");
                    query.ParentQueryGUID.Add(queryIdentifier2);
                    query.Parameters.Add(new QueryParameter("in", "CheckType_Id", check.CheckTypeId, SqlDbType.Int));
                    query.Parameters.Add(new QueryParameter("in", "Task", check.Task, SqlDbType.NVarChar));

                    listQuery.Add(query);
                }

                var result = await _sessionQueryExecutor.Execute(listQuery).ConfigureAwait(false);
                model = new DataProviderVoidModel(result.ResultMessage);
            }

            return model;
        }

        public async Task<DataProviderVoidModel> DeleteObject(string objectId)
        {
            var queryIdentifier1 = Guid.NewGuid().ToString();
            var query1 = new Query(queryIdentifier1, "DeleteUserObject");

            query1.Parameters.Add(new QueryParameter("in", "Object_Id", objectId, SqlDbType.Int));

            var listQuery = new List<Query>
            {
                query1
            };

            var result = await _sessionQueryExecutor.Execute(listQuery).ConfigureAwait(false);
            var model = new DataProviderVoidModel(result.ResultMessage);
            return model;
        }
    }
}
