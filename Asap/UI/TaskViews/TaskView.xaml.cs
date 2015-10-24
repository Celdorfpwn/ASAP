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
using SushiPikant.UI.ViewModels;

namespace SushiPikant.UI.TaskViews
{
    /// <summary>
    /// Interaction logic for TaskView.xaml
    /// </summary>
    public partial class TaskView : UserControl
    {

        public TaskViewModel ViewModel { get; private set; }

        public TaskView(TaskViewModel viewModel)
        {
            InitializeComponent();
            MainGrid.DataContext = viewModel;
            ViewModel = viewModel;
            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            var path = "pack://application:,,,/UI;component/Icons/" + viewModel.Severity.ToLower() + ".png";
            logo.UriSource = new Uri(path);
            logo.EndInit();
            SeverityIcon.Source = logo;
        }
    }
}
