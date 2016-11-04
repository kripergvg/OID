using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace OID.SoapDataProvider
{
    public class QueryParameter
    {
        private string code;
        private string value;
        private string direction;
        private SqlDbType dbtype;

        public QueryParameter(string direction, string code, string value)
        {
            this.direction = direction;
            this.code = code;
            this.value = value;
        }

        public QueryParameter(string direction, string code, string value, SqlDbType dbtype)
        {
            this.direction = direction;
            this.code = code;
            this.value = value;
            this.dbtype = dbtype;
        }

        public string Code
        {
            get
            {
                return this.code;
            }
        }

        public string Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }

        public string Direction
        {
            get
            {
                return this.direction;
            }
        }

        public SqlDbType DBType
        {
            get
            {
                return this.dbtype;
            }
        }
    }
}
