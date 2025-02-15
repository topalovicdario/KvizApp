using KvizApp.Models;
using KvizApp.Services;
using KvizApp.ViewModels;
using KvizApp.Views;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

namespace KvizApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            // Register services
           
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddSingleton<IQuizService, QuizService>();
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<ICreatable, OnlineGame>();
            builder.Services.AddSingleton<CreateGameSession>();
            builder.Services.AddSingleton<CreateGameSessionViewModel>();

            // Zamijenite sa stvarnom implementacijom
            builder.Services.AddTransient<AnswerViewModel>();
            builder.Services.AddTransient<AnswerPage>();

            // Add HttpClient if AuthService uses it


            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    
                })
                .ConfigureLifecycleEvents(events =>
                {
#if WINDOWS
                    events.AddWindows(windows => windows.OnWindowCreated(window =>
                    {
                        var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
                        var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);
                        var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
                        appWindow.Resize(new Windows.Graphics.SizeInt32(1500,800 ));
                    }));
#endif
                });

            // Add logging (optional)


            return builder.Build();
        }
    }
}