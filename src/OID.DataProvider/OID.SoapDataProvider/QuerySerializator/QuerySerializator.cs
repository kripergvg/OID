using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using OID.DataProvider.Models;

namespace OID.SoapDataProvider.QuerySerializator
{
    public class QuerySerializator : IQuerySerializator
    {
        public string ToStringXml(List<Query> queries)
        {
            const string rootElement = "queries";
            const string level1Element = "query";

            var xws = new XmlWriterSettings
            {
                Indent = true,
                NewLineOnAttributes = true,
                OmitXmlDeclaration = true
            };
            var sb = new StringBuilder();

            using (var xw = XmlWriter.Create(sb, xws))
            {
                xw.WriteStartElement(rootElement);
                foreach (var query in queries)
                {
                    query.ToXml(xw, level1Element);
                }
                xw.WriteEndElement();
                xw.WriteEndDocument();
            }
            return sb.ToString();
        }

        public QueryResult ToQueryList(string queriesAsXmlString)
        {
            const string level1Element = "result";
            var count = 0;
            var queries = new List<Query>();

            var code = 0;
            var message = String.Empty;

            using (var sr = new StringReader(queriesAsXmlString))
            {
                var reader = XmlReader.Create(sr);
                var doc = new XmlDocument();
                doc.Load(reader);

                foreach (XmlNode node in doc.GetElementsByTagName("status"))
                {
                    foreach (XmlAttribute attr in node.Attributes)
                    {
                        if (attr.Name == "code")
                        {
                            code = Int32.Parse(attr.Value);
                        }

                        if (attr.Name == "message")
                        {
                            message = attr.Value;
                        }
                    }
                }

                foreach (XmlNode node in doc.GetElementsByTagName(level1Element))
                {
                    Query query;
                    var queryGuid = "";
                    var name = "";
                    var queryResult = "";
                    var queryMessage = "";

                    foreach (XmlAttribute attr in node.Attributes)
                    {
                        if (attr.Name == "QueryGUID")
                        {
                            queryGuid = attr.Value;
                        }

                        if (attr.Name == "Name")
                        {
                            name = attr.Value;
                        }

                        if (attr.Name == "QueryResultCode")
                        {
                            queryResult = attr.Value;
                        }

                        if (attr.Name == "QueryResultMessage")
                        {
                            queryMessage = attr.Value;
                        }
                    }

                    if (queryGuid != "" && name != "")
                    {
                        query = new Query(queryGuid, name)
                        {
                            QueryResultCode = Convert.ToInt32(queryResult),
                            QueryResultMessage = queryMessage
                        };

                        queries.Add(query);
                        count++;
                    }
                    else
                    {
                        throw new Exception("Invalid query param: QueryGUID or Name not found");
                    }

                    foreach (XmlNode node2 in node.ChildNodes)
                    {
                        if (node2.Name == "param")
                        {
                            foreach (XmlNode param in node2.ChildNodes)
                            {
                                foreach (XmlAttribute a in param.Attributes)
                                {
                                    var dir = "";
                                    if (param.Name == "input")
                                    {
                                        dir = "in";
                                    }
                                    if (param.Name == "output")
                                    {
                                        dir = "out";
                                    }
                                    if (param.Name == "inherited")
                                    {
                                        dir = "inh";
                                    }
                                    query.Parameters.Add(new QueryParameter(dir, a.Name, a.Value));
                                }
                            }
                        }
                        if (node2.Name == "parent")
                        {
                            var a = node2.Attributes["QueryGUID"];
                            if (a != null)
                            {
                                query.ParentQueryGUID.Add(a.Value);
                            }
                        }
                        if (node2.Name == "rettable")
                        {
                            var dt = new DataTable();

                            foreach (XmlNode row in node2.ChildNodes)
                            {
                                if (dt.Rows.Count == 0)
                                {
                                    foreach (XmlAttribute a in row.Attributes)
                                    {
                                        dt.Columns.Add(a.Name);
                                    }
                                }
                                var arr = new List<string>();
                                foreach (XmlAttribute a in row.Attributes)
                                {
                                    arr.Add(a.Value);
                                }

                                dt.Rows.Add(arr.ToArray());
                            }

                            query.RetTable = dt;
                        }
                    }
                }
            }
            if (count == 0)
            {
                throw new Exception("Queries not found");
            }

            // TODO брать сообщение из сервиса
            return new QueryResult(new ResultMessage(code, OID.DataProvider.Models.MessageType.Information, message), queries);
        }
    }
}


