
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Search.Models;
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
    public static class CourseSearch
    {
        [FunctionName("CourseSearch")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
                                                    ILogger log,
                                                    [Inject] ICourseService coursesService)
        {
            string query = req.Query["q"];
            FACSearchResult results = null;

            if (string.IsNullOrWhiteSpace(query))
                return new BadRequestObjectResult($"Empty or missing search value");

            try {
                results = await coursesService.CourseSearch(log,
                    new SearchCriteriaStructure() {
                        SubjectKeywordField = query
                    });
                //if (results == null)
                //    return new NotFoundObjectResult(query);

                return new OkObjectResult(results);

            } catch (Exception e) {
                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}
