using Dfc.ProviderPortal.Courses;
using Dfc.ProviderPortal.Courses.Helpers;
using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Courses.Services;
using Dfc.ProviderPortal.Courses.Settings;
using Dfc.ProviderPortal.Packages.AzureFunctions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;

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
            builder.Services.Configure<ReferenceDataServiceSettings>(configuration.GetSection(nameof(ReferenceDataServiceSettings)));
            builder.Services.AddScoped<ICosmosDbHelper, CosmosDbHelper>();
            builder.Services.AddScoped<ISearchServiceWrapper, SearchServiceWrapper>();
            builder.Services.AddScoped<ICourseService, CoursesService>();
            builder.Services.AddScoped<ICourseMigrationReportService, CourseMigrationReportService>();
            builder.Services.AddScoped<IDfcReportService, DfcReportService>();
            builder.Services.AddSingleton<ICosmosDbSettings, CosmosDbSettings>();
            builder.Services.AddSingleton<IReferenceDataServiceSettings, ReferenceDataServiceSettings>();
            builder.Services.AddTransient((provider) => new HttpClient());
            builder.Services.AddSingleton<IProviderServiceSettings>(sp => sp.GetRequiredService<IOptions<ProviderServiceSettings>>().Value);
            builder.Services.AddSingleton<IQualificationServiceSettings>(sp => sp.GetRequiredService<IOptions<QualificationServiceSettings>>().Value);
            builder.Services.AddSingleton<IVenueServiceSettings>(sp => sp.GetRequiredService<IOptions<VenueServiceSettings>>().Value);
            builder.Services.AddSingleton<IReferenceDataServiceSettings>(sp => sp.GetRequiredService<IOptions<ReferenceDataServiceSettings>>().Value);
            builder.Services.AddSingleton<ProviderServiceWrapper>();
            builder.Services.AddSingleton<QualificationServiceWrapper>();
            builder.Services.AddSingleton<VenueServiceWrapper>();
            builder.Services.AddSingleton<FeChoiceServiceWrapper>();

            var serviceProvider = builder.Services.BuildServiceProvider();
            serviceProvider.GetService<ICourseMigrationReportService>().Initialise().Wait();
            serviceProvider.GetService<IDfcReportService>().Initialise().Wait();
            //serviceProvider.GetService<ISearchServiceWrapper>().Initialise().Wait();
        }
    }
}