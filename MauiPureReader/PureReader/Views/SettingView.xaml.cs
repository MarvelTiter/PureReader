using PureReader.ViewModels;

namespace PureReader.Views;

public partial class SettingView : ContentPage
{
	public SettingView(SettingViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}