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
using Dfc.ProviderPortal.Courses.Models;

namespace Dfc.ProviderPortal.Courses.Functions
{
    public static class ArchiveProvidersLiveCourses
    {
        [FunctionName("ArchiveProvidersLiveCourses")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
                                                    ILogger log,
                                                    [Inject] ICourseService coursesService)
        {
            string qryUKPRN = req.Query["UKPRN"];
            string qryUIMode = req.Query["Mode"];

            List<string> persisted = null;

            if (string.IsNullOrWhiteSpace(qryUKPRN))
                return new BadRequestObjectResult($"Empty or missing UKPRN value.");

            if (!int.TryParse(qryUKPRN, out int UKPRN))
                return new BadRequestObjectResult($"Invalid UKPRN value, expected a valid integer");

            if (string.IsNullOrWhiteSpace(qryUIMode))
                return new BadRequestObjectResult($"Empty or missing UIMode value.");

            if (!int.TryParse(qryUIMode, out int UIMode))
                return new BadRequestObjectResult($"Invalid UIMode value, expected a valid integer");


            try
            {
                var returnCode = await coursesService.ArchiveProvidersLiveCourses(UKPRN, UIMode);

                return new OkObjectResult(persisted);

            }
            catch (Exception e)
            {
                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}
