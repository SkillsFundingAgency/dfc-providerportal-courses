
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Courses.Models;
using Dfc.ProviderPortal.Packages.AzureFunctions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;


namespace Dfc.ProviderPortal.Courses.Functions
{
    public static class UpdateCourse
    {
        [FunctionName("UpdateCourse")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequestMessage req,
            ILogger log,
            [Inject] ICourseService coursesService)
        {

            Course course = await req.Content.ReadAsAsync<Course>();

            try
            {
                var updatedCourse = (Course)await coursesService.Update(course);
                return new OkObjectResult(updatedCourse);

            }
            catch (Exception e)
            {
                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}
