using PureReader.Models;

namespace PureReader.Views;

public partial class Bookshelf : ContentPage
{
	public Bookshelf(BookshelfModel bookshelfModel)
	{
		InitializeComponent();
		BindingContext = bookshelfModel;
	}
}