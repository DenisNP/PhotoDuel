using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using PhotoDuel.Models;
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
            services.AddScoped<DuelService>();
        }

        public void Configure(IApplicationBuilder app, IDbService dbService)
        {
            app.UseRouting();
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            
            dbService.Init("photoduel", type =>
            {
                if (type == typeof(Duel)) return "duels";
                if (type == typeof(User)) return "users";
                if (type == typeof(Report)) return "reports";
                throw new ArgumentOutOfRangeException(nameof(type), $"No collection for type: {type.FullName}");
            });
        }
    }
}