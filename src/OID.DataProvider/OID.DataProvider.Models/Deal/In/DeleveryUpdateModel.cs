namespace OID.DataProvider.Models.Deal.In
{
    public class DeleveryUpdateModel
    {
        public DeleveryUpdateModel(string dealId, string deleveruCptyServiceId, string weight, string length, string height, string width)
        {
            DealId = dealId;
            DeleveruCptyServiceId = deleveruCptyServiceId;
            Weight = weight;
            Length = length;
            Height = height;
            Width = width;
        }

        public string DealId { get; set; }

        public string DeleveruCptyServiceId { get; set; }

        public string Weight { get; set; }

        public string Length { get; set; }

        public string Height { get; set; }

        public string Width { get; set; }
    }
}
