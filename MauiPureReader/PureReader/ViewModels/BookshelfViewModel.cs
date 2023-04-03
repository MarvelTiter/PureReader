using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PureReader.Views;
using Shared.Data;
using Shared.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureReader.ViewModels;

public partial class BookshelfViewModel : ObservableObject
{
    private readonly BookService bookService;

    public BookshelfViewModel(BookService bookService)
    {
        this.bookService = bookService;
    }
    [ObservableProperty]
    private ObservableCollection<Book> books;

    [RelayCommand]
    private async Task GetBooks()
    {
        var books = await bookService.GetBooks();
        Books = new ObservableCollection<Book>(books);
    }

    [RelayCommand]
    private async Task AddBook()
    {
        await bookService.AddBook(new Book
        {
            Title = "测试",
            Progress = 55,
        });
    }

    [RelayCommand]
    private Task BookSelected(Book book)
    {
        return Shell.Current.GoToAsync(nameof(ReadView),new Dictionary<string, object>
        {
            [nameof(ReadViewModel.Current)]=book,
        });
    }
}
