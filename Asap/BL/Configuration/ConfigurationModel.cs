using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Configuration
{
    /// <summary>
    /// User for saving configurations for Git,Jira,Jenkings,Credentials etc.
    /// </summary>
    public class ConfigurationModel
    {
        /// <summary>
        /// The credentials used for storing Username and password
        /// </summary>
        /// <returns>The credentials</returns>
        public Credentials Credentials { get; private set; }

        /// <summary>
        /// The git settings user to store infos required for git connection
        /// </summary>
        /// <returns>The git settings</returns>
        public GitSettings GitSettings { get; private set; }

        /// <summary>
        /// The constructor
        /// </summary>
        public ConfigurationModel()
        {
            Credentials = new Credentials();
            GitSettings = new GitSettings();
        }


        /// <summary>
        /// The singleton
        /// </summary>
        /// <returns></returns>
        private static ConfigurationModel Singleton { get;set; }

        public static ConfigurationModel Instance
        {
            get
            {
                if (Singleton == null)
                {
                    Singleton = new ConfigurationModel();
                }

                return Singleton;
            }
               
        }

    }
}
