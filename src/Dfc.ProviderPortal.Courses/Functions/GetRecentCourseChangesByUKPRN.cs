
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
using Microsoft.Extensions.Configuration;


namespace Dfc.ProviderPortal.Courses.Functions
{
    public class GetRecentCourseChangesByUKPRN
    {
        private readonly ICourseService _coursesService;
        private readonly IConfiguration _configuration;

        public GetRecentCourseChangesByUKPRN(ICourseService coursesService, IConfiguration configuration)
        {
            _coursesService = coursesService;
            _configuration = configuration;
        }

        [FunctionName("GetRecentCourseChangesByUKPRN")]
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

                if (!int.TryParse(_configuration["CosmosDbSettings:RecentCount"], out int count))
                    count = 10;
                return new OkObjectResult(persisted //.SelectMany(c => c.CourseRuns)
                                                   .OrderByDescending(c => c.UpdatedDate ?? c.CreatedDate)
                                                   .Take(count));

            } catch (Exception e) {
                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}
