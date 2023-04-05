using System.Windows.Input;

namespace Shared.Services
{
    public interface INavigable
    {
        ICommand NavigatedToCommand { get; }
        ICommand NavigatedFromCommand { get; }
        void OnNavigatedTo();
        void OnNavigatedFrom();
    }
}
