using Autofac;
using Microsoft.AspNetCore.Http;
using OID.Web.Authenticate;

namespace OID.Web
{
    public static class RegisterExtensions
    {
        public static ContainerBuilder RegisterWeb(this ContainerBuilder builder)
        {
            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>();
            builder.RegisterType<UserManager>().As<IUserManager>();
            return builder;
        }
    }
}
