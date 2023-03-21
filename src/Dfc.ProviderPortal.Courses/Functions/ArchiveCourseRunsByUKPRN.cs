using System;
using System.Net.Http;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Packages.AzureFunctions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Dfc.ProviderPortal.Courses.Functions
{
    public static class ArchiveCourseRunsByUKPRN
    {
        [FunctionName("ArchiveCourseRunsByUKPRN")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequestMessage req,
                                            ILogger log,
                                            [Inject] ICourseService coursesService)
        {
            var qryUKPRN = req.RequestUri.ParseQueryString()["UKPRN"]?.ToString()
                               ?? (await (dynamic)req.Content.ReadAsAsync<object>())?.UKPRN;

            if (string.IsNullOrWhiteSpace(qryUKPRN))
                return new BadRequestObjectResult($"Empty or missing UKPRN value.");

            if (!int.TryParse(qryUKPRN, out int UKPRN))
                return new BadRequestObjectResult($"Invalid UKPRN value, expected a valid integer");

            try
            {
                var returnCode = await coursesService.ArchiveCourseRunsByUKPRN(UKPRN);

                return new OkObjectResult(returnCode);
            }
            catch (Exception e)
            {
                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}
