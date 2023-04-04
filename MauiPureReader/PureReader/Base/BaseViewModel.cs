using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureReader.Base
{
    public partial class BaseViewModel : ObservableObject
    {
        public BaseViewModel()
        {
            Shell.Current.Unloaded += Current_Unloaded;
            Shell.Current.Navigated += Current_Navigated;            
        }

        protected virtual void OnNavigateIn()
        {

        }

        protected virtual void OnNavigateOut()
        {

        }

        ShellNavigationState current = null;

        private void Current_Navigated(object sender, ShellNavigatedEventArgs e)
        {
            current ??= e.Current;
            if (e.Current == current)
            {
                OnNavigateIn();
            }
            else if (e.Previous == current)
            {
                OnNavigateOut();
            }
        }

        private void Current_Unloaded(object sender, EventArgs e)
        {
            Shell.Current.Navigated -= Current_Navigated;
            Shell.Current.Unloaded -= Current_Unloaded;
        }
    }
}
