using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using OID.DataProvider.Interfaces;
using OID.DataProvider.Models;

namespace OID.SoapDataProvider.Providers.Infrastructure
{
    public class UserSessionQueryExecutor : IUserSessionQueryExecutor
    {
        private readonly IAppQueryExecutor _appQueryExecutor;
        private readonly ISessionProvider _sessionProvider;

        public UserSessionQueryExecutor(IAppQueryExecutor appQueryExecutor, ISessionProvider sessionProvider)
        {
            _appQueryExecutor = appQueryExecutor;
            _sessionProvider = sessionProvider;
        }

        public async Task<SessionQueryResult> Execute(List<Query> existedQueries, UserModel userModel)
        {
            foreach (var existedQuery in existedQueries)
            {
                existedQuery.Parameters.Add(new QueryParameter("in", "Session_Id", userModel.SessionId, SqlDbType.NVarChar));
            }

            var result = await _appQueryExecutor.Execute(existedQueries).ConfigureAwait(false);
            if ((MessageType) result.ResultMessage.Code == MessageType.SessionNotAcitve)
            {
                var authenticateResult = await _sessionProvider.Authenticate(userModel.Login, userModel.PasswordHash)
                    .ConfigureAwait(false);
                if (authenticateResult.ResultMessage.Code == 0)
                {
                    foreach (var existedQuery in existedQueries)
                    {
                        existedQuery.Parameters.Single(p => p.Code == "Session_Id").Value = authenticateResult.Model.SessionId;
                        var queryResult = await _appQueryExecutor.Execute(existedQueries).ConfigureAwait(false);
                        return new SessionQueryResult(queryResult, authenticateResult.Model.SessionId);
                    }
                }

                return new SessionQueryResult(result, userModel.SessionId);
            }
            else
            {
                return new SessionQueryResult(result, userModel.SessionId);
            }
        }
    }
}
