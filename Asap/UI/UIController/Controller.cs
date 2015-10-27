using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SushiPikant.UI.Security;
using SushiPikant.UI.SettigsViews;
using SushiPikant.UI.Settings.SettingsViews;
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
            Window.ContentContainer.Content = new LoginView(this);
            //SwitchToSettings();
        }

        internal void SwitchToDevView()
        {
            DevView = new DevView(new DevViewModel());
            Window.ContentContainer.Content = DevView;
        }

        internal void SwitchToSettings()
        {
            var settings = new SettingsView();
            Window.ContentContainer.Content = settings;
        }
    }
}
