using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Properties;

namespace BL.Configuration
{
    public abstract class Setting
    {

        internal Setting() { }

        /// <summary>
        /// The project settings
        /// </summary>
        /// <returns>The default project setttings</returns>
        internal Settings Settings
        {
            get
            {
                return Settings.Default;
            }
        }
    }
}
