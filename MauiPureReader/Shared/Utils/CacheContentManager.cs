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
        private IList<Content> caches;
        int startIndex;
        int endIndex;
        const int CACHE_SIZE = 100;
        const int REMAIN_SIZE = 20;
        public CacheContentManager(Book book, BookService service)
        {
            this.book = book;
            this.service = service;
            preCursor = book.LineCursor;
        }

        public async Task<bool> LoadContentsAsync(int cursor, CancellationToken token)
        {
            if (token.IsCancellationRequested) return false;
            // 往前读40，往后读60
            var contents = await service.GetBookContents(book.Id, cursor - 40, CACHE_SIZE, token);
            if (contents.Count < CACHE_SIZE)
            {
                return await LoadContentsAsync(cursor, token);
            }
            else
            {
                caches = contents;
                startIndex = caches.FirstOrDefault()?.LineIndex ?? 0;
                endIndex = caches.LastOrDefault()?.LineIndex ?? 0;
                return true;
            }
        }
        public IEnumerable<Content> GetContents(int start)
        {
            if (caches == null)
            {
                yield break;
            }
            foreach (var item in caches.Where(c => c.LineIndex >= start))
            {
                yield return item;
            }
        }

        public Content GetSingle(int index)
        {
            return caches.FirstOrDefault(c => c.LineIndex == index);
        }

        CancellationTokenSource source = new CancellationTokenSource();
        int preCursor = -1;
        public void CheckCacheCapacity(int cursor)
        {
            if (cursor != preCursor && Math.Abs(cursor - preCursor) > 5)
            {
                preCursor = cursor;
                if (cursor - startIndex < REMAIN_SIZE || endIndex - cursor < REMAIN_SIZE + 20)
                {
                    source?.Cancel();
                    source?.Dispose();
                    source = new CancellationTokenSource();
                    _ = LoadContentsAsync(cursor, source.Token);
                }
            }
        }
    }
}
