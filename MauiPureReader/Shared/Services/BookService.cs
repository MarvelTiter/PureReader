using MDbContext.ExpressionSql;
using Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Services
{
    public class BookService
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
            await context.Insert<Book>().AppendData(book).IgnoreColumns(b => b.Id).ExecuteAsync();
        }

        public Task UpdateBookInfo(Book book)
        {
            return context.Update<Book>().Set(b => b.BookSize, book.BookSize).Where(b => b.Id == book.Id).ExecuteAsync();
        }

        public Task UpdateBookProgress(Book book)
        {
            return context.Update<Book>().Set(b => b.LineCursor, book.LineCursor).Where(b => b.Id == book.Id).ExecuteAsync();
        }
    }
}
