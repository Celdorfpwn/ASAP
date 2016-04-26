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
using ConfigurationDI;
using SushiPikant.UI.UIController;
using ToolsConfiguration;

namespace SushiPikant.UI.Configuration
{
    /// <summary>
    /// Interaction logic for ConfigurationView.xaml
    /// </summary>
    public partial class ConfigurationView : UserControl
    {

        private IConfiguration Model { get; set; }

        private Controller _controller { get; set; }

        public ConfigurationView(Controller controller)
        {
            InitializeComponent();
            _controller = controller;
            Model = ConfigurationDependencyInjection.Resolve<IConfiguration>();

            _credentialsView.Model = Model.Credentials;
            _sourceControlView.Model = Model.SourceControlConfig;
            _issuesTrackingView.Model = Model.IssuesTrackingConfig;
            _codeReviewView.Model = Model.CodeReviewConfig;
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Model.Save();
            _controller.SwitchToDevView();
        }
    }
}
