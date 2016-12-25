using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OID.SoapDataProvider
{
    public class SoapParser : ISoapParser
    {
        public bool BoolParse(object value)
        {
            if (value == null)
            {
                return false;
            }

            return value.ToString() == "Y";
        }
    }
}
