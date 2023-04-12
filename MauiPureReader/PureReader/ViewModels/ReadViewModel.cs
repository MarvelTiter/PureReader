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
        private ObservableCollection<Content> contents;
        [ObservableProperty]
        private bool loading;
        private const int REMAIN_COUNT = 10;
        private const int MAX_CONTENT = 100;

        Rect ViewRect => Shell.Current.CurrentPage.Bounds;
        int ContentCount => Contents.Count;
        CancellationTokenSource source;
        public ReadViewModel(IFileHandler fileHandler, BookService bookService)
        {
            this.fileHandler = fileHandler;
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

        private void FirstRender()
        {
            Loading = true;
            Task.Run(async () =>
            {
                var start = forwardOffset;
            reset:
                //var needToRender = caches.Skip(start).Take(count);
                var needToRender = await bookService.GetBookContents(Current.Id, start, 30);
                if (needToRender.Count < 30)
                {
                    goto reset;
                }
                foreach (var item in needToRender)
                {
                    //Contents.Add(new Content(item));
                    Contents.Add(item);
                }
                forwardOffset += 30;
                Loading = false;
            });
        }
        private async void RenderForward(int appendCount)
        {
            if (forwardOffset >= Current.BookSize - 1) return;
            var start = forwardOffset;
            var count = appendCount;
            var needToRender = await bookService.GetBookContents(Current.Id, start, count);
            foreach (var item in needToRender)
            {
                Contents.Add(item);
            }
            forwardOffset = needToRender.LastOrDefault()?.LineIndex ?? Current.BookSize - 1;
        }

        private async void RenderPrevious(int insertCount)
        {
            if (previousOffset <= 0) return;
            var start = previousOffset - insertCount;
            var count = start < 0 ? insertCount + start : insertCount;
            start = start < 0 ? 0 : start;
            var needToRender = await bookService.GetBookContents(Current.Id, start, count);
            foreach (var item in needToRender.Reverse())
            {
                Contents.Insert(0, item);
            }
            previousOffset = needToRender.FirstOrDefault()?.LineIndex ?? 0;
        }

        int forwardOffset;
        int previousOffset;
        Task solveTask;
        public override Task OnNavigatedTo()
        {
            Contents.Clear();
            forwardOffset = Current.LineCursor;
            previousOffset = Current.LineCursor;
            source = new CancellationTokenSource();
            if (!Current.Done)
            {
                // 继续解析
                solveTask = fileHandler.Solve(Current, source.Token);
            }
            FirstRender();
            return Task.CompletedTask;
        }


        public override async Task OnNavigatedFrom()
        {
            Contents.Clear();
            source?.Cancel();
            source?.Dispose();
            if (solveTask != null) await solveTask;
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
                solveTask = fileHandler.Solve(Current, source.Token);
            }
            return Task.CompletedTask;
        }
    }
}
