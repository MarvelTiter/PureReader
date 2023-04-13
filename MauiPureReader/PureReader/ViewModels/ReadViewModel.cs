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
        private const int MAX_CONTENT = 50;
        private const int APPEND_COUNT = 10;
        [ObservableProperty]
        private string currentChapter;
        [ObservableProperty]
        private string progress;
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
            if (deletingOnHead || deletingOnTail) return;
            if (e.FirstVisibleItemIndex != preIndex)
            {
                preIndex = e.FirstVisibleItemIndex;
                Current.LineCursor = Contents[e.FirstVisibleItemIndex].LineIndex;
                //Progress = Current.FormatProgress;
                if (e.VerticalDelta > 0)
                {
                    CheckRemainAndLoad(e.LastVisibleItemIndex, e.VerticalDelta);
                }
                else
                {
                    CheckPreviousAndLoad(e.FirstVisibleItemIndex, e.VerticalDelta);
                }
            }
        }

        [RelayCommand]
        private void Tap()
        {

        }
        [RelayCommand]
        private void Swipe()
        {

        }
        /// <summary>
        /// 检查剩余多少项，并按需加载
        /// </summary>
        /// <param name="lastVisibleItemIndex"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void CheckRemainAndLoad(int lastVisibleItemIndex, double delta)
        {
            if (ContentCount - (lastVisibleItemIndex + 1) < REMAIN_COUNT)
            {
                RenderForward(APPEND_COUNT);
                if (ContentCount > MAX_CONTENT)
                {
                    RemoveAtHead(APPEND_COUNT);
                }
            }
        }
        private void CheckPreviousAndLoad(int firstVisibleItemIndex, double delta)
        {
            if (firstVisibleItemIndex + 1 <= REMAIN_COUNT)
            {
                RenderPrevious(APPEND_COUNT);
                if (ContentCount > MAX_CONTENT)
                {
                    RemoveAtTail(APPEND_COUNT);
                }
            }
        }
        bool deletingOnHead;
        void RemoveAtHead(int count)
        {
            deletingOnHead = true;
            for (int i = 0; i < count; i++)
            {
                Contents.RemoveAt(0);
            }
            cache.FixedIndex(true, count);
            deletingOnHead = false;
        }
        bool deletingOnTail;
        void RemoveAtTail(int count)
        {
            deletingOnTail = true;
            for (int i = 0; i < count; i++)
            {
                Contents.RemoveAt(ContentCount - 1);
            }
            cache.FixedIndex(false, count);
            deletingOnTail = false;
        }

        private void RenderForward(int appendCount)
        {
            var needToRender = cache.LoadForwardContent(appendCount);
            foreach (var item in needToRender)
            {
                Contents.Add(item);
            }
        }

        private void RenderPrevious(int insertCount)
        {
            var needToRender = cache.LoadPreviousContent(insertCount);
            foreach (var item in needToRender)
            {
                Contents.Insert(0, item);
            }
        }

        CacheContentManager cache;
        public override async Task OnNavigatedTo()
        {
            Contents.Clear();
            if (!Current.Done)
            {
                // 继续解析
                source = new CancellationTokenSource();
                _ = fileHandler.Solve(Current, source.Token);
            }
            cache = new CacheContentManager(Current, bookService);
            var contents = await cache.Init();
            foreach (var item in contents)
            {
                Contents.Add(item);
            }
        }

        public override async Task OnNavigatedFrom()
        {
            Contents.Clear();
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
