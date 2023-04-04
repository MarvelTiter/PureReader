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
    private readonly NavigationService navigation;
    private readonly FileService fileService;

    public BookshelfViewModel(BookService bookService, NavigationService navigation, FileService fileService)
    {
        this.bookService = bookService;
        this.navigation = navigation;
        this.fileService = fileService;
        _ = GetBooks();
    }
    [ObservableProperty]
    private ObservableCollection<Book> books;

    [ObservableProperty]
    private Book selectedBook;

    [RelayCommand]
    private async Task GetBooks()
    {
        var books = await bookService.GetBooks();
        Books = new ObservableCollection<Book>(books);
    }

    [RelayCommand]
    private async Task AddBook()
    {
        var files = await fileService.PickerFilesAsync();
        if (files == null) return;
        foreach (var item in files)
        {
            //var fs = fileService.OpenFileAsync(item.FullPath);
            await bookService.AddBook(new Book
            {
                Title = item.FileName,
                FilePath = item.FullPath,
            });
        }
        await GetBooks();
    }

    [RelayCommand]
    private async Task BookSelected()
    {
        await navigation.NavigateToReadViewAsync(SelectedBook);
        SelectedBook = null;
    }
}
