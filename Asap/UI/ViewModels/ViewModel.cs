using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SushiPikant.UI.ViewModels
{
    public abstract class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected virtual void RaisePropertyChangedForAll()
        {
            foreach (var property in this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                RaisePropertyChanged(property.Name);
            }
        }
    }
}
