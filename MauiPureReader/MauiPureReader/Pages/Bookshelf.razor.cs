using Shared.Data;
using Shared.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiPureReader.Pages
{
    public partial class Bookshelf
    {
        [Inject] public BookService Service { get; set; }
        IList<Book> books;
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            books = await Service.GetBooks() ?? Enumerable.Empty<Book>().ToList();
        }
        private async Task AddBook()
        {
            await Service.AddBook(new Book
            {
                Title = "测试",
                FirstLine = 55,
            });
            books = await Service.GetBooks();
        }
    }
}
