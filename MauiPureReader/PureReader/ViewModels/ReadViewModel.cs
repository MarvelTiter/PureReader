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
        private ObservableCollection<Content> contents;
        Rect ViewRect => Shell.Current.CurrentPage.Bounds;
        public ReadViewModel(FileService fileService, BookService bookService)
        {
            this.fileService = fileService;
            this.bookService = bookService;
            Contents = new ObservableCollection<Content>();
        }

        [RelayCommand]
        private void Tap()
        {
            Contents.RemoveAt(0);

        }

        [RelayCommand]
        private void Swipe()
        {
            Debug.WriteLine("Swipe....");
        }

        int preIndex = 0;
        [RelayCommand]
        private void HandleScroll(ItemsViewScrolledEventArgs e)
        {
            //Debug.WriteLine("=========================Scroll============================");
            //Debug.WriteLine($"VerticalOffset: {e.VerticalOffset}, VerticalDelta: {e.VerticalDelta}");
            //Debug.WriteLine($"FirstVisibleItemIndex: {e.FirstVisibleItemIndex}, LastVisibleItemIndex: {e.LastVisibleItemIndex}");

            //Current.FirstLine = e.FirstVisibleItemIndex;
            //Current.LastLine = e.LastVisibleItemIndex;
            if (e.VerticalOffset > ViewRect.Height / 2 && e.VerticalDelta > 0)
            {
                preIndex += e.CenterItemIndex;
                Contents.RemoveAt(0);
                Contents.RemoveAt(0);
                Contents.RemoveAt(0);
                Contents.RemoveAt(0);
                Contents.RemoveAt(0);
                Render(5);
            }
            //Render();
            //Debug.WriteLine($"CenterItemIndex:{e.CenterItemIndex}, VerticalOffset: {e.VerticalOffset}");
        }


        [RelayCommand]
        private void RequestChapters()
        {
            //Debug.WriteLine("========================RemainingItemsThresholdReached=============================");
            //Debug.WriteLine(Current.LastLine);
            //Debug.WriteLine("=====================================================");
        }

        [RelayCommand]
        private void LoadPrevious()
        {
            //Debug.WriteLine("==========================Pull Refresh===========================");
            //Debug.WriteLine(Current.LastLine);
            //Debug.WriteLine("=====================================================");
        }

        List<string> caches = new List<string>();

        private void Render(int appendCount = 100)
        {
            var start = preIndex;
            var count = start + appendCount;
            var needToRender = caches.Skip(start).Take(count);
            foreach (var item in needToRender)
            {
                Contents.Add(new Content(item));
            }
        }

        public override void OnNavigatedTo()
        {
            caches.Clear();
            Contents.Clear();
            using var fs = fileService.OpenFile(Current.FilePath);
            TxtHandler.Solve(fs, caches);
            //Current.BookSize = contents.Count;
            //await bookService.UpdateBookInfo(Current);
            Render();
            //await Render(caches);
            //OnContentLoaded?.Invoke(Current.Progress);
        }

        public override async void OnNavigatedFrom()
        {
            await bookService.UpdateBookProgress(Current);
        }
    }
}
