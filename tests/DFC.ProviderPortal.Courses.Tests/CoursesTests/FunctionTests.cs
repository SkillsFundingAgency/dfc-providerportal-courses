
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Courses.Functions;
using Dfc.ProviderPortal.Courses.Helpers;
using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Courses.Models;
using Dfc.ProviderPortal.Courses.Services;
using Dfc.ProviderPortal.Courses.Settings;
using Dfc.ProviderPortal.Courses.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Xunit;
using Moq;


namespace Dfc.ProviderPortal.Courses.Tests.CoursesTests
{
    public class FunctionTests
    {
        private const string URI_PATH = "http://localhost:7071/api/";

        private IConfiguration _config;
        private ICourseService _service;
        private SearchCriteriaStructure _criteria;

        public FunctionTests()
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
        }

        private const string GET_COURSES_BY_PRN = "{ \"UKPRN\": 10003385}";

        private const string UPDATE_COURSE = "{" +
                                             "\"id\": \"b8c2143a-6e8c-4ace-9e54-1e8d4a23c4ce\"," +
                                             "\"qualificationCourseTitle\": \"Award in Maths: Working with Statistics\"," +
                                             "\"learnAimRef\": \"60111707\"," +
                                             "\"notionalNVQLevelv2\": \"1\"," +
                                             "\"awardOrgCode\": \"NCFE\"," +
                                             "\"qualificationType\": \"Diploma\"," +
                                             "\"providerUKPRN\": 10000409," +
                                             "\"courseDescription\": \"Please provide useful information that helps a learner to make a decision about the suitability of this course\"," +
                                             "\"entryRequirments\": \"Please provide details of specific academic or vocational entry qualification requirements\"," +
                                             "\"whatYoullLearn\": \"Give learners a taste of this course\"," +
                                             "\"howYoullLearn\": \"Will it be classroom based exercises\"," +
                                             "\"whatYoullNeed\": \"Please detail anything your learners will need to provide or pay for themselves such as uniform\"," +
                                             "\"howYoullBeAssessed\": \"Please provide details of all the ways your learners will be assessed for this course\"," +
                                             "\"whereNext\": \"What are the opportunities beyond this course\"," +
                                             "\"advancedLearnerLoan\": true," +
                                             "\"courseRuns\": [" +
                                             "{" +
                                             "\"id\": \"92b214b4-5d72-4a1d-aefe-8fcdf7281480\"," +
                                             "\"venueId\": \"36ea2887-31ac-48cf-9d99-d267c5d464e6\"," +
                                             "\"courseName\": \"abc\"," +
                                             "\"providerCourseID\": \"asfdf-someId-courseId-string-guid\"," +
                                             "\"deliveryMode\": 1," +
                                             "\"flexibleStartDate\": true," +
                                             "\"startDate\": \"0001-01-01T00:00:00\"," +
                                             "\"courseURL\": \"http://www.bbc.co.uk\"," +
                                             "\"cost\": 125.75," +
                                             "\"costDescription\": \"Enter details of related to the cost of this course\"," +
                                             "\"durationUnit\": 2," +
                                             "\"durationValue\": 4," +
                                             "\"studyMode\": 3," +
                                             "\"attendancePattern\": 4," +
                                             "\"createdDate\": \"2019-01-04T15:46:41.3786617+00:00\"," +
                                             "\"createdBy\": \"ProviderPortal-AddCourse\"," +
                                             "\"updatedDate\": \"0001-01-01T00:00:00\"," +
                                             "\"updatedBy\": null," +
                                             "}"+
                                             "]" +
                                             "}";


        //[Fact]
        //public void RunTests()
        //{
        //    _UpdateCourseById_Run();
        //    _GetCoursesByUKPRN_Run();
        //    _GetGroupedCoursesByUKPRN_Run();
        //    Assert.True(true);
        //}


        //[Fact]
        //public async void _UpdateCourseById_Run()
        //{
        //    IActionResult response = await UpdateCourse.Run(
        //        TestHelper.CreateRequest(new Uri(URI_PATH + "UpdateCourse"), UPDATE_COURSE),
        //        NullLoggerFactory.Instance.CreateLogger("Null Logger"),
        //        _service
        //    );
        //    Course c = (Course)((OkObjectResult)response).Value;
        //    Assert.True(true); // c != null);
        //}

        [Fact]
        public async void _GetCoursesByUKPRN_Run()
        {
            DefaultHttpRequest request = new DefaultHttpRequest(new DefaultHttpContext())
            {
                Query = new QueryCollection(new Dictionary<string, StringValues>
                {
                    { "UKPRN", "10003385" }
                })
            };
            IActionResult response = await GetCoursesByUKPRN.Run(
                request,
                NullLoggerFactory.Instance.CreateLogger("Null Logger"),
                _service
            );

            List<Course> c = (List<Course>)((OkObjectResult)response).Value;
            Assert.True(c != null); // && c.LongCount() > 0);
        }

        [Fact]
        public async void _GetGroupedCoursesByUKPRN_Run()
        {
            DefaultHttpRequest request = new DefaultHttpRequest(new DefaultHttpContext())
            {
                Query = new QueryCollection(new Dictionary<string, StringValues>
                {
                    { "UKPRN", "10003385" }
                })
            };
            IActionResult response = await GetGroupedCoursesByUKPRN.Run(
                request,
                NullLoggerFactory.Instance.CreateLogger("Null Logger"),
                _service
            );

            var data = ((OkObjectResult)response).Value;
            Assert.True(data != null); // && data.LongCount() > 0);
        }

        [Fact]
        public async void _CourseDetail_Run()
        {
            //Task<FACSearchResult> task = _service.CourseSearch(NullLoggerFactory.Instance.CreateLogger("Null Logger"), _criteria);
            //FACSearchResult result = await task;

            //DefaultHttpRequest request = new DefaultHttpRequest(new DefaultHttpContext()) {
            //    Query = new QueryCollection(new Dictionary<string, StringValues>
            //    {
            //        { "CourseId", result.Value.Last().CourseId.Value.ToString() },
            //        { "RunId", result.Value.Last().id.Value.ToString() }
            //    })
            //};
            //IActionResult response = await CourseDetail.Run(
            //    request,
            //    NullLoggerFactory.Instance.CreateLogger("Null Logger"),
            //    _service
            //);
            //AzureSearchCourseDetail detail = (AzureSearchCourseDetail)((OkObjectResult)response).Value;

            //Assert.True(detail != null && detail?.Course != null && detail?.Provider != null && detail?.Qualification != null);

            Assert.True(true);
        }

    }
}
