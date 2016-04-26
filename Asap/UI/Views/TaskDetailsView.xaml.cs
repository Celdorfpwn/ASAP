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
using Microsoft.Win32;
using SushiPikant.UI.Helpers;
using SushiPikant.UI.SettigsViews;
using SushiPikant.UI.UIController;
using SushiPikant.UI.ViewModels;

namespace SushiPikant.UI.Views
{
    /// <summary>
    /// Interaction logic for TaskDetailsView.xaml
    /// </summary>
    public partial class TaskDetailsView : UserControl
    {

        private TaskViewModel ViewModel { get; set; }

        public TaskDetailsView(TaskViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            this.DataContext = ViewModel;
        }

        private void AttachmentClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                //var saveDialog = new SaveFileDialog();
                //saveDialog.FileName = button.Content.ToString();

                //if (saveDialog.ShowDialog() == true)
                //{
                //    ViewModel.SaveFile(saveDialog.FileName, button.Tag);
                //}

                ViewModel.OpenFile(button.Tag);
            }
        }

        private void CommentKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ViewModel.AddComment(CommentTextBox.Text);

                CommentTextBox.Text = String.Empty;
            }
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Controller.Instance.SwitchToDevView();
        }

        private void SwitchToClick(object sender, RoutedEventArgs e)
        {
            
            ViewModel.SwitchToTask();
            if (ViewModel.StatusMessage == "Nothing to commit")
            {
                StatusMessage.Visibility = Visibility.Visible;
                SwitchTo.Visibility = Visibility.Hidden;
                new Action(delegate ()
                {
                    StatusMessage.Visibility = Visibility.Hidden;
                    ViewModel.StatusMessage = String.Empty;
                    SwitchTo.Visibility = Visibility.Visible;
                })
                    .RunAfter(TimeSpan.FromSeconds(5));
            }
        }
    }
}
