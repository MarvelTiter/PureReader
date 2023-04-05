using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PureReader.Base;
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
    public partial class ReadViewModel : BaseViewModel
    {
        private readonly FileService fileService;
        private readonly BookService bookService;
        [ObservableProperty]
        private Book current;
        [ObservableProperty]
        private string fileContent;
        [ObservableProperty]
        private ObservableCollection<Content> chapters;
        public ReadViewModel(FileService fileService, BookService bookService)
        {
            this.fileService = fileService;
            this.bookService = bookService;
            Chapters = new ObservableCollection<Content>();
        }

        [RelayCommand]
        private void Tap()
        {
            Debug.WriteLine("Tap....");
        }

        [RelayCommand]
        private void Swipe()
        {
            Debug.WriteLine("Swipe....");
        }

        //double preOffset;
        [RelayCommand]
        private void HandleScroll(ItemsViewScrolledEventArgs e)
        {
            Debug.WriteLine($"VerticalOffset: {e.VerticalOffset}, VerticalDelta: {e.VerticalDelta}");
            Debug.WriteLine($"FirstVisibleItemIndex: {e.FirstVisibleItemIndex}, LastVisibleItemIndex: {e.LastVisibleItemIndex}");
            Debug.WriteLine("=====================================================");
            Current.FirstLine = e.FirstVisibleItemIndex;
            Current.LastLine = e.LastVisibleItemIndex;
            //Render();
            //Debug.WriteLine($"CenterItemIndex:{e.CenterItemIndex}, VerticalOffset: {e.VerticalOffset}");
        }

        [RelayCommand]
        private void RequestChapters()
        {
            Debug.WriteLine("=====================================================");
            Debug.WriteLine(Current.LastLine);
            Debug.WriteLine("=====================================================");
        }

        List<string> contents = new List<string>();
       
        private void Render()
        {
            var start = Current.FirstLine;
            var end = Current.LastLine;
            var count = end - start + 20;
            var needToRender = contents.Skip(start).Take(count);
            Chapters.Clear();
            foreach (var item in needToRender)
            {
                Chapters.Add(new Content(item));
            }
        }

        public override void OnNavigatedTo()
        {
            Chapters.Clear();
            contents.Clear();
            using var fs = fileService.OpenFile(Current.FilePath);
            TxtHandler.Solve(fs, contents);
            //Current.BookSize = contents.Count;
            //await bookService.UpdateBookInfo(Current);
            Render();
            //await Render(caches);
            //OnContentLoaded?.Invoke(Current.Progress);
        }

        public override async void OnNavigatedFrom()
        {
            Chapters.Clear();
            await bookService.UpdateBookProgress(Current);
        }
    }
}
