
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Courses.Models;
using Dfc.ProviderPortal.Packages.AzureFunctions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace Dfc.ProviderPortal.Courses.Functions
{
    public static class GetRecentCourseChangesByUKPRN
    {
        [FunctionName("GetRecentCourseChangesByUKPRN")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
                                                    ILogger log,
                                                    ExecutionContext context,
                                                    [Inject] ICourseService coursesService)
        {
            string fromQuery = req.Query["UKPRN"];
            List<Course> persisted = null;

            if (string.IsNullOrWhiteSpace(fromQuery))
                return new BadRequestObjectResult($"Empty or missing UKPRN value.");

            if (!int.TryParse(fromQuery, out int UKPRN))
                return new BadRequestObjectResult($"Invalid UKPRN value, expected a valid integer");

            try
            {
                persisted = (List<Course>)await coursesService.GetCoursesByUKPRN(UKPRN);
                if (persisted == null)
                    return new NotFoundObjectResult(UKPRN);

                var config = new ConfigurationBuilder().SetBasePath(context.FunctionAppDirectory)
                                                       .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                                                       .AddEnvironmentVariables()
                                                       .Build();

                if (!int.TryParse(config["CosmosDbSettings:RecentCount"], out int count))
                    count = 10;
                return new OkObjectResult(persisted //.SelectMany(c => c.CourseRuns)
                                                   .OrderByDescending(c => c.UpdatedDate ?? c.CreatedDate)
                                                   .Take(count));

            }
            catch (Exception e)
            {
                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}
