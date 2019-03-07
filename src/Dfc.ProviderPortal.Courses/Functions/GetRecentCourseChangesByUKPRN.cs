
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Packages.AzureFunctions.DependencyInjection;
using Dfc.ProviderPortal.Courses.Models;


namespace Dfc.ProviderPortal.Courses.Functions
{
    public static class GetRecentCourseChangesByUKPRN
    {
        [FunctionName("GetRecentCourseChangesByUKPRN")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
                                                    ILogger log,
                                                    [Inject] ICourseService coursesService,
                                                    [Inject] ICosmosDbSettings settings)
        {
            string fromQuery = req.Query["UKPRN"];
            List<Course> persisted = null;

            if (string.IsNullOrWhiteSpace(fromQuery))
                return new BadRequestObjectResult($"Empty or missing UKPRN value.");

            if (!int.TryParse(fromQuery, out int UKPRN))
                return new BadRequestObjectResult($"Invalid UKPRN value, expected a valid integer");

            try {
                persisted = (List<Course>) await coursesService.GetCoursesByUKPRN(UKPRN);
                if (persisted == null)
                    return new NotFoundObjectResult(UKPRN);

                return new OkObjectResult(persisted //.SelectMany(c => c.CourseRuns)
                                                   .OrderByDescending(c => c.UpdatedDate ?? c.CreatedDate)
                                                   .Take(settings?.RecentCount > 0 ? settings.RecentCount : 10));

            } catch (Exception e) {
                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}
