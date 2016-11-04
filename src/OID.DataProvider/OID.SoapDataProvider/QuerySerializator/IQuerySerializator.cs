using System.Collections.Generic;

namespace OID.SoapDataProvider.QuerySerializator
{
    public interface IQuerySerializator
    {
        string ToStringXml(List<Query> queries);

        QueryResult ToQueryList(string queriesAsXmlString);
    }
}
