using Shared.Data;
using Shared.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Utils
{
    internal class CacheContentManager
    {
        private readonly Book book;
        private readonly BookService service;
        private Queue<Content> forward;
        private Stack<Content> previous;
        int forwardIndex;
        int previousIndex;
        const int FIRST_LOAD = 50;
        const int CACHE_SIZE = 50;
        const int ONCE_PICK_COUNT = 10;
        const int CACHE_LIMIT = 20;
        public CacheContentManager(Book book, BookService service)
        {
            this.book = book;
            this.service = service;
            forwardIndex = book.LineCursor;
            previousIndex = book.LineCursor;
            forward = new Queue<Content>();
            previous = new Stack<Content>();
        }

        public async Task<IList<Content>> Init()
        {
            var contents = await service.GetBookContents(book.Id, forwardIndex, FIRST_LOAD);
            if (contents.Count < FIRST_LOAD)
            {
                await Task.Delay(100);
                return await Init();
            }
            else
            {
                forwardIndex += FIRST_LOAD;
                CheckPreviousCache();
                CheckForwardCache();
                return contents;
            }
        }

        private void CheckForwardCache()
        {
            TaskHelper.Run(nameof(CheckForwardCache), async token =>
            {
                if (forward.Count < CACHE_LIMIT && forwardIndex < book.Lines)
                {
                    var contents = await service.GetBookContents(book.Id, forwardIndex, CACHE_SIZE);
                    forwardIndex = contents.Last().LineIndex + 1;
                    foreach (var item in contents)
                    {
                        forward.Enqueue(item);
                    }
                }
            });
        }
        private void UpdateForwardCache()
        {
            TaskHelper.Run(nameof(UpdateForwardCache), async token =>
            {
                var contents = await service.GetBookContents(book.Id, forwardIndex, CACHE_SIZE);
                forwardIndex = contents.Last().LineIndex + 1;
                forward.Clear();
                foreach (var item in contents)
                {
                    forward.Enqueue(item);
                }
            });
        }
        public IEnumerable<Content> LoadForwardContent(int count)
        {
            while (forward.Count > 0 && count > 0)
            {
                yield return forward.Dequeue();
                count--;
            }
            CheckForwardCache();
        }

        private void CheckPreviousCache()
        {
            TaskHelper.Run(nameof(CheckPreviousCache), async token =>
            {
                if (previous.Count < CACHE_SIZE && previousIndex > 0)
                {
                    var old = previous.Reverse().ToList();
                    var start = previousIndex - CACHE_SIZE;
                    var count = start < 0 ? start + CACHE_SIZE : CACHE_SIZE;
                    var contents = await service.GetBookContents(book.Id, start, count);
                    previousIndex = contents.FirstOrDefault()?.LineIndex ?? 0;
                    previous.Clear();
                    foreach (var item in contents) previous.Push(item);
                    foreach (var item in old) previous.Push(item);
                }
            });
        }
        private void UpdatePreviousCache()
        {
            TaskHelper.Run(nameof(UpdatePreviousCache), async token =>
            {
                var start = previousIndex - CACHE_SIZE;
                var count = start < 0 ? start + CACHE_SIZE : CACHE_SIZE;
                var contents = await service.GetBookContents(book.Id, start, count);
                previous.Clear();
                foreach (var item in contents) previous.Push(item);
            });
        }
        public IEnumerable<Content> LoadPreviousContent(int count)
        {
            while (previous.Count > 0 && count > 0)
            {
                yield return previous.Pop();
                count--;
            }
            CheckPreviousCache();
        }

        internal void FixedIndex(bool head, int index)
        {
            if (head)
            {
                /*
                 * 往下翻，更新前面缓存
                 */
                previousIndex = index;
                UpdatePreviousCache();
            }
            else
            {
                /*
                 * 往上翻，更新后面的缓存
                 */
                forwardIndex = index;
                UpdateForwardCache();
            }
        }
    }
}
