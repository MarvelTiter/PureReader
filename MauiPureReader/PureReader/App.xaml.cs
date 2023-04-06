using Shared.Data;
using System.Collections.ObjectModel;

namespace PureReader
{
    public partial class App : Application
    {       
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            App.Current?.MainPage?.DisplayAlert("未处理异常", "", "ok");
        }
    }
}