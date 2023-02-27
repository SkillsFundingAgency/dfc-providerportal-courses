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
    public static class GetAllDfcReports
    {
        [FunctionName("GetAllDfcReports")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log,
            [Inject] IDfcReportService dfcReportService)
        {
            try
            {
                var result = await dfcReportService.GetDfcReports();
                return new OkObjectResult(result);

            }
            catch (Exception e)
            {
                return new InternalServerErrorObjectResult(e);
            }

        }
    }
}
