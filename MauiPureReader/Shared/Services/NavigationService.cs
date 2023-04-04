using CommunityToolkit.Mvvm.Input;
using PureReader.ViewModels;
using PureReader.Views;
using Shared.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Services
{
    public class NavigationService
    {
        public static RelayCommand GoBackCommand => new RelayCommand(async () =>
        {
            await Shell.Current.GoToAsync("..");
        });
        public Task NavigateToReadViewAsync(Book book)
        {
            var parameters = new Dictionary<string, object>
            {
                [nameof(ReadViewModel.Current)] = book
            };
            return Shell.Current.GoToAsync(nameof(ReadView), parameters);
        }

        public Task GoBackAsync()
        {
            return Shell.Current.GoToAsync("..");
        }
    }
}
