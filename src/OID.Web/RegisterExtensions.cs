using Autofac;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using OID.Core;
using OID.Web.Authenticate;
using OID.Web.Core.MappingProfiles;

namespace OID.Web
{
    public static class RegisterExtensions
    {
        public static ContainerBuilder RegisterWeb(this ContainerBuilder builder)
        {
            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>();
            builder.RegisterType<UserManager>().As<IUserManager>();
            builder.RegisterType<SessionUpdater>().As<ISessionUpdater>();

            var config = new MapperConfiguration(expression =>
            {
                expression.AddProfile(new SoapToViewModelProfile());
                expression.AddProfile(new ParentToChildProfile());
            });
            builder.Register(c => new Mapper(config)).As<IMapper>();

            return builder;
        }
    }
}
