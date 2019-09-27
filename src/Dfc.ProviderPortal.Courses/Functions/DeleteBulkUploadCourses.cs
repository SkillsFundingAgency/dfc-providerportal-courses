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
using System.Net.Http;
using System.Collections.Generic;
using Dfc.ProviderPortal.Courses.Models;

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

            List<string> messagesList = null;

            if (string.IsNullOrWhiteSpace(strUKPRN))
                return new BadRequestObjectResult($"Empty or missing UKPRN value.");

            if (!int.TryParse(strUKPRN, out int UKPRN))
                return new BadRequestObjectResult($"Invalid UKPRN value, expected a valid integer");

            try
            {
                messagesList = await coursesService.DeleteBulkUploadCourses(UKPRN);

                var r0 = await coursesService.ChangeAllCourseRunStatusesForUKPRNSelection(UKPRN, RecordStatus.Archived);

                if (messagesList == null)
                    return new NotFoundObjectResult(UKPRN);

                return new OkObjectResult(messagesList);

            }
            catch (Exception e)
            {
                return new InternalServerErrorObjectResult(e);
            }
            finally
            {
                log.LogInformation($"DeleteCoursesByUKPRN finished");
            }
        }
    }
}
