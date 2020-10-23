using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Courses.Models;
using Dfc.ProviderPortal.Packages.AzureFunctions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dfc.ProviderPortal.Courses.Functions
{
    public class DeleteCoursesByUKPRN
    {
        [FunctionName("DeleteCoursesByUKPRN")]
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
                messagesList = await coursesService.DeleteCoursesByUKPRN(UKPRN);
                if (messagesList == null)
                    return new NotFoundObjectResult(UKPRN);

                return new OkObjectResult(messagesList);

            }
            catch (Exception e)
            {
                log.LogError("call to coursesService.ArchiveCourseRunsByUKPRN failed", e);
                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}
