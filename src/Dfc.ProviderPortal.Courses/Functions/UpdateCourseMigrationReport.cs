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
    
    public static class UpdateCourseMigrationReport
    {
        [FunctionName("UpdateCourseMigrationReport")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequestMessage req,
            ILogger log,
            [Inject] ICourseMigrationReportService courseMigrationReportService)
        {
            var courseMigrationReport = await req.Content.ReadAsAsync<CourseMigrationReport>();
            
            try
            {
                await courseMigrationReportService.AddMigrationReport(courseMigrationReport);
                return new OkResult();

            }
            catch (Exception e)
            {
                return new InternalServerErrorObjectResult(e);
            }

        }
    }
}
