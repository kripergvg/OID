using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OID.DataProvider.Models.Deal;

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

        public PaymentStatus ParsePaymentStatus(object value)
        {
            return ParseEnum(new Dictionary<string, PaymentStatus>
            {
                ["NotExecuted"] = PaymentStatus.NotExecuted,
                ["Executed"] = PaymentStatus.Executed
            }, value);
        }

        public PaymentOperation ParsePaymentOperation(object value)
        {
            return ParseEnum(new Dictionary<string, PaymentOperation>
            {
                ["Block"] = PaymentOperation.Block,
                ["PaymentWithPassword"] = PaymentOperation.PaymentWithPassword
            }, value);
        }

        public DealService ParseDealService(object value)
        {
            return ParseEnum(new Dictionary<string, DealService>
            {
                ["DefaultGarantyPayment"] = DealService.DefaultGarantyPayment,
                ["p2p"] = DealService.P2P
            }, value);
        }

        public CheckStatus ParseCheckStatus(object value)
        {
            return ParseEnum(new Dictionary<string, CheckStatus>
            {
                ["1"] = CheckStatus.New
            }, value);
        }

        public CheckType ParseCheckType(object value)
        {
            return ParseEnum(new Dictionary<string, CheckType>
            {
                ["1"] = CheckType.Function,
                ["2"] = CheckType.Condition,
                ["3"] = CheckType.Equipment,
                ["4"] = CheckType.Custom
            }, value);
        }

        private T ParseEnum<T>(Dictionary<string, T> mapping, object currentValue)
        {
            var valueStr = currentValue?.ToString();
            if (String.IsNullOrEmpty(valueStr))
                throw new NotSupportedException();

            foreach (var map in mapping)
            {
                if (map.Key == valueStr)
                {
                    return map.Value;
                }
            }

            throw new NotSupportedException();
        }
    }
}
