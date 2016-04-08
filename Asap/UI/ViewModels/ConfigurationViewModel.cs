using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigurationDI;
using ToolsConfiguration;

namespace SushiPikant.UI.ViewModels
{
    public class ConfigurationViewModel : ViewModel
    {

        public IConfiguration Model { get ; private set; }

        public ConfigurationViewModel()
        {
            Model = ConfigurationDependencyInjection.Resolve<IConfiguration>();
        }

    }
}
