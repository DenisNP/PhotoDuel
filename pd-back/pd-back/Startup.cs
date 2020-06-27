using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PhotoDuel.Services;

namespace PhotoDuel
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; });

            services.AddSingleton<IDbService, MongoService>();
            services.AddScoped<UserService>();
        }

        public void Configure(IApplicationBuilder app, IDbService dbService, ILogger<Startup> logger)
        {
            app.UseRouting();
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            
            dbService.Init("photoduel", logger);
        }
    }
}