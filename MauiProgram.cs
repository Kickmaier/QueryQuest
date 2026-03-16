using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QueryQuest.Application.Services;
using QueryQuest.ViewModels;
using QueryQuest.Core.Interfaces;
using QueryQuest.Core.Models;
using QueryQuest.Infrastructure.Data;
using QueryQuest.Application.Interfaces;
using QueryQuest.Views;
using QueryQuest.Infrastructure.Repositories;
using QueryQuest.ViewModels.Models;


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
            //builder.Configuration.AddUserSecrets<MainPage>();
            builder.Services.AddSingleton<OpenTriviaDbApiConnection>();
            builder.Services.AddSingleton<IGameSettingsService, GameSettingsService>();
            builder.Services.AddSingleton<ITriviaService, OpenTriviaService>();
            builder.Services.AddSingleton<IHtmlService, HtmlService>();
            builder.Services.AddSingleton<IScoreHandler, ScoreHandler>();
            builder.Services.AddSingleton<BaseApiClient>();
            builder.Services.AddSingleton<HttpClient>();
            builder.Services.AddTransient<IQuestionService, QuestionManager>();
            builder.Services.AddTransient<ScoreHandler>();
            builder.Services.AddTransient<QuizUIState>();
            builder.Services.AddTransient<MainUIState>();
            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddTransient<QuizViewModel>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<QuizPage>();
            builder.Services.AddTransient<AppShell>();
            builder.Services.AddSingleton<App>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
