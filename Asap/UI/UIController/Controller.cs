using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SushiPikant.UI.Configuration;
using SushiPikant.UI.Security;
using SushiPikant.UI.SettigsViews;
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
            SwitchToSettings();
        }

        internal void SwitchToDevView()
        {
            DevView = DevView.Instance;
            Window.ContentContainer.Content = DevView;
        }

        internal void SwitchToSettings()
        {
            Window.ContentContainer.Content = new ConfigurationView(this);
        }
    }
}
