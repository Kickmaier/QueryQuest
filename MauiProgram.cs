using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QueryQuest.Data;

namespace QueryQuest
{
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
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.Configuration.AddUserSecrets<MainPage>();
            builder.Services.AddSingleton<MongoDbConnection>();
            builder.Services.AddSingleton<OpenTriviaDbApiConnection>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
