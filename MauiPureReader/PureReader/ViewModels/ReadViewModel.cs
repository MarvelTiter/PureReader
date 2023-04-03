using CommunityToolkit.Mvvm.ComponentModel;
using Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureReader.ViewModels
{
    [QueryProperty(nameof(Current), nameof(Current))]
    public partial class ReadViewModel : ObservableObject
    {
        [ObservableProperty]
        private Book current;
    }
}
