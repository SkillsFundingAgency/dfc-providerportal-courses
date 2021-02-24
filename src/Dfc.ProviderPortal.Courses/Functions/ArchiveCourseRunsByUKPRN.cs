using System;
using System.Net.Http;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Courses.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Dfc.ProviderPortal.Courses.Functions
{
    public class ArchiveCourseRunsByUKPRN
    {
        private readonly ICourseService _coursesService;

        public ArchiveCourseRunsByUKPRN(ICourseService coursesService)
        {
            _coursesService = coursesService;
        }

        [FunctionName("ArchiveCourseRunsByUKPRN")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequestMessage req)
        {
            var qryUKPRN = req.RequestUri.ParseQueryString()["UKPRN"]?.ToString();

            if (string.IsNullOrWhiteSpace(qryUKPRN))
                return new BadRequestObjectResult($"Empty or missing UKPRN value.");

            if (!int.TryParse(qryUKPRN, out int UKPRN))
                return new BadRequestObjectResult($"Invalid UKPRN value, expected a valid integer");

            try
            {
                var returnCode = await _coursesService.ArchiveCourseRunsByUKPRN(UKPRN);

                return new OkObjectResult(returnCode);
            }
            catch (Exception e)
            {
                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}
