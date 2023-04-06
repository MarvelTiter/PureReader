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
            NavigatedToCommand = new AsyncRelayCommand(OnNavigatedTo);
            NavigatedFromCommand = new AsyncRelayCommand(OnNavigatedFrom);
        }

        public virtual Task OnNavigatedTo()
        {
            return Task.CompletedTask;
        }

        public virtual Task OnNavigatedFrom()
        {
            return Task.CompletedTask;
        }

    }
}
