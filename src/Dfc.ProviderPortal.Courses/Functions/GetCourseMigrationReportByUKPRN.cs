using System;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Packages.AzureFunctions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Dfc.ProviderPortal.Courses.Functions
{

    public static class GetCourseMigrationReportByUKPRN
    {
        [FunctionName("GetCourseMigrationReportByUKPRN")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log,
            [Inject] ICourseMigrationReportService courseMigrationReportService)
        {
            string fromQuery = req.Query["UKPRN"];
            if (!int.TryParse(fromQuery, out int UKPRN))
                return new BadRequestObjectResult($"Invalid UKPRN value, expected a non-empty valid integer");

            try
            {
                var result = await courseMigrationReportService.GetMigrationReport(UKPRN);
                return new OkObjectResult(result);

            }
            catch (Exception e)
            {
                return new InternalServerErrorObjectResult(e);
            }

        }
    }
}
