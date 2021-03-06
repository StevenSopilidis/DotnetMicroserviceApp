using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Http;

namespace PlatformService
{
    public class Startup
    {
        public Startup(IConfiguration configuration,IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }
        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment _env;
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();

            if(_env.IsProduction())
            {
                Console.WriteLine("Using SQL SERVER DB");
                string dbPassword = Configuration["DB_PASSWORD"];
                string connectionString = Configuration.GetConnectionString("PlatformsConn");
                connectionString.Replace("PASSWORD", dbPassword);
                services.AddDbContext<AppDbContext>(opts => opts.UseSqlServer(connectionString));
            }else {
                Console.WriteLine("Using InMem DB");
                services.AddDbContext<AppDbContext>(opts => opts.UseInMemoryDatabase("InMem"));
            }
            
            services.AddScoped<IPlatformRepo, PlatformRepo>();
            //make use of AddHttpClient so we can make use of HttpClientFactory
            services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
            services.AddSingleton<IMessageBusClient, MessageBusClient>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PlatformService", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PlatformService v1"));
            }

            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            PrepDb.PrepPopulation(app, _env.IsProduction());
        }
    }
}
