using DExpSql;
using MDbContext.ExpressionSql;
using Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Services
{
    public partial class BookService
    {
        private readonly IExpressionContext context;

        public BookService(IExpressionContext context)
        {
            this.context = context;
        }

        public Task<IList<Book>> GetBooks()
        {
            return context.Select<Book>().ToListAsync();
        }

        public async Task AddBook(Book book)
        {
            await context.Insert<Book>().AppendData(book).ExecuteAsync();
        }

        public Task UpdateBookInfo(Book book)
        {
            return context.Update<Book>()
                .Set(b => b.BookSize, book.BookSize)
                .Set(b => b.Offset, book.Offset)
                .Set(b=>b.Lines, book.Lines)
                .Where(b => b.Id == book.Id).ExecuteAsync();
        }

        public Task UpdateBookProgress(Book book)
        {
            return context.Update<Book>().Set(b => b.LineCursor, book.LineCursor).Where(b => b.Id == book.Id).ExecuteAsync();
        }

        public async Task<IList<Content>> GetBookContents(string bookId, int start, int count)
        {
            try
            {
                var end = start + count;
                var contents = await context.Select<Content>()
                    .Where(c => c.BookId == bookId && c.LineIndex >= start && c.LineIndex < end)
                    .OrderBy(c => c.LineIndex, true)
                    .ToListAsync();
                return contents;
            }
            catch (Exception)
            {
                return await GetBookContents(bookId, start, count);
            }
            
        }

        public Task<IList<Content>> GetAllContents(Book book)
        {
            return context.Select<Content>().Where(b => b.BookId == book.Id).ToListAsync();
        }

        public Task<int> SaveContents(IEnumerable<Content> contents)
        {
            return context.Insert<Content>().AppendData(contents).ExecuteAsync();
        }

        public async Task DeleteBookAsync(Book book)
        {
            var trans = context.BeginTransaction();
            trans.Delete<Book>().Where(b => b.Id == book.Id).AttachTransaction();
            trans.Delete<Content>().Where(c => c.BookId == book.Id).AttachTransaction();
            await trans.CommitTransactionAsync();
        }
    }
}
