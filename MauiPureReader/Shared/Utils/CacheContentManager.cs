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
                CheckCachePool();
                return contents;
            }
        }

        private void CheckCachePool()
        {
            TaskHelper.Run(nameof(CheckCachePool), async () =>
            {
                if (forward.Count < 20 && forwardIndex < book.Lines)
                {
                    var contents = await service.GetBookContents(book.Id, forwardIndex, 50);
                    forwardIndex = contents.Last().LineIndex + 1;
                    foreach (var item in contents)
                    {
                        forward.Enqueue(item);
                    }
                }

                if (previous.Count < 20 && previousIndex > 0)
                {
                    var start = previousIndex - 50;
                    var count = start < 0 ? start + 50 : 50;
                    var contents = await service.GetBookContents(book.Id, start, count);
                    previousIndex = contents.FirstOrDefault()?.LineIndex ?? 0;
                    IList<Content> old = previous.ToList();
                    old.Reverse();
                    var newList = contents.Concat(old);
                    previous = new Stack<Content>(newList);
                }
            });
        }

        private void UpdatePreviousCache()
        {
            TaskHelper.Run(nameof(UpdatePreviousCache), async () =>
            {
                var start = previousIndex - 20;
                var count = start < 0 ? start + 20 : 20;
                var contents = await service.GetBookContents(book.Id, start, count);
                previous.Clear();
                foreach (var item in contents)
                {
                    previous.Push(item);
                }
            });
        }


        /// <summary>
        /// 检查缓存是否足够
        /// </summary>
        private void CheckForwardCache()
        {

        }

        private void UpdateForwardCache()
        {
            TaskHelper.Run(nameof(UpdateForwardCache), async () =>
            {
                var contents = await service.GetBookContents(book.Id, forwardIndex, 20);
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
            CheckCachePool();
        }

        public IEnumerable<Content> LoadPreviousContent(int count)
        {
            while (previous.Count > 0 && count > 0)
            {
                yield return previous.Pop();
                count--;
            }
            CheckCachePool();
        }

        internal void FixedIndex(bool head, int count)
        {
            if (head)
            {
                /*
                 * 往下翻，更新前面缓存
                 */
                previousIndex += count;
                UpdatePreviousCache();
            }
            else
            {
                /*
                 * 往上翻，更新后面的缓存
                 */
                forwardIndex -= count;
                UpdateForwardCache();
            }
        }
    }
}
