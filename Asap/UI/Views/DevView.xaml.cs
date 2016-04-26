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
using SushiPikant.UI.Views;

namespace SushiPikant.UI.SettigsViews
{
    /// <summary>
    /// Interaction logic for DevView.xaml
    /// </summary>
    public partial class DevView : UserControl
    {

        public DevViewModel ViewModel { get; private set; }

        private Point ClickPosition { get; set; }

        private const string DragDataFormat = "DragData";

        public DevView(DevViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            MainGrid.DataContext = ViewModel;
        }


        private void ListViewItemDrag(object sender, MouseEventArgs e)
        {
            ListViewItem view = sender as ListViewItem;
            if (e.LeftButton == MouseButtonState.Pressed && IsMouseMoving(e) && view.Content != null && view.Content is TaskView)
            {
                DataObject dragData = new DataObject(DragDataFormat, view.Content);

                DragDrop.DoDragDrop(view, dragData, DragDropEffects.Move);
            }

            e.Handled = true;
        }

        private void ListViewDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DragDataFormat))
            {
                TaskView item = e.Data.GetData(DragDataFormat) as TaskView;
                ListView listView = sender as ListView;
                ViewModel.Update(item, listView.ItemsSource);
            }
            e.Handled = true;
        }

        private void ListViewPreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private bool IsMouseMoving(MouseEventArgs e)
        {
            var diff = ClickPosition - e.GetPosition(null);

            return Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance;
        }

        private void ListViewItemPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClickPosition = e.GetPosition(null);
        }

        private void CurrentDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DragDataFormat))
            {
                TaskView item = e.Data.GetData(DragDataFormat) as TaskView;
                ViewModel.Update(item);
            }
            e.Handled = true;
        }

        private void ListViewRequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }



        private static DevView _instance { get; set; }

        public static DevView Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DevView(new DevViewModel());
                }
                return _instance;
            }
        }

    }
}
