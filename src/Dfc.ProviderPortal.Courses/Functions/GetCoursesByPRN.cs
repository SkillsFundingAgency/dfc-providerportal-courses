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
using Dfc.ProviderPortal.Courses.Interfaces;
using System.Collections.Generic;

namespace Dfc.ProviderPortal.Courses.Functions
{
    public static class GetCoursesByPRN
    {
        [FunctionName("GetCoursesByPRN")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log,
            [Inject] ICourseService coursesService)
        {
            string fromQuery = req.Query["prn"];
            IEnumerable<ICourse> persisted = null;

            if (string.IsNullOrWhiteSpace(fromQuery))
            {
                return new BadRequestObjectResult($"Empty or missing prn value.");
            }

            if (!int.TryParse(fromQuery, out int prn))
            {
                return new BadRequestObjectResult($"Invalid prn value. Expected an {nameof(Int32)}");
            }

            try
            {
                persisted = await coursesService.GetCourseByPRN(prn);

                if (persisted == null)
                {
                    return new NotFoundObjectResult(prn);
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
