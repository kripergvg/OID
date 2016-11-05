using AutoMapper;
using OID.DataProvider.Models.Object;
using OID.Web.Models;

namespace OID.Web.Core.MappingProfiles
{
    public class SoapToViewModelProfile : Profile
    {
        public SoapToViewModelProfile()
        {
            CreateMap<UserObject, ObjectListViewModel>();
        }
    }
}
