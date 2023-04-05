using CommunityToolkit.Maui.Behaviors;
using PureReader.Views;
using Shared.Services;
using System.Diagnostics;

namespace PureReader
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ReadView), typeof(ReadView));
            //Navigating += AppShell_Navigating;
            //Navigated += AppShell_Navigated;
        }

        private void AppShell_Navigating(object sender, ShellNavigatingEventArgs e)
        {
            //if (CurrentPage.BindingContext is INavigable nav)
            //{
            //    CurrentPage.NavigatedFrom += CurrentPage_NavigatedFrom; ;
            //}
            //Debug.WriteLine($"Navigating CurrentPage: {CurrentPage.GetType().Name}, State: {CurrentState.Location}, Current: {e.Current?.Location}, Target: {e.Target?.Location}");
        }

        private void AppShell_Navigated(object sender, ShellNavigatedEventArgs e)
        {
            if (CurrentPage.BindingContext is INavigable nav)
            {
                //CurrentPage.Behaviors.Add(new EventToCommandBehavior
                //{
                //    EventName = nameof(NavigatedTo),
                //    Command = nav.NavigatedToCommand,
                //});
            }
            //Debug.WriteLine($"Navigated CurrentPage: {CurrentPage.GetType().Name}, State: {CurrentState.Location}, Current: {e.Current?.Location}, Previous: {e.Previous?.Location}");
        }
    }
}