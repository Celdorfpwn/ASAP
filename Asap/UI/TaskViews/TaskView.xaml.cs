using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
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

        private void MessageClick(object sender, RoutedEventArgs e)
        {
            if (Popup.IsOpen)
            {
                Popup.IsOpen = false;
            }
            else
            {
                Popup.IsOpen = true;
                ViewModel.PopulateComments();
                Comments.ItemsSource = ViewModel.Comments;
                PopupTextBox.Focus();
            }
        }

        private void PopupKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ViewModel.AddComment(PopupTextBox.Text);

                PopupTextBox.Text = String.Empty;
            }
            else if (e.Key == Key.Escape)
            {
                Popup.IsOpen = false;
            }
        }

        private void LastCommentKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ViewModel.SetResolveMessage(LastCommentTextBox.Text);
                LastPopup.IsOpen = false;
            }
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.IsOpen = false;
        }


        public void PopUpLastComment()
        {
            LastPopup.IsOpen = true;

            Dispatcher.BeginInvoke(DispatcherPriority.Input,
             new Action(delegate ()
            {
                LastCommentTextBox.Focus();
                Keyboard.Focus(LastCommentTextBox);
            }));
        }
    }
}
