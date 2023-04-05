using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PureReader.Base
{
    public partial class BaseViewModel : ObservableObject, INavigable
    {
        public ICommand NavigatedToCommand { get; }

        public ICommand NavigatedFromCommand {get; }

        public BaseViewModel()
        {
            NavigatedToCommand = new RelayCommand(OnNavigatedTo);
            NavigatedFromCommand = new RelayCommand(OnNavigatedFrom);
        }

        public virtual void OnNavigatedTo()
        {
        }

        public virtual void OnNavigatedFrom()
        {
        }

    }
}
