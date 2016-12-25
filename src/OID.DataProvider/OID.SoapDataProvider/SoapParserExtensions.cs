using System;
using System.Globalization;

namespace OID.SoapDataProvider
{
    internal  static class SoapParserExtensions
    {
        public static int GetInt(this object value)
        {
            return int.Parse(value.ToString());
        }

        public static int? GetNullableInt(this object value)
        {
            var strValue = value?.ToString();
            if (String.IsNullOrEmpty(strValue))
                return null;

            return GetInt(value);
        }

        public static DateTime GetDateTime(this object value)
        {
            return DateTime.Parse(value.ToString());
        }

        public static DateTime? GetNullableDateTime(this object value)
        {
            var strValue = value?.ToString();
            if (String.IsNullOrEmpty(strValue))
                return null;

            return GetDateTime(value);
        }

        public static double GetDouble(this object value)
        {
            return double.Parse(value.ToString(), CultureInfo.InvariantCulture);
        }

        public static double? GetNullableDouble(this object value)
        {
            var strValue = value?.ToString();
            if (String.IsNullOrEmpty(strValue))
                return null;

            return GetDouble(value);
        }
    }
}
