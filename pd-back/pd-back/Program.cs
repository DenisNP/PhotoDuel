using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace PhotoDuel
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            StartServer();
        }

        private static void StartServer()
        {
            new WebHostBuilder()
                .UseKestrel()
                .UseStartup<Startup>()
                .ConfigureLogging(build =>
                {
                    build
                        .AddFilter("Microsoft", LogLevel.Warning)
                        .AddFilter("System", LogLevel.Warning)
                        .AddConsole();
                })
                .Build()
                .Run();
        }
    }
}