using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OID.Core.HashGenerator;
using OID.SoapDataProvider.QuerySerializator;
using OID.SoapDataProvider.SoapServiceClient;

namespace OID.SoapDataProvider.Providers.Infrastructure
{
    internal class AppQueryExecutorDecorator : IAppQueryExecutorDecorator
    {
        private readonly string _login;
        private readonly string _password;
        private readonly IHashGenerator _hashGenerator;
        private readonly IQueryExecutor _executor;

        public AppQueryExecutorDecorator(string login, string password, IHashGenerator hashGenerator, IQueryExecutor executor)
        {
            _login = login;
            _password = password;
            _hashGenerator = hashGenerator;
            _executor = executor;
        }

        public async Task<QueryResult> Execute(List<Query> existedQueries)
        {
            var queryIdentifier = Guid.NewGuid().ToString();
            var firstQuery = new Query(queryIdentifier, "AuthSession");
            firstQuery.Parameters.Add(new QueryParameter("in", "Login", _login));
            firstQuery.Parameters.Add(new QueryParameter("in", "PasswordHash", _hashGenerator.Generate(_password, true)));
            firstQuery.Parameters.Add(new QueryParameter("in", "SocAccount_Name", "Technical"));

            var fulllistQuery = new List<Query>
            {
                firstQuery
            };

            Query lastQuery = new Query(Guid.NewGuid().ToString(), "CloseSession");
            foreach (var query in existedQueries)
            {
                query.ParentQueryGUID.Add(queryIdentifier);
                fulllistQuery.Add(query);
                lastQuery.ParentQueryGUID.Add(query.GUID);
            }

            lastQuery.ParentQueryGUID.Add(queryIdentifier);
            fulllistQuery.Add(lastQuery);

            var queries = await _executor.Execute(fulllistQuery).ConfigureAwait(false);
            return queries;
        }
    }
}
