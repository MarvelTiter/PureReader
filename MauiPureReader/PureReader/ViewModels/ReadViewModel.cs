using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PureReader.Base;
using Shared.Data;
using Shared.Services;
using Shared.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureReader.ViewModels
{
    [QueryProperty(nameof(Current), nameof(Current))]
    public partial class ReadViewModel : BaseViewModel
    {
        private readonly FileService fileService;
        private readonly NavigationService navigationService;
        [ObservableProperty]
        private Book current;
        [ObservableProperty]
        private string fileContent;
        [ObservableProperty]
        private ObservableCollection<Content> chapters;
        public ReadViewModel(FileService fileService, NavigationService navigationService)
        {
            this.fileService = fileService;
            this.navigationService = navigationService;
        }

        [RelayCommand]
        private void Tap()
        {

        }

        [RelayCommand]
        private void Swipe()
        {

        }

        protected override async void OnNavigateIn()
        {
            Chapters = new ObservableCollection<Content>();
            using var fs = fileService.OpenFile(Current.FilePath);
            await TxtHandler.Solve(fs, Chapters);
        }
    }
}
