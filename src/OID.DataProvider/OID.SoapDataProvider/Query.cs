using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace OID.SoapDataProvider
{
    public class Query
    {
        public List<QueryParameter> Parameters = new List<QueryParameter>();
        public List<string> ParentQueryGUID = new List<string>();
        private DataTable retTable = new DataTable();
        private string guid;
        private string name;
        private int queryResultCode;
        private string queryResultMessage;

        private const string dateTimeFormat = "yyyyMMdd HH:mm:ss.fff";

        public Query(string guid, string name)
        {
            this.guid = guid;
            this.name = name;
            this.queryResultCode = -1;
        }

        public string GUID
        {
            get
            {
                return this.guid;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public int QueryResultCode
        {
            get
            {
                return this.queryResultCode;
            }
            set
            {
                this.queryResultCode = value;
            }
        }

        public string QueryResultMessage
        {
            get
            {
                return this.queryResultMessage;
            }
            set
            {
                this.queryResultMessage = value;
            }
        }

        public DataTable RetTable
        {
            get
            {
                return this.retTable;
            }
            set
            {
                this.retTable = value;
            }

        }


        public void ToXml(XmlWriter xw, string Level1Element)
        {
            xw.WriteStartElement(Level1Element);
            xw.WriteAttributeString("QueryGUID", this.GUID);
            xw.WriteAttributeString("Name", this.Name);
            xw.WriteAttributeString("QueryResultCode", this.QueryResultCode.ToString());
            xw.WriteAttributeString("QueryResultMessage", this.QueryResultMessage);

            foreach (string s in this.ParentQueryGUID)
            {
                xw.WriteStartElement("parent");
                xw.WriteAttributeString("QueryGUID", s);
                xw.WriteEndElement();
            }

            xw.WriteStartElement("param");
            xw.WriteStartElement("input");
            foreach (QueryParameter p in this.Parameters)
            {
                if (p.Direction == "in")
                {
                    xw.WriteAttributeString(p.Code, p.Value);
                }
            }
            xw.WriteEndElement();

            xw.WriteStartElement("inherited");
            foreach (QueryParameter p in this.Parameters)
            {
                if (p.Direction == "inh")
                {
                    xw.WriteAttributeString(p.Code, p.Value);
                }
            }
            xw.WriteEndElement();

            xw.WriteStartElement("output");
            foreach (QueryParameter p in this.Parameters)
            {
                if (p.Direction == "out" || p.Direction == "inout")
                {
                    xw.WriteAttributeString(p.Code, p.Value);
                }
            }
            xw.WriteEndElement();
            xw.WriteEndElement();

            if (this.RetTable.Rows.Count > 0)
            {
                xw.WriteStartElement("rettable");
                foreach (DataRow dr in this.RetTable.Rows)
                {
                    xw.WriteStartElement("row");
                    foreach (DataColumn dc in this.RetTable.Columns)
                    {
                        xw.WriteAttributeString(dc.Caption, dr[dc].ToString());
                    }
                    xw.WriteEndElement();
                }
                xw.WriteEndElement();
            }


            xw.WriteEndElement();
        }
    }
}
