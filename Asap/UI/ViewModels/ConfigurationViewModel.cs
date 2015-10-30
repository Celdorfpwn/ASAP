using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Configuration;

namespace SushiPikant.UI.ViewModels
{
    public class ConfigurationViewModel : ViewModel
    {

        private ConfigurationModel Model { get ; set; }

        public ConfigurationViewModel()
        {
            Model = new ConfigurationModel();
        }

        /// <summary>
        /// Gets or sets the username
        /// </summary>
        /// <returns></returns>
        public string Username
        {
            get
            {
                return Model.Credentials.Username;
            }

            set
            {
                Model.Credentials.Username = value;
            }
        }

        /// <summary>
        /// Gets or sets the password
        /// </summary>
        /// <returns></returns>
        public string Password
        {
            get
            {
                return Model.Credentials.Password;
            }

            set
            {
                Model.Credentials.Password = value;
            }
        }





        /// <summary>
        /// The singleton
        /// </summary>
        /// <returns>The singleton</returns>
        private static ConfigurationViewModel Singleton { get;set; }

        /// <summary>
        /// Singleton implementation
        /// </summary>
        /// <returns>Returns the only instance of the class</returns>
        public static ConfigurationViewModel Instance
        {
            get
            {
                if (Singleton == null)
                {
                    Singleton = new ConfigurationViewModel();
                }

                return Singleton;
            }
        }
    }
}
