using Shared.Data;
using Shared.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Utils
{
    public class CacheContentManager
    {
        private readonly Book book;
        private readonly BookService service;
        private Queue<Content> forward;
        private Stack<Content> previous;
        private IList<Content> caches;
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
            Init();
        }

        public async void Init()
        {
            var contents = await service.GetBookContents(book.Id, book.LineCursor, CACHE_SIZE);
            if (contents.Count < CACHE_SIZE)
            {
                Init();
            }
            else
            {
                caches = contents;
            }
        }

        private void CheckForwardCache()
        {
            TaskHelper.Run(nameof(CheckForwardCache), async token =>
            {
                while (forward.Count < CACHE_LIMIT && forwardIndex < book.Lines)
                {
                    if (token.IsCancellationRequested) break;
                    var contents = await service.GetBookContents(book.Id, forwardIndex, CACHE_SIZE);
                    forwardIndex = contents.Last().LineIndex + 1;
                    foreach (var item in contents)
                    {
                        forward.Enqueue(item);
                    }
                }
            });
        }

        public IEnumerable<Content> GetContents(int start)
        {
            if (caches == null) yield break;
            foreach (var item in caches.Where(c => c.LineIndex >= start))
            {
                yield return item;
            }
        }

        public bool GetForward(out Content forward)
        {
            if (this.forward.Count > 0)
            {
                forward = this.forward.Dequeue();
                CheckForwardCache();
                return true;
            }
            else
            {
                forward = null;
                return false;
            }
        }

        private void CheckPreviousCache()
        {
            TaskHelper.Run(nameof(CheckPreviousCache), async token =>
            {
                while (previous.Count < CACHE_SIZE && previousIndex > 0)
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
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
    }
}
