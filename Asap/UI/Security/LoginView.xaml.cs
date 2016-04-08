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
using SushiPikant.UI.UIController;
using SushiPikant.UI.ViewModels;

namespace SushiPikant.UI.Security
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {

        private Controller Controller { get; set; }

        public LoginView(Controller controller)
        {
            InitializeComponent();
            Controller = controller;
            MainGrid.DataContext = new ConfigurationViewModel();
        }

        private void Login()
        {
            try
            {
                Controller.SwitchToDevView();
            }
            catch (Exception)
            {
                ErrorLabel.Content = "Invalid credentials";
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }

        private void WindowKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Login();
            }
        }
    }
}

