using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OID.DataProvider.Interfaces;
using OID.DataProvider.Models;
using OID.DataProvider.Models.Session;
using OID.SoapDataProvider.Providers.Infrastructure;

namespace OID.SoapDataProvider.Providers
{
    public class SessionProvider : ISessionProvider
    {
        private readonly IAppQueryExecutorDecorator _appQueryExecutor;

        public SessionProvider(IAppQueryExecutorDecorator appQueryExecutor)
        {
            _appQueryExecutor = appQueryExecutor;
        }

        public async Task<DataProviderModel<AuthModel>> Authenticate(string email, string passwordHash)
        {
            string queryIdentifier = Guid.NewGuid().ToString();
            var query = new Query(queryIdentifier, "AuthSession");

            query.Parameters.Add(new QueryParameter("in", "Login", email));
            query.Parameters.Add(new QueryParameter("in", "PasswordHash", passwordHash));
            query.Parameters.Add(new QueryParameter("in", "SocAccount_Name", "Email"));

            var result = await _appQueryExecutor.Execute(new List<Query>
            {
                query
            }).ConfigureAwait(false);

            var model = new DataProviderModel<AuthModel>(result.ResultMessage, null);

            foreach (var q1 in result.Queries)
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

        public DataProviderModel<bool> CheckSession(string sessionId)
        {
            throw new NotImplementedException();
        }

        public async Task<DataProviderVoidModel> CloseSession(string sessionId)
        {
            string queryIdentifier = Guid.NewGuid().ToString();
            var q = new Query(queryIdentifier, "CloseSession");
            q.Parameters.Add(new QueryParameter("in", "Session_Id", sessionId));
            var listQuery = new List<Query> { q };

            var result = await _appQueryExecutor.Execute(listQuery).ConfigureAwait(false);
            return new DataProviderVoidModel(result.ResultMessage);
        }
    }
}
