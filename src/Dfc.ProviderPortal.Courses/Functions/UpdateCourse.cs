
using System;
using System.IO;
using System.Net.Http;
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
    public static class UpdateCourse
    {
        [FunctionName("UpdateCourse")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequestMessage req,
            ILogger log,
            [Inject] ICourseService coursesService)
        {

            Course course = await req.Content.ReadAsAsync<Course>();

            try
            {
                var updatedCourse = (Course)await coursesService.Update(course);
                return new OkObjectResult(updatedCourse);

            } catch (Exception e) {
                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}
