using Microsoft.Extensions.Logging;
using Dfc.ProviderPortal.Courses.Models.Models.Courses;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using Moq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Web.Http.Hosting;
using Dfc.ProviderPortal.Courses.Functions;
using Dfc.ProviderPortal.Courses.Tests.Helpers;
using System.Web.Http;

namespace Dfc.ProviderPortal.Courses.Tests
{
    public class AddCourseTests
    {
        private Course _course = null;
        public AddCourseTests()
        {
            TestHelper.AddEnvironmentVariables();
        }
        [Fact]
        public void AddVenue()
        {

            _course = new Course
            {
                ID = Guid.NewGuid(),
                QuAP = new Models.Models.Qualifications.QuAP()
            };
            
            var json = JsonConvert.SerializeObject(_course);
            Uri uri = new Uri("https://dfc-dev-prov-cdb.documents.azure.com/ ");
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = uri,
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            var mockLogger = new Mock<ILogger<Task>>();

            Task<HttpResponseMessage> task = AddCourse.Run(request, mockLogger.Object);
            _course = TestHelper.GetAFReturnedObject<Course>(task);

            Assert.True(_course != null);
        }
    }
}
