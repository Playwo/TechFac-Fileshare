using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Fileshare.Authentication;
using Fileshare.Extensions;
using Fileshare.Models;
using Microsoft.AspNetCore.HttpOverrides;

namespace Fileshare
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //SERVICES
            services.AddServices();
            services.AddSingleton<Random>();

            //DB
            services.AddDbContext<UploaderContext>(options =>
            {
                //options.UseInMemoryDatabase("Local");
                options.UseNpgsql(Configuration.GetConnectionString());
                options.EnableSensitiveDataLogging();
            },
            ServiceLifetime.Transient, ServiceLifetime.Transient);

            //ASP
            services.AddControllers();
            services.AddRazorPages();
            services.AddAuthentication()
                .AddScheme<KeyAuthOptions, KeyAuthHandler>("Key", x => x.ApiKey = Configuration.GetApiKey())
                .AddScheme<BasicAuthOptions, BasicAuthHandler>("Basic", null)
                .AddScheme<BearerAuthOptions, BearerAuthHandler>("Bearer", null);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Task.Run(() => app.ApplicationServices.RunServicesAsync());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions()
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}
