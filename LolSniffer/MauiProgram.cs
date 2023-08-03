using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Hosting;
using LolSniffer.Services;
using System.Threading.Tasks;

namespace LolSniffer
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

            builder.Services.AddSingleton<LobbyService>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            var builtApp = builder.Build();
            RunLobbyService(builtApp.Services.GetService<LobbyService>());
            return builtApp;
        }

        private static void RunLobbyService(LobbyService lobbyService)
        {
            Task.Run(() => lobbyService.StartService());
        }
    }
}
