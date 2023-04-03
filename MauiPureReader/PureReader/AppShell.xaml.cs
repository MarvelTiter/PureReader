using PureReader.Views;

namespace PureReader
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(Bookshelf), typeof(Bookshelf));
        }
    }
}