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
using ToolsConfiguration;

namespace SushiPikant.UI.Configuration
{
    /// <summary>
    /// Interaction logic for IssuesTrackingConfig.xaml
    /// </summary>
    public partial class IssuesTrackingView : UserControl
    {
        public IIssuesTrackingConfig Model
        {
            get
            {
                return this.DataContext as IIssuesTrackingConfig;
            }
            set
            {
                this.DataContext = value;
            }
        }

        public IssuesTrackingView()
        {
            InitializeComponent();
        }
    }
}
