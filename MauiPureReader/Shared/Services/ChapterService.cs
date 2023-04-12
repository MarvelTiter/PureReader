using MDbContext.ExpressionSql;
using Shared.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Services
{
    public class ChapterService
    {
        private readonly IExpressionContext context;

        public ChapterService(IExpressionContext context)
        {
            this.context = context;
        }

        public Task<IList<Chapter>> GetChapters(string bookId)
        {
            return context.Select<Chapter>().Where(c => c.BookId == bookId).ToListAsync();
        }

        public Task<Chapter> GetLatestChapter(string bookId)
        {
            //context.Select<Chapter>().Where(c => c.BookId == bookId).OrderBy(c => c.Id).
            throw new NotImplementedException();
        }
    }
}
