using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PureReader.Base;
using PureReader.EventHub;
using PureReader.Views;
using Shared.Data;
using Shared.Services;
using Shared.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PureReader.ViewModels;

public partial class BookshelfViewModel : BaseViewModel
{
    private readonly BookService bookService;
    private readonly NavigationService navigation;
    private readonly FileService fileService;

    [ObservableProperty]
    private ObservableCollection<Book> books;

    public BookshelfViewModel(BookService bookService, NavigationService navigation, FileService fileService)
    {
        this.bookService = bookService;
        this.navigation = navigation;
        this.fileService = fileService;
        this.Register<BookshelfViewModel, bool>("OnNavigatedBackFromReadView", finish =>
        {
            _ = GetBooks();
        });
        _ = GetBooks();
    }

    //public override async Task OnNavigatedTo()
    //{
    //    await GetBooks();
    //}

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
                Id = await item.GetFileMd5Value(),
                Title = item.FileName,
                FilePath = item.FullPath,
            });
        }
        await GetBooks();
    }

    [RelayCommand]
    private async Task BookTapped(Book book)
    {
        await navigation.NavigateToReadViewAsync(book);
    }

    [RelayCommand]
    private async Task DeleteBook(Book book)
    {
        await bookService.DeleteBookAsync(book);
        await GetBooks();
    }
}
