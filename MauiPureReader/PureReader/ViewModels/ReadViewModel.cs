using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PureReader.Base;
using PureReader.EventHub;
using Shared.Data;
using Shared.Services;
using Shared.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureReader.ViewModels
{
    [QueryProperty(nameof(Current), nameof(Current))]
    public partial class ReadViewModel : BaseViewModel, IDisappearingPage, IAppearingPage
    {
        private readonly IFileHandler fileHandler;
        private readonly BookService bookService;
        [ObservableProperty]
        private Book current;
        [ObservableProperty]
        private string fileContent;
        [ObservableProperty]
        private bool loading;
        [ObservableProperty]
        private string currentChapter;
        [ObservableProperty]
        private string progress;
        public BookService Service => bookService;
        CancellationTokenSource source;
        public ReadViewModel(IFileHandler fileHandler, BookService bookService)
        {
            this.fileHandler = fileHandler;
            this.bookService = bookService;
        }

        [RelayCommand]
        async void SaveProgress(int cursor)
        {
            Current.LineCursor = cursor;
            Progress = Current.FormatProgress;
            await bookService.UpdateBookProgress(Current);
        }

        public override Task OnNavigatedTo()
        {
            if (!Current.Done)
            {
                // 继续解析
                source = new CancellationTokenSource();
                _ = fileHandler.Solve(Current, source.Token);
            }
            return Task.CompletedTask;
        }

        public override async Task OnNavigatedFrom()
        {
            source?.Cancel();
            source?.Dispose();
            //await bookService.UpdateBookInfo(Current);
            await bookService.UpdateBookProgress(Current);
            SimpleEventHub.Send<BookshelfViewModel, bool>("OnNavigatedBackFromReadView", true);
        }

        public async Task OnDisappearing()
        {
            source?.Cancel();
            source?.Dispose();
            await bookService.UpdateBookProgress(Current);
        }

        public Task OnAppearing()
        {
            source = new CancellationTokenSource();
            if (!Current.Done)
            {
                // 继续解析
                _ = fileHandler.Solve(Current, source.Token);
            }
            return Task.CompletedTask;
        }
    }
}
