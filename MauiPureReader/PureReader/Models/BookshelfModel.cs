using Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureReader.Models
{
    public class BookshelfModel
    {
        private readonly BookService bookService;

        public BookshelfModel(BookService bookService)
        {
            this.bookService = bookService;
        }

        public string Service => bookService.GetType().Name;
    }
}
