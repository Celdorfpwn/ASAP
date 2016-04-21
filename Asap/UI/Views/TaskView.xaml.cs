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
using SushiPikant.UI.Helpers;
using SushiPikant.UI.ViewModels;
using SushiPikant.UI.Views;

namespace SushiPikant.UI.SettigsViews
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

        public void SetTemporarWarningMessage(string message)
        {
            MessageLabel.Foreground = Brushes.Red;
            ViewModel.StatusMessage = message;
            new Action(delegate ()
            {
                MessageLabel.Foreground = Brushes.Orange;
                ViewModel.StatusMessage = String.Empty;
            })
                .RunAfter(TimeSpan.FromSeconds(5));
            
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

        private void DetailsClick(object sender, RoutedEventArgs e)
        {
            DevView.Instance.PopupDetails(ViewModel);
        }
    }
}
