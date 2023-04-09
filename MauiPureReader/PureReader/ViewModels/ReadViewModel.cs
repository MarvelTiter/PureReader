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
        [ObservableProperty]
        bool loading;
        private const int REMAIN_COUNT = 10;
        private const int MAX_CONTENT = 100;

        Rect ViewRect => Shell.Current.CurrentPage.Bounds;
        int ContentCount => Contents.Count;
        public ReadViewModel(FileService fileService, BookService bookService)
        {
            this.fileService = fileService;
            this.bookService = bookService;
            Contents = new ObservableCollection<Content>();
        }

        int preIndex = -1;
        [RelayCommand]
        private void HandleScroll(ItemsViewScrolledEventArgs e)
        {
            //Debug.WriteLine("=========================Scroll============================");
            //Debug.WriteLine($"VerticalOffset: {e.VerticalOffset}, VerticalDelta: {e.VerticalDelta}");
            if (DeletingOnHead) return;
            if (e.FirstVisibleItemIndex != preIndex)
            {
                //Debug.WriteLine($"FirstVisibleItemIndex: {e.FirstVisibleItemIndex}, LastVisibleItemIndex: {e.LastVisibleItemIndex}");
                preIndex = e.FirstVisibleItemIndex;
                if (e.VerticalDelta > 0)
                {
                    Current.LineCursor++;
                    CheckRemainAndLoad(e.LastVisibleItemIndex);
                }
                else
                {
                    Current.LineCursor--;
                    CheckPreviousAndLoad(e.FirstVisibleItemIndex);
                }
                //Debug.WriteLine($"LineCursor: {Current.LineCursor}, ContentCount: {ContentCount}");
            }
        }
        /// <summary>
        /// 检查剩余多少项，并按需加载
        /// </summary>
        /// <param name="lastVisibleItemIndex"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void CheckRemainAndLoad(int lastVisibleItemIndex)
        {
            if (ContentCount - (lastVisibleItemIndex + 1) < REMAIN_COUNT)
            {
                RenderForward(10);
                if (ContentCount > MAX_CONTENT)
                {
                    RemoveAtHead(10);
                }
            }
        }
        private void CheckPreviousAndLoad(int firstVisibleItemIndex)
        {
            if (firstVisibleItemIndex + 1 <= REMAIN_COUNT)
            {
                RenderPrevious(10);
                if (ContentCount > MAX_CONTENT)
                {
                    RemoveAtTail(10);
                }
            }
        }
        bool DeletingOnHead;
        void RemoveAtHead(int count)
        {
            DeletingOnHead = true;
            for (int i = 0; i < count; i++)
            {
                Contents.RemoveAt(0);
            }
            previousOffset += 10;
            DeletingOnHead = false;
        }

        void RemoveAtTail(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Contents.RemoveAt(ContentCount - 1);
            }
        }

        //List<string> caches = new List<string>();
        List<Content> caches = new List<Content>();

        private void RenderForward(int appendCount = 50)
        {
            var start = forwardOffset;
            var count = appendCount;
            var needToRender = caches.Skip(start).Take(count);
            foreach (var item in needToRender)
            {
                //Contents.Add(new Content(item));
                Contents.Add(item);
            }
            forwardOffset += count;
        }

        private void RenderPrevious(int insertCount)
        {
            if (previousOffset <= 0) return;
            var start = previousOffset - insertCount;
            var count = start < 0 ? insertCount + start : insertCount;
            var needToRender = caches.Skip(start).Take(count).Reverse();
            foreach (var item in needToRender)
            {
                //Contents.Insert(0, new Content(item));
                Contents.Insert(0, item);
            }
            previousOffset -= 10;
        }

        int forwardOffset;
        int previousOffset;
        public override async Task OnNavigatedTo()
        {
            caches.Clear();
            Contents.Clear();
            forwardOffset = Current.LineCursor;
            previousOffset = Current.LineCursor;
            if (!Current.Done)
            {
                // 继续解析
            }
            using var fs = fileService.OpenFile(Current.FilePath);
            var watch = Stopwatch.StartNew();
            await TxtHandler.Solve(fs, caches);
            //Contents.Add(new Content($"{watch.Elapsed}"));
            //var cts = new ObservableCollection<Content>();
            //await TxtHandler.Solve(fs, cts);
            //Current.BookSize = caches.Count;
            //await bookService.UpdateBookInfo(Current);
            //RenderForward();
            Loading = false;
            //await Render(caches);
            //OnContentLoaded?.Invoke(Current.Progress);
        }

        public override async Task OnNavigatedFrom()
        {
            caches.Clear();
            Contents.Clear();
            Loading = true;
            await bookService.UpdateBookProgress(Current);
        }
    }
}
