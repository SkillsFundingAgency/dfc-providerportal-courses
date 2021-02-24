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
    public class DeleteCoursesByUKPRN
    {
        private readonly ICourseService _coursesService;
        private readonly ILogger _logger;

        public DeleteCoursesByUKPRN(ICourseService coursesService, ILogger<DeleteCoursesByUKPRN> logger)
        {
            _coursesService = coursesService;
            _logger = logger;
        }
        
        [FunctionName("DeleteCoursesByUKPRN")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequestMessage req)
        {
            _logger.LogInformation($"DeleteCoursesByUKPRN starting");

            string strUKPRN = req.RequestUri.ParseQueryString()["UKPRN"]?.ToString()
                                ?? (await (dynamic)req.Content.ReadAsAsync<object>())?.UKPRN;

            if (string.IsNullOrWhiteSpace(strUKPRN))
                return new BadRequestObjectResult($"Empty or missing UKPRN value.");

            if (!int.TryParse(strUKPRN, out int UKPRN))
                return new BadRequestObjectResult($"Invalid UKPRN value, expected a valid integer");

            try
            {
                // Soft-delete (archive) instead of deleting so that changes show up in the CosmosDB Change Feed Listener.
                // https://stackoverflow.com/questions/48491932/detecting-update-and-deletion-in-cosmos-db-using-cosmosdbtrigger-in-an-azure-fun/48492092#48492092
                // This will keep the rest of the system from becoming inconsistent and causing errors.
                var result = await _coursesService.ArchiveCourseRunsByUKPRN(UKPRN);
                result.EnsureSuccessStatusCode();
                return new OkResult();
            }
            catch (Exception e)
            {
                _logger.LogError("call to coursesService.ArchiveCourseRunsByUKPRN failed", e);
                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}
