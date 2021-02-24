using System;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Courses.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Dfc.ProviderPortal.Courses.Functions
{
    public class GetTotalLiveCourses
    {
        private readonly ICourseService _courseService;

        public GetTotalLiveCourses(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [FunctionName("GetTotalLiveCourses")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {
            try
            {
                var allCourses = await _courseService.GetTotalLiveCourses();
                return new OkObjectResult(allCourses);

            }
            catch (Exception e)
            {
                return new InternalServerErrorObjectResult(e);
            }

        }
    }
}
