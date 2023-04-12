using System.Windows.Input;

namespace Shared.Services
{
    public interface INavigable
    {
        ICommand NavigatedToCommand { get; }
        ICommand NavigatedFromCommand { get; }
        Task OnNavigatedTo();
        Task OnNavigatedFrom();
    }

    public interface IDisappearingPage
    {
        Task OnDisappearing();
    }

    public interface IAppearingPage
    {
        Task OnAppearing();
    }
}
