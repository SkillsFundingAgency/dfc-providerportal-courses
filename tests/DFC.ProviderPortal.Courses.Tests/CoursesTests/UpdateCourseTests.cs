
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Courses.Functions;
using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Courses.Models;
using DFC.ProviderPortal.Courses.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;

namespace DFC.ProviderPortal.Courses.Tests.CoursesTests
{
    public class UpdateCoursesTests
    {
        Course _course = null;

        private const string URI_PATH = "http://localhost:7071/api/";

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

    public UpdateCoursesTests()
        {
            TestHelper.AddEnvironmentVariables();
        }


        //[Fact]
        //public void RunTests()
        //{
        //    _GetAllProviders_ReturnsResults();
        //    //_GetAllProviders_ExpectedCount();
        //    //_GetProviderById_Run();
        //    //_GetProviderByPRN_Run();
        //    //_GetProviderByPRNAndName_Run();
        //    Assert.True(true);
        //}




        [Fact]
        public async void _UpdateCourseById_Run()
        {
            System.Net.Http.HttpRequestMessage rm = TestHelper.CreateRequest(new Uri(URI_PATH + "UpdateCourseById"), UPDATE_COURSE);

            Mock<ICourseService> cs = new Mock<ICourseService>();

            cs.Setup(x => x.UpdateById(It.IsAny<Course>())).Returns(Task.FromResult(It.IsAny<ICourse>()));

            var result = await UpdateCourseById.Run(rm, new LogHelper((ILogger)null), cs.Object);
            //_course = TestHelper.GetAFReturnedObject<Course>(task);

            Assert.NotNull(result);
            Assert.True(true);
        }
    }
}
