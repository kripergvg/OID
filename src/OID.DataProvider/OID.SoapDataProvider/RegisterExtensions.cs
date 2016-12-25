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
            builder.RegisterType<DealObjectProvider>().As<IDealObjectProvider>();
            builder.RegisterType<RegionProvider>().As<IRegionProvider>();
            builder.RegisterType<UserSessionQueryExecutorDecorator>().As<IUserSessionQueryExecutorDecorator>();
            builder.Register(c => new AppQueryExecutorDecorator(login, password, c.Resolve<IHashGenerator>(), c.Resolve<IQueryExecutor>()))
                .As<IAppQueryExecutorDecorator>();
            builder.RegisterType<QueryExecutor>().As<IQueryExecutor>();
            builder.RegisterType<SoapParser>().As<ISoapParser>();
            builder.RegisterType<DealProvider>().As<IDealProvider>();

            return builder;
        }
    }
}
