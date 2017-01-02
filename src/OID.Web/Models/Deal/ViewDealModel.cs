using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using OID.DataProvider.Models.Deal;

namespace OID.Web.Models.Deal
{
    public class ViewDealModel
    {
        [Display(Name = "Номер сделки")]
        public int DealId { get; set; }

        [Display(Name = "Оператор доставки")]
        public string DeliveryTypeName { get; set; }

        [Display(Name = "Размер указанный, см")]
        public Size Size { get; set; }

        [Display(Name = "Вес для расчета стоимости доставки, грамм")]
        public int Weight { get; set; }

        [Display(Name = "Цена")]
        public double Price { get; set; }

        [Display(Name = "Комментарий")]
        public string Comment { get; set; }

        [Display(Name = "Продавец")]
        public string SellerName { get; set; }

        [Display(Name = "Покупатель")]
        public string BuyerName { get; set; }

        [Display(Name = "Объекты сделки")]
        public IList<DealObject> DealObjects { get; set; }

        public bool AcceptedByMe { get; set; }

        public bool AcceptedByPartner { get; set; }

        public bool IsBlocked { get; set; }

        public bool IsAccepted { get; set; }

        public DealType DealType { get; set; }

        public class DealObject
        {
            public int ObjectId { get; set; }

            public int CheckListId { get; set; }

            public string ObjectName { get; set; }

            public IList<ObjectCheck> ObjectChecks { get; set; }

            public int DealObjectId { get; set; }

            [Display(Name = "Подтверждено партнером")]
            public bool AcceptedByMe { get; set; }

            [Display(Name = "Подтверждено мной")]
            public bool AcceptedByPartner { get; set; }

            public class ObjectCheck
            {
                [Display(Name = "Подробное описание проверки")]
                public string Description { get; set; }

                [Display(Name = "Фотография для проверки")]
                public string ImageUrl { get; set; }
            }
        }
    }
}
