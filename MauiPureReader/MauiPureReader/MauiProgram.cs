using Microsoft.AspNetCore.Components.WebView.Maui;
using MauiPureReader.Data;
using MauiPureReader.Services;
using MDbContext;
using Microsoft.Data.Sqlite;

namespace MauiPureReader;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
#endif
        builder.Services.AddMasaBlazor();
        builder.Services.AddSingleton<ThemeService>();

        builder.Services.AddLightOrm(option =>
        {
            option.SetDatabase(DbBaseType.Sqlite, () =>
            {
                return new SqliteConnection(Constants.DbConnectString);
            }).InitializedContext<InitContext>();
        });

        builder.Services.AddScoped<BookService>();

        return builder.Build();
    }
}
