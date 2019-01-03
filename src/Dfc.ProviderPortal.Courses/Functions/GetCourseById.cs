using System;
using System.IO;
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

namespace Dfc.ProviderPortal.Courses.Functions
{
    public static class GetCourseById
    {
        [FunctionName("GetCourseById")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log,
            [Inject] ICourseService coursesService)
        {
            string fromQuery = req.Query["id"];
            Course persisted = null;

            if (string.IsNullOrWhiteSpace(fromQuery))
            {
                return new BadRequestObjectResult($"Empty or missing id value.");
            }

            if (!Guid.TryParse(fromQuery, out Guid id))
            {
                return new BadRequestObjectResult($"Invalid id value. Expected a non-empty valid {nameof(Guid)}");
            }

            try
            {
                persisted = (Course) await coursesService.GetCourseById(id);

                if (persisted == null)
                {
                    return new NotFoundObjectResult(id);
                }

                return new OkObjectResult(persisted);
            }
            catch (Exception e)
            {
                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}
