using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiPureReader.Pages
{
    public partial class BookView
    {
        [Parameter]
        public int Id { get; set; }
        bool showAppbar;
    }
}
