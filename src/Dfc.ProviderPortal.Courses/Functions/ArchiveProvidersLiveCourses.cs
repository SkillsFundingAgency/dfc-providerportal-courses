using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Courses.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Dfc.ProviderPortal.Courses.Functions
{
    public class ArchiveProvidersLiveCourses
    {
        private readonly ICourseService _coursesService;

        public ArchiveProvidersLiveCourses(ICourseService coursesService)
        {
            _coursesService = coursesService;
        }

        [FunctionName("ArchiveProvidersLiveCourses")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {
            string qryUKPRN = req.Query["UKPRN"];
            string qryUIMode = req.Query["Mode"];

            List<string> persisted = null;

            if (string.IsNullOrWhiteSpace(qryUKPRN))
                return new BadRequestObjectResult($"Empty or missing UKPRN value.");

            if (!int.TryParse(qryUKPRN, out int UKPRN))
                return new BadRequestObjectResult($"Invalid UKPRN value, expected a valid integer");

            if (string.IsNullOrWhiteSpace(qryUIMode))
                return new BadRequestObjectResult($"Empty or missing UIMode value.");

            if (!int.TryParse(qryUIMode, out int UIMode))
                return new BadRequestObjectResult($"Invalid UIMode value, expected a valid integer");


            try
            {
                var returnCode = await _coursesService.ArchiveProvidersLiveCourses(UKPRN, UIMode);

                return new OkObjectResult(persisted);

            }
            catch (Exception e)
            {
                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}
