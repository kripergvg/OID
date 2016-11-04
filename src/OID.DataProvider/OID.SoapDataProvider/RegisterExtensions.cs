using Autofac;
using OID.DataProvider.Interfaces;
using OID.SoapDataProvider.Providers;
using OID.SoapDataProvider.Providers.Infrastructure;
using OID.SoapDataProvider.QuerySerializator;
using OID.SoapDataProvider.SoapServiceClient;
using OID.Core.HashGenerator;

namespace OID.SoapDataProvider
{
    public static class RegisterExtensions
    {
        public static ContainerBuilder RegisterSoapDataProvider(this ContainerBuilder builder, string login, string password,string soapClientUrl)
        {
            builder.Register(c => new SoapServiceClient.SoapServiceClient(soapClientUrl)).As<ISoapServiceClient>();
            builder.RegisterType<QuerySerializator.QuerySerializator>().As<IQuerySerializator>();
            builder.RegisterType<SessionProvider>().As<ISessionProvider>();
            builder.RegisterType<UserProvider>().As<IUserProvider>();
            builder.RegisterType<UserSessionQueryExecutor>().As<IUserSessionQueryExecutor>();
            builder.Register(c => new AppQueryExecutor(login, password, c.Resolve<IHashGenerator>(), c.Resolve<IQuerySerializator>(), c.Resolve<ISoapServiceClient>()))
                .As<IAppQueryExecutor>();

            return builder;
        }
    }
}
