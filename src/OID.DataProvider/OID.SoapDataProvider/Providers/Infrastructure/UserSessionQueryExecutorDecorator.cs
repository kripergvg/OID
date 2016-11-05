using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using OID.DataProvider.Interfaces;
using OID.DataProvider.Models;
using OID.Core;

namespace OID.SoapDataProvider.Providers.Infrastructure
{
    public class UserSessionQueryExecutorDecorator : IUserSessionQueryExecutorDecorator
    {
        private readonly ISessionProvider _sessionProvider;
        private readonly IQueryExecutor _executor;
        private readonly IUserManager _userManager;

        public UserSessionQueryExecutorDecorator(ISessionProvider sessionProvider, IQueryExecutor executor, IUserManager userManager)
        {
            _sessionProvider = sessionProvider;
            _executor = executor;
            _userManager = userManager;
        }

        public async Task<SessionQueryResult> Execute(List<Query> existedQueries)
        {
            var userModel = _userManager.GetUser();

            foreach (var existedQuery in existedQueries)
            {
                existedQuery.Parameters.Add(new QueryParameter("in", "Session_Id", userModel.SessionId, SqlDbType.NVarChar));
            }

            var result = await _executor.Execute(existedQueries).ConfigureAwait(false);
            if ((MessageType) result.ResultMessage.Code == MessageType.SessionNotAcitve)
            {
                var authenticateResult = await _sessionProvider.Authenticate(userModel.Login, userModel.PasswordHash)
                    .ConfigureAwait(false);
                if (authenticateResult.ResultMessage.Code == 0)
                {
                    foreach (var existedQuery in existedQueries)
                    {
                        existedQuery.Parameters.Single(p => p.Code == "Session_Id").Value = authenticateResult.Model.SessionId;

                        var queryResult = await _executor.Execute(existedQueries).ConfigureAwait(false);
                        _userManager.UpdateSessionId(authenticateResult.Model.SessionId);
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
