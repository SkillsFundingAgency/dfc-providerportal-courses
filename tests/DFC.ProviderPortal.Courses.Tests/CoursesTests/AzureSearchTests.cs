
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Primitives;
using Dfc.ProviderPortal.Courses.Functions;
using Dfc.ProviderPortal.Courses.Helpers;
using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Courses.Models;
using Dfc.ProviderPortal.Courses.Services;
using Dfc.ProviderPortal.Courses.Settings;
using DFC.ProviderPortal.Courses.Tests.Helpers;
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
        private SearchCriteriaStructure _criteria;

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
                    .Configure<QualificationServiceSettings>(_config.GetSection(nameof(QualificationServiceSettings)))
                    .Configure<SearchServiceSettings>(_config.GetSection(nameof(SearchServiceSettings)))
                    .AddScoped<ICosmosDbHelper, CosmosDbHelper>()
                    .AddScoped<IProviderServiceWrapper, ProviderServiceWrapper>()
                    .AddScoped<ICourseService, CoursesService>()
                    .AddScoped<IVenueServiceWrapper, VenueServiceWrapper>()
                    .AddScoped<ISearchServiceWrapper, SearchServiceWrapper>();
            ServiceProvider provider = services.BuildServiceProvider();

            // Get CourseService instance
            _service = provider.GetService<ICourseService>();

            _criteria = new SearchCriteriaStructure() {
                SubjectKeyword = "biol",
                TownOrPostcode = "B1 2JP",
                QualificationLevels = new string[] { "1", "2", "3", "4", "5", "7", "E" },
                AttendanceModes = new string[] { },
                AttendancePatterns = new string[] { "1", "2", "3" },
                DFE1619Funded = "",
                StudyModes = new string[] { },
                Distance = 200,
                TopResults = 100
            };
        }


        [Fact]
        public void RunTests()
        {
            _SearchCourse_Run();
            _CourseDetail_Run();
            Assert.True(true);
        }

        [Fact]
        public async void _SearchCourse_Run()
        {
            Task<FACSearchResult> task = _service.CourseSearch(NullLoggerFactory.Instance.CreateLogger("Null Logger"), _criteria);
            FACSearchResult result = await task;
            Assert.True(result != null && result.Value?.Count() > 0);
        }


        [Fact]
        public async void _CourseDetail_Run()
        {
            Task<FACSearchResult> task = _service.CourseSearch(NullLoggerFactory.Instance.CreateLogger("Null Logger"), _criteria);
            FACSearchResult result = await task;

            DefaultHttpRequest request = new DefaultHttpRequest(new DefaultHttpContext()) {
                Query = new QueryCollection(new Dictionary<string, StringValues>
                {
                    { "CourseId", result.Value.First().CourseId.Value.ToString() },
                    { "RunId", result.Value.First().id.Value.ToString() }
                })
            };
            IActionResult response = await CourseDetail.Run(
                request,
                NullLoggerFactory.Instance.CreateLogger("Null Logger"),
                _service
            );
            AzureSearchCourseDetail detail = (AzureSearchCourseDetail)((OkObjectResult)response).Value;

            Assert.True(detail != null && detail?.Course != null && detail?.Provider != null && detail?.Qualification != null);
        }

    }
}
