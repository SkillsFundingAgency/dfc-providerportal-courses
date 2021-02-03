using System;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Courses.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Dfc.ProviderPortal.Courses.Functions
{

    public class GetCourseMigrationReportByUKPRN
    {
        private readonly ICourseMigrationReportService _courseMigrationReportService;

        public GetCourseMigrationReportByUKPRN(ICourseMigrationReportService courseMigrationReportService)
        {
            _courseMigrationReportService = courseMigrationReportService;
        }

        [FunctionName("GetCourseMigrationReportByUKPRN")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {
            string fromQuery = req.Query["UKPRN"];
            if (!int.TryParse(fromQuery, out int UKPRN))
                return new BadRequestObjectResult($"Invalid UKPRN value, expected a non-empty valid integer");

            try
            {
                var result = await _courseMigrationReportService.GetMigrationReport(UKPRN);
                return new OkObjectResult(result);

            }
            catch (Exception e)
            {
                return new InternalServerErrorObjectResult(e);
            }

        }
    }
}
