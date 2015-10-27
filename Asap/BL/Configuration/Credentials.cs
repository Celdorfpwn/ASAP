using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Properties;

namespace BL.Configuration
{
    /// <summary>
    /// Credentials class user for saving login parameters
    /// </summary>
    public class Credentials : Setting
    {
        internal Credentials() { }

        /// <summary>
        /// Gets or set & saves the username
        /// </summary>
        /// <returns>The username</returns>
        public string Username
        {
            get
            {
                return Settings.Username;
            }
            set
            {
                Settings.Username = value;
                Settings.Save();
            }
        }

        /// <summary>
        /// Gets or sets & saves the password
        /// </summary>
        /// <returns>The password</returns>
        public string Password
        {
            get
            {
                return Settings.Password;
            }
            set
            {
                Settings.Password = value;
                Settings.Save();
            }
        }


        public string Credentials64
        {
            get
            {
                var bytes = Encoding.UTF8.GetBytes(Username + ":" + Password);
                return Convert.ToBase64String(bytes);
            }
        }

    }
}
