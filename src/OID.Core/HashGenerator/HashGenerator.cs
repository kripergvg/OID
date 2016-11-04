using System.Security.Cryptography;
using System.Text;

namespace OID.Core.HashGenerator
{
    public class HashGenerator: IHashGenerator
    {
        public string Generate(string input, bool hexString)
        {
            var md5pass = MD5.Create();
            // Convert the input string to a byte array and compute the hash. 
            byte[] data = md5pass.ComputeHash(Encoding.Default.GetBytes(input));

            string s = ByteArrayToString(data);

            // Return the hexadecimal string.
            if (hexString)
            {
                return "0x" + s.ToString();
            }
            else
            {
                return s.ToString();
            }
        }

        private string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}
