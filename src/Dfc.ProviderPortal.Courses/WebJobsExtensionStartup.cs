
using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Dfc.ProviderPortal.Courses;
using Dfc.ProviderPortal.Courses.Helpers;
using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Courses.Services;
using Dfc.ProviderPortal.Courses.Settings;
using Dfc.ProviderPortal.Packages.AzureFunctions.DependencyInjection;


[assembly: WebJobsStartup(typeof(WebJobsExtensionStartup), "Web Jobs Extension Startup")]

namespace Dfc.ProviderPortal.Courses
{
    public class WebJobsExtensionStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddDependencyInjection();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            builder.Services.AddSingleton<IConfiguration>(configuration);
            builder.Services.Configure<CosmosDbSettings>(configuration.GetSection(nameof(CosmosDbSettings)));
            builder.Services.Configure<CosmosDbCollectionSettings>(configuration.GetSection(nameof(CosmosDbCollectionSettings)));
            builder.Services.Configure<ProviderServiceSettings>(configuration.GetSection(nameof(ProviderServiceSettings)));
            builder.Services.Configure<VenueServiceSettings>(configuration.GetSection(nameof(VenueServiceSettings)));
            builder.Services.Configure<QualificationServiceSettings>(configuration.GetSection(nameof(QualificationServiceSettings)));
            builder.Services.Configure<SearchServiceSettings>(configuration.GetSection(nameof(SearchServiceSettings)));
            builder.Services.AddScoped<ICosmosDbHelper, CosmosDbHelper>();
            builder.Services.AddScoped<ISearchServiceWrapper, SearchServiceWrapper>();
            builder.Services.AddScoped<ICourseService, CoursesService>();
        }
    }
}
