using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Dfc.ProviderPortal.Packages.AzureFunctions.DependencyInjection;
using Dfc.ProviderPortal.Courses.Interfaces;
using System.Collections.Generic;
using Dfc.ProviderPortal.Courses.Models;
using System.Linq;

namespace Dfc.ProviderPortal.Courses.Functions
{
    public static class GetCoursesByLevelForUKPRN
    {
        [FunctionName("GetCoursesByLevelForUKPRN")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
                                                    ILogger log,
                                                    [Inject] ICourseService coursesService)
        {
            string fromQuery = req.Query["UKPRN"];
            List<Course> persisted = null;

            if (string.IsNullOrWhiteSpace(fromQuery))
                return new BadRequestObjectResult($"Empty or missing UKPRN value.");

            if (!int.TryParse(fromQuery, out int UKPRN))
                return new BadRequestObjectResult($"Invalid UKPRN value, expected a non-empty valid integer");

            try
            {
                persisted = (List<Course>)await coursesService.GetCoursesByUKPRN(UKPRN);
                if (persisted == null)
                    return new NotFoundObjectResult(UKPRN);

                var grouped = from Course c1 in persisted
                              group c1 by c1.NotionalNVQLevelv2 into grouped1
                              from grouped2 in (
                                from Course c2 in grouped1
                                group c2 by c2.LearnAimRef
                              )
                              group grouped2 by grouped1.Key;
                return new OkObjectResult(grouped); // persisted);

            }
            catch (Exception e)
            {
                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}
