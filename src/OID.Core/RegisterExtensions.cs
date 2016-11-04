using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OID.Core.HashGenerator;

namespace OID.Core
{
    public static class RegisterExtensions
    {
        public static ContainerBuilder RegisterSoapDataProvider(this ContainerBuilder builder, string login, string password, IHashGenerator hashGenerator)
        {
        }
    }
}
