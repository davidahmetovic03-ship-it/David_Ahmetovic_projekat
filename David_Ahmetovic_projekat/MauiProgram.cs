using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;

namespace David_Ahmetovic_projekat
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseLocalNotification() // Dodavanje podrške za notifikacije
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Registracija servisa i stranica (prije build-a)
            builder.Services.AddSingleton<Services.DatabaseService>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<SubjectsPage>();
            builder.Services.AddTransient<SchedulePage>();
            builder.Services.AddTransient<CalendarPage>();
            builder.Services.AddTransient<AbsencesPage>();

            return builder.Build();
        }
    }
}
