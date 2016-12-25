using System;

namespace OID.DataProvider.Models.User
{
    public enum PaymentType
    {
        Yandex = 2
    }

    public static class PaymentTypeExtensions
    {
        public static string GetHumanName(this PaymentType paymentType)
        {
            switch (paymentType)
            {
                case PaymentType.Yandex:
                    return "Яндекс: Yandex.Money";
                default:
                    throw new ArgumentOutOfRangeException(nameof(paymentType), paymentType, null);
            }
        }
    }
}
 