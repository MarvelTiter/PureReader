using MauiPureReader.Data;
using MDbContext.ExpressionSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiPureReader.Services
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
    }
}
