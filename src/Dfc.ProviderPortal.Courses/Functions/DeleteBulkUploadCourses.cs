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
    public static class DeleteBulkUploadCourses
    {
        [FunctionName("DeleteBulkUploadCourses")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequestMessage req,
                                            ILogger log,
                                            [Inject] ICourseService coursesService)
        {
            log.LogInformation($"DeleteCoursesByUKPRN starting");

            string strUKPRN = req.RequestUri.ParseQueryString()["UKPRN"]?.ToString()
                                ?? (await (dynamic)req.Content.ReadAsAsync<object>())?.UKPRN;

            if (string.IsNullOrWhiteSpace(strUKPRN))
                return new BadRequestObjectResult($"Empty or missing UKPRN value.");

            if (!int.TryParse(strUKPRN, out int UKPRN))
                return new BadRequestObjectResult($"Invalid UKPRN value, expected a valid integer");

            try
            {
                var result = await coursesService.ArchivePendingBulkUploadCourseRunsByUKPRN(UKPRN);
                result.EnsureSuccessStatusCode();
                return new OkResult();
            }
            catch (Exception e)
            {
                log.LogError("call to coursesService.ArchiveCourseRunsByUKPRN failed", e);
                return new InternalServerErrorObjectResult(e);
            }
            finally
            {
                log.LogInformation($"DeleteCoursesByUKPRN finished");
            }
        }
    }
}
