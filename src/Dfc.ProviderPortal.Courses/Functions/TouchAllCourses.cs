
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Packages.AzureFunctions.DependencyInjection;
using Dfc.ProviderPortal.Courses.Models;
using Dfc.ProviderPortal.Courses.Helpers;


namespace Dfc.ProviderPortal.Courses.Functions
{
    public static class TouchAllCourses
    {
        [FunctionName("TouchAllCourses")]
        //public static void Run([TimerTrigger("0 0 0 */1 * *")]TimerInfo myTimer,    // Every 24 hrs normally
        public static void Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer,    // Every minute for debug
            //[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequestMessage req,
            ILogger log,
            [Inject] ICourseService coursesService)
        {
            log.LogInformation($"TouchAllCourses timer trigger function executed at: {DateTime.Now}");

            try {
                IEnumerable<Course> courses = (IEnumerable<Course>)/*await*/ coursesService.TouchAllCourses(log);
                log.LogInformation($"TouchAllCourses affected {courses.LongCount()} courses");
                //return new OkObjectResult(courses);

            } catch (Exception e) {
                //return new InternalServerErrorObjectResult(e);
                log.LogError(e, "*** TouchAllCourses failed");
                throw e;
            }
        }
    }
}
