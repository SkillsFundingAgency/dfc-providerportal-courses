
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Courses.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;


namespace Dfc.ProviderPortal.Courses.Functions
{
    public class GetCourseCountsByStatusForUKPRN
    {
        private readonly ICourseService _coursesService;

        public GetCourseCountsByStatusForUKPRN(ICourseService coursesService)
        {
            _coursesService = coursesService;
        }

        [FunctionName("GetCourseCountsByStatusForUKPRN")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {
            string fromQuery = req.Query["UKPRN"];
            List<Course> persisted = null;

            if (string.IsNullOrWhiteSpace(fromQuery))
                return new BadRequestObjectResult($"Empty or missing UKPRN value.");

            if (!int.TryParse(fromQuery, out int UKPRN))
                return new BadRequestObjectResult($"Invalid UKPRN value, expected a valid integer");

            try {
                persisted = (List<Course>) await _coursesService.GetCoursesByUKPRN(UKPRN);
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
