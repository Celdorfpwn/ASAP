using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public static class Credentials
    {
        public static string Username { get;set; }

        public static string Password { get;set; }

        public static string Credentials64
        {
            get
            {
                var bytes = Encoding.UTF8.GetBytes(Username + ":" + Password);
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
