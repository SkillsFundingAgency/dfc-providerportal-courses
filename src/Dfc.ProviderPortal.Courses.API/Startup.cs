
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Dfc.ProviderPortal.Courses.Helpers;
using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Courses.Services;
using Dfc.ProviderPortal.Courses.Settings;
using Swashbuckle.AspNetCore.Swagger;


namespace Dfc.ProviderPortal.Courses.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = //configuration;
                new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new Info { Title = "Courses API", Version = "v1" }); });

            services.Configure<CosmosDbCollectionSettings>(Configuration.GetSection(nameof(CosmosDbCollectionSettings)))
                    .Configure<CosmosDbSettings>(Configuration.GetSection(nameof(CosmosDbSettings)))
                    .Configure<ProviderServiceSettings>(Configuration.GetSection(nameof(ProviderServiceSettings)))
                    .Configure<VenueServiceSettings>(Configuration.GetSection(nameof(VenueServiceSettings)))
                    .Configure<SearchServiceSettings>(Configuration.GetSection(nameof(SearchServiceSettings)))
                    .AddScoped<ICourseService, CoursesService>()
                    .AddScoped<ICosmosDbHelper, CosmosDbHelper>()
                    .AddScoped<IProviderServiceWrapper, ProviderServiceWrapper>()
                    .AddScoped<IVenueServiceWrapper, VenueServiceWrapper>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Courses API v1"); });
        }
    }
}
