using Autofac;
using OID.Core.HashGenerator;

namespace OID.Core
{
    public static class RegisterExtensions
    {
        public static ContainerBuilder RegisterCore(this ContainerBuilder builder)
        {
            builder.RegisterType<HashGenerator.HashGenerator>().As<IHashGenerator>();
            return builder;
        }
    }
}
