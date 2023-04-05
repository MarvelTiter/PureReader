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
        }
    }
}