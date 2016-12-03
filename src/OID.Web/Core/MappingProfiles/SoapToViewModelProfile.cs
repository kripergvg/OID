using AutoMapper;
using OID.DataProvider.Models.Deal;
using OID.DataProvider.Models.Object;
using OID.DataProvider.Models.User;
using OID.Web.Models;
using CheckType = OID.Web.Models.CheckType;

namespace OID.Web.Core.MappingProfiles
{
    public class SoapToViewModelProfile : Profile
    {
        public SoapToViewModelProfile()
        {
            CreateMap<UserObject, ObjectListViewModel>();
            CreateMap<UserPhone, PhoneViewModel>().ForMember(d => d.Phone, s => s.MapFrom(src => src.Number));
            CreateMap<PhoneViewModel, UserPhone>().ForMember(d => d.Number, s => s.MapFrom(src => src.Phone));
            CreateMap<CheckListItem, ObjectModifyViewModel.ObjectCheck>()
                .ForMember(d => d.CheckType, s => s.MapFrom(src => (CheckType) src.CheckType))
                .ForMember(d => d.Description, s => s.MapFrom(src => src.Task));
        }
    }
}
