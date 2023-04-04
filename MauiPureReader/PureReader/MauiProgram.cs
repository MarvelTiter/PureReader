using CommunityToolkit.Maui;
using MDbContext;
using Microsoft.Data.Sqlite;
using PureReader.ViewModels;
using PureReader.Views;
using Shared.Data;
using Shared.Services;
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
                .AddTransient<BookshelfView>().AddTransient<BookshelfViewModel>()
                .AddTransient<ReadView>().AddTransient<ReadViewModel>()             
                .AddTransient<SettingView>().AddTransient<SettingViewModel>();
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