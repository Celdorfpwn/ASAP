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
using SushiPikant.UI.SettigsViews;
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
            DevView.Instance.CloseCurrentPopup();
            var button = sender as Button;
            if (button != null)
            {
                var saveDialog = new SaveFileDialog();
                saveDialog.FileName = button.Content.ToString();

                if (saveDialog.ShowDialog() == true)
                {
                    ViewModel.SaveFile(saveDialog.FileName, button.Tag);
                }
            }

            DevView.Instance.ReopenCurrentPopup();
        }

        private void CommentKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ViewModel.AddComment(CommentTextBox.Text);

                CommentTextBox.Text = String.Empty;
            }
        }
    }
}
