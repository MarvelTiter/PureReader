using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Behaviors;
using MDbContext;
using Microsoft.Data.Sqlite;
using PureReader.ViewModels;
using PureReader.Views;
using Shared.Data;
using Shared.Services;
using System.Linq.Expressions;
using System.Text;

namespace PureReader
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.Services.AddLightOrm(option =>
            {
                option.SetDatabase(DbBaseType.Sqlite, () =>
                {
                    return new SqliteConnection(Constants.DbConnectString);
                }).InitializedContext<InitContext>();
            });
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            builder.Services.RegisterViews().RegisterServices();

            return builder.Build();
        }

        private static IServiceCollection RegisterViews(this IServiceCollection services)
        {
            return services
                .AddNavigableView<BookshelfView, BookshelfViewModel>()
                .AddNavigableView<ReadView, ReadViewModel>()
                .AddNavigableView<SettingView, SettingViewModel>();
        }

        private static IServiceCollection AddNavigableView<View, ViewModel>(this IServiceCollection services) where View : ContentPage where ViewModel : class
        {
            services.AddScoped<ViewModel>();
            services.AddTransient(provider =>
            {
                var viewModel = provider.GetService<ViewModel>();
                var ctor = typeof(View).GetConstructor(new Type[] {typeof(ViewModel)});                
                var page = (View)ctor.Invoke(new object[] { viewModel });
                if (viewModel is INavigable navigable)
                {
                    page.Behaviors.Add(new EventToCommandBehavior
                    {
                        EventName = nameof(ContentPage.NavigatedTo),
                        Command = navigable.NavigatedToCommand,
                    });
                    page.Behaviors.Add(new EventToCommandBehavior
                    {
                        EventName = nameof(ContentPage.NavigatedFrom),
                        Command = navigable.NavigatedFromCommand,
                    });
                }
                return page;
            });
            return services;
        }

        private static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            return services
                .AddSingleton<BookService>()
                .AddSingleton<NavigationService>()
                .AddSingleton<FileService>();
        }

    }
}