using System;
using OID.DataProvider.Models;

namespace OID.SoapDataProvider
{
    public static class PhoneTypeExtensions
    {
        public static string GetName(this PhoneType phoneType)
        {
            switch (phoneType)
            {
                case PhoneType.Mobile:
                    return "Mobile";
                case PhoneType.Work:
                    return "Work";
                case PhoneType.Home:
                    return "Home";
                case PhoneType.Additional:
                    return "Additional";
                default:
                    throw new ArgumentOutOfRangeException(nameof(phoneType), phoneType, null);
            }
        }
    }
}
