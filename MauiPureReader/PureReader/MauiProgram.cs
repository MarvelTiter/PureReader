using CommunityToolkit.Maui;
using MDbContext;
using Microsoft.Data.Sqlite;
using PureReader.Models;
using PureReader.Views;
using Shared.Data;
using Shared.Services;

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
            builder.Services.AddTransient<Bookshelf>();
            builder.Services.AddTransient<BookshelfModel>();
            builder.Services.AddSingleton<BookService>();

            return builder.Build();
        }
    }
}