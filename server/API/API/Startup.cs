using API.Services;
using API.Services.ServiceBus;
using Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Services;
using Services.Extensions;
using Services.Options;
using System;

namespace API
{
    public class Startup
    {
        private const string OriginsName = "allowAllOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();
            services.AddControllers().AddNewtonsoftJson();
            services.AddCors();
            services.AddCors(options =>
            {
                options.AddPolicy(OriginsName,
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

            services.AddServices();
            services.AddDbContext<LocationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("Database"));
            });

            // comment if Azure Function is used to process the message instead of FileParseBackgroundService
            services.AddHostedService<FileParseBackgroundService>();
            services.Configure<AmazonOptions>(Configuration.GetSection("Amazon"));
            services.Configure<AzureBlobServiceOptions>(Configuration.GetSection("Azure"));
            services.Configure<AzureServiceBusOptions>(Configuration.GetSection("Azure"));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Location History API", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Location History API"); });
            app.UseRouting();
            app.UseCors(OriginsName);
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}