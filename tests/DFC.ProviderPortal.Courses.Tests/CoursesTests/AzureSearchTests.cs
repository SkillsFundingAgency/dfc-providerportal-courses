
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Dfc.ProviderPortal.Courses.Functions;
using Dfc.ProviderPortal.Courses.Helpers;
using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Courses.Models;
using Dfc.ProviderPortal.Courses.Services;
using Dfc.ProviderPortal.Courses.Settings;
using DFC.ProviderPortal.Courses.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Xunit;
using Moq;


namespace DFC.ProviderPortal.Courses.Tests.CoursesTests
{
    public class AzureSearchTests
    {
        private const string URI_PATH = "http://localhost:7071/api/";

        private IConfiguration _config;
        private ICourseService _service;

        public AzureSearchTests()
        {
            //TestHelper.AddEnvironmentVariables();

            _config = new ConfigurationBuilder().SetBasePath(Environment.CurrentDirectory)
                                                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                                                .AddEnvironmentVariables()
                                                .Build();

            ServiceCollection services = new ServiceCollection();
            services.Configure<CosmosDbCollectionSettings>(_config.GetSection(nameof(CosmosDbCollectionSettings)))
                    .Configure<CosmosDbSettings>(_config.GetSection(nameof(CosmosDbSettings)))
                    .Configure<ProviderServiceSettings>(_config.GetSection(nameof(ProviderServiceSettings)))
                    .Configure<VenueServiceSettings>(_config.GetSection(nameof(VenueServiceSettings)))
                    .AddScoped<ICosmosDbHelper, CosmosDbHelper>()
                    .AddScoped<IProviderServiceWrapper, ProviderServiceWrapper>()
                    .AddScoped<ICourseService, CoursesService>()
                    .AddScoped<IVenueServiceWrapper, VenueServiceWrapper>();
            ServiceProvider provider = services.BuildServiceProvider();

            // Get CourseService instance
            _service = provider.GetService<ICourseService>();
        }


        [Fact]
        public void RunTests()
        {
            _PopulateSearch_Run();
            Assert.True(true);
        }


        [Fact]
        public async void _PopulateSearch_Run()
        {
            Mock<HttpRequest> mock = TestHelper.CreateMockRequest(new object());
            Task<IActionResult> task = FindACourseAzureSearchData.Run(mock.Object, new LogHelper((ILogger)null), _service);
            task.Wait();

            IEnumerable<IAzureSearchCourse> results = (IEnumerable<IAzureSearchCourse>)((Microsoft.AspNetCore.Mvc.ObjectResult)task.Result).Value;
            Assert.True(results.Any());
        }
    }
}
