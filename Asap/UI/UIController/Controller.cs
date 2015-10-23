using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SushiPikant.UI.Security;
using SushiPikant.UI.TaskViews;
using SushiPikant.UI.ViewModels;

namespace SushiPikant.UI.UIController
{
    public class Controller
    {
        public MainWindow Window { get;set; }

        public DevView DevView { get;set; }



        public Controller(MainWindow window)
        {
            Window = window;
            Initialize();
        }

        private void Initialize()
        {
            DevView = new DevView(new DevViewModel());

            Window.ContentContainer.Content = new LoginView(this);
        }

        internal void SwitchToDevView()
        {
            Window.ContentContainer.Content = DevView;
        }
    }
}
