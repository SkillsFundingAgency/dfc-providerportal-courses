using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Dfc.ProviderPortal.Packages.AzureFunctions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Courses.Models;

namespace Dfc.ProviderPortal.Courses.Functions
{
    public static class AddCourse
    {
        [FunctionName("AddCourse")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log,
            [Inject] ICourseService coursesService)
        {
            using (var streamReader = new StreamReader(req.Body))
            {
                var requestBody = await streamReader.ReadToEndAsync();

                Course fromBody = null;
                Course persisted = null;

                try
                {
                    fromBody = JsonConvert.DeserializeObject<Course>(requestBody);
                }
                catch (Exception e)
                {
                    return new BadRequestObjectResult(e);
                }

                try
                {
                    persisted = (Course) await coursesService.AddCourse(fromBody);
                }
                catch (Exception e)
                {
                    return new InternalServerErrorObjectResult(e);
                }

                return new CreatedResult(persisted.id.ToString(), persisted);
            }
        }

    }
}
