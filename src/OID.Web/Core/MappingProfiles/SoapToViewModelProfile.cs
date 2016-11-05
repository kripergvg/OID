using AutoMapper;
using OID.DataProvider.Models.Object;
using OID.DataProvider.Models.User;
using OID.Web.Models;

namespace OID.Web.Core.MappingProfiles
{
    public class SoapToViewModelProfile : Profile
    {
        public SoapToViewModelProfile()
        {
            CreateMap<UserObject, ObjectListViewModel>();
            CreateMap<UserPhone, PhoneViewModel>().ForMember(d => d.Phone, s => s.MapFrom(src => src.Number));
        }
    }
}
