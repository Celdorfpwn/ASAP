using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Security
{
    public static class SecurityExtensions
    {

        private static string password = "z40cgp9JQLSRtGsUMtCl";

        public static string Encrypt(this string value)
        {
            return Cryptography.Encrypt<AesManaged>(value, password);
        }

        public static string Decrypt(this string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return String.Empty;
            }
            else
            {
                return Cryptography.Decrypt<AesManaged>(value, password);
            }
        }
    }
}
