using PureReader.ViewModels;

namespace PureReader.Views;

public partial class BookshelfView : ContentPage
{
	public BookshelfView(BookshelfViewModel bookshelfModel)
	{
		InitializeComponent();
		BindingContext = bookshelfModel;
	}
}