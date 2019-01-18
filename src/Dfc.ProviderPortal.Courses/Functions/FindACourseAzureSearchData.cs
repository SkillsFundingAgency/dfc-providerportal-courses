
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Packages.AzureFunctions.DependencyInjection;
using Dfc.ProviderPortal.Courses.Models;


namespace Dfc.ProviderPortal.Courses.Functions
{
    public static class FindACourseAzureSearchData
    {
        [FunctionName("FindACourseAzureSearchData")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
                                                    ILogger log,
                                                    [Inject] ICourseService coursesService)
        {
            //List<Course> courses = null;
            IEnumerable<IAzureSearchCourse> results = null;

            try {
                //courses = (List<Course>) await coursesService.GetAllCourses(log);
                //if (courses == null)
                //    return new NotFoundObjectResult(null);

                results = await coursesService.FindACourseAzureSearchData(log);

                return new OkObjectResult(results); // courses);

            } catch (Exception e) {
                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}
