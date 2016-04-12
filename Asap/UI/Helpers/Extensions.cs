using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SushiPikant.UI.Helpers
{
    public static class Extensions
    {
        public static void RunAfter(this Action action, TimeSpan span)
        {
            var dispatcherTimer = new DispatcherTimer { Interval = span };
            dispatcherTimer.Tick += (sender, args) =>
            {
                var timer = sender as DispatcherTimer;
                if (timer != null)
                {
                    timer.Stop();
                }
                action();
            };
            dispatcherTimer.Start();
        }
    }
}
