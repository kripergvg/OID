using System;

namespace OID.DataProvider.Models.Deal
{
    public class DealModel
    {
        public DealModel(
            int dealId,
            int? sellerUserId,
            int? buyerUserId,
            int hostUserId,
            int? checkedByUserId,
            string checkStatusName,
            string checkComment,
            int dealCurrencyId,
            bool isApprovedBySeller,
            bool isApprovedByBuyer, 
            bool isApprovedByMe,
            bool isAccepted,
            double price,
            int liveTimeInHours,
            int paymentTimeInMinutes,
            string comment,
            DateTime? closeDate, 
            DateTime createDate,
            DateTime changeDate,
            DealType dealType,
            bool isMyDeal, 
            bool blocked,
            int pin, 
            int? accountId, 
            string objects)
        {
            DealId = dealId;
            SellerUserId = sellerUserId;
            BuyerUserId = buyerUserId;
            HostUserId = hostUserId;
            CheckedByUserId = checkedByUserId;
            CheckStatusName = checkStatusName;
            CheckComment = checkComment;
            DealCurrencyId = dealCurrencyId;
            IsApprovedBySeller = isApprovedBySeller;
            IsApprovedByBuyer = isApprovedByBuyer;
            IsApprovedByMe = isApprovedByMe;
            IsAccepted = isAccepted;
            Price = price;
            LiveTimeInHours = liveTimeInHours;
            PaymentTimeInMinutes = paymentTimeInMinutes;
            Comment = comment;
            CloseDate = closeDate;
            CreateDate = createDate;
            ChangeDate = changeDate;
            DealType = dealType;
            IsMyDeal = isMyDeal;
            Blocked = blocked;
            Pin = pin;
            AccountId = accountId;
            Objects = objects;
        }

        public int DealId { get; set; }

        public int? SellerUserId { get; set; }

        public int? BuyerUserId { get; set; }

        public int HostUserId { get; set; }

        public int? CheckedByUserId { get; set; }

        public string CheckStatusName { get; set; }

        public string CheckComment { get; set; }

        public int DealCurrencyId { get; set; }

        public bool IsApprovedBySeller { get; set; }

        public bool IsApprovedByBuyer { get; set; }

        public bool IsApprovedByMe { get; set; }

        public bool IsAccepted { get; set; }

        public double Price { get; set; }

        public int LiveTimeInHours { get; set; }

        public int PaymentTimeInMinutes { get; set; }

        public string Comment { get; set; }

        public DateTime? CloseDate { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime ChangeDate { get; set; }

        public DealType DealType { get; set; }

        public bool IsMyDeal { get; set; }

        public bool Blocked { get; set; }

        public int Pin { get; set; }

        public int? AccountId { get; set; }

        public string Objects { get; set; }
    }
}
