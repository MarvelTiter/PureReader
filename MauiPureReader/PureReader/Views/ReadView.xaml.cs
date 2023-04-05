using PureReader.ViewModels;

namespace PureReader.Views;

public partial class ReadView : ContentPage
{

    public ReadView(ReadViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

}