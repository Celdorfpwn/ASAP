using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Practices.Unity;
using ToolsConfiguration;

namespace SushiPikant.UI.Configuration
{
    /// <summary>
    /// Interaction logic for CredentialsView.xaml
    /// </summary>
    public partial class CredentialsView : UserControl
    {

        public ICredentials Model
        {
            get
            {
                return this.DataContext as ICredentials;
            }
            set
            {
                this.DataContext = value;
            }
        }

        public CredentialsView()
        {
            InitializeComponent();
        }
    }
}
