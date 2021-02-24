
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
    public class GetGroupedCoursesByUKPRN
    {
        private readonly ICourseService _coursesService;

        public GetGroupedCoursesByUKPRN(ICourseService coursesService)
        {
            _coursesService = coursesService;
        }

        [FunctionName("GetGroupedCoursesByUKPRN")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {
            string fromQuery = req.Query["UKPRN"];
            List<Course> persisted = null;

            if (string.IsNullOrWhiteSpace(fromQuery))
                return new BadRequestObjectResult($"Empty or missing UKPRN value.");

            if (!int.TryParse(fromQuery, out int UKPRN))
                return new BadRequestObjectResult($"Invalid UKPRN value, expected a non-empty valid integer");

            try {
                persisted = (List<Course>) await _coursesService.GetCoursesByUKPRN(UKPRN);
                if (persisted == null)
                    return new NotFoundObjectResult(UKPRN);

                var grouped = from Course c1 in persisted
                              group c1 by c1.QualificationType into grouped1
                              from grouped2 in (
                                from Course c2 in grouped1
                                group c2 by c2.LearnAimRef
                              )
                              group grouped2 by grouped1.Key;
                return new OkObjectResult(grouped); // persisted);

            } catch (Exception e) {

                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}
