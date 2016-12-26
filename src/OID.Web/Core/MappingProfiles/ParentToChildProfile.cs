using AutoMapper;
using OID.Web.Models.Deal;

namespace OID.Web.Core.MappingProfiles
{
    public class ParentToChildProfile : Profile
    {
        public ParentToChildProfile()
        {
            CreateMap<DealModifyModel, SellDealModifyViewModel>();
        }
    }
}
