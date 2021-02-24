using System;
using System.Net.Http;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Courses.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Dfc.ProviderPortal.Courses.Functions
{
    public class DeleteBulkUploadCourses
    {
        private readonly ICourseService _coursesService;
        private readonly ILogger _logger;

        public DeleteBulkUploadCourses(ICourseService coursesService, ILogger<DeleteBulkUploadCourses> logger)
        {
            _coursesService = coursesService;
            _logger = logger;
        }

        [FunctionName("DeleteBulkUploadCourses")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequestMessage req)
        {
            string strUKPRN = req.RequestUri.ParseQueryString()["UKPRN"]?.ToString()
                                ?? (await (dynamic)req.Content.ReadAsAsync<object>())?.UKPRN;

            if (string.IsNullOrWhiteSpace(strUKPRN))
                return new BadRequestObjectResult($"Empty or missing UKPRN value.");

            if (!int.TryParse(strUKPRN, out int UKPRN))
                return new BadRequestObjectResult($"Invalid UKPRN value, expected a valid integer");

            try
            {
                var result = await _coursesService.ArchivePendingBulkUploadCourseRunsByUKPRN(UKPRN);
                result.EnsureSuccessStatusCode();
                return new OkResult();
            }
            catch (Exception e)
            {
                _logger.LogError("call to coursesService.ArchiveCourseRunsByUKPRN failed", e);
                return new InternalServerErrorObjectResult(e);
            }
            finally
            {
                _logger.LogInformation($"DeleteCoursesByUKPRN finished");
            }
        }
    }
}
