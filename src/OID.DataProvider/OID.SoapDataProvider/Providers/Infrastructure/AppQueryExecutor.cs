using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OID.Core.HashGenerator;
using OID.SoapDataProvider.QuerySerializator;
using OID.SoapDataProvider.SoapServiceClient;

namespace OID.SoapDataProvider.Providers.Infrastructure
{
    internal class AppQueryExecutor : IAppQueryExecutor
    {
        private readonly string _login;
        private readonly string _password;
        private readonly IHashGenerator _hashGenerator;
        private readonly IQuerySerializator _querySerializator;
        private readonly ISoapServiceClient _serviceClient;

        public AppQueryExecutor(string login, string password, IHashGenerator hashGenerator, IQuerySerializator querySerializator, ISoapServiceClient serviceClient)
        {
            _login = login;
            _password = password;
            _hashGenerator = hashGenerator;
            _querySerializator = querySerializator;
            _serviceClient = serviceClient;
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

            var serializedQueries = _querySerializator.ToStringXml(fulllistQuery);
            var response = await _serviceClient.PostAsync(serializedQueries).ConfigureAwait(false);
            var queries = _querySerializator.ToQueryList(response);

            return queries;
        }
    }
}
