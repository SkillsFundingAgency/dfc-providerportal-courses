
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
    public static class GetCourseCountsByStatusForUKPRN
    {
        [FunctionName("GetCourseCountsByStatusForUKPRN")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
                                                    ILogger log,
                                                    [Inject] ICourseService coursesService)
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

                // Get all statuses and count of course runs with matching status
                var grouped = from RecordStatus rs in Enum.GetValues(typeof(RecordStatus))
                              from CourseRun r in persisted.SelectMany(c => c.CourseRuns)
                                                           .Where(s => s.RecordStatus == rs)
                                                           .DefaultIfEmpty(new CourseRun() { id = Guid.Empty })
                              group r by rs into runsbystatus
                              select new {
                                  Status = runsbystatus.Key,
                                  Description = Enum.GetName(typeof(RecordStatus), runsbystatus.Key),
                                  Count = runsbystatus.LongCount(r => r.id != Guid.Empty)
                              };
                return new OkObjectResult(grouped);

            } catch (Exception e) {
                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}
