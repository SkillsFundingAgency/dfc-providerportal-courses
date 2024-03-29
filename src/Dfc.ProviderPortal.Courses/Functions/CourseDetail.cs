
using System;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Courses.Interfaces;
//using Newtonsoft.Json;
using Dfc.ProviderPortal.Packages.AzureFunctions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;


namespace Dfc.ProviderPortal.Courses.Functions
{
    public static class CourseDetail
    {
        [FunctionName("CourseDetail")] //GetCourseSearchDataByCourseAndRunIds")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log,
            [Inject] ICourseService coursesService)
        {
            string qryCourseId = req.Query["CourseId"];
            string qryRunId = req.Query["RunId"];

            if (string.IsNullOrWhiteSpace(qryCourseId))
                return new BadRequestObjectResult($"Empty or missing CourseId value.");
            if (string.IsNullOrWhiteSpace(qryRunId))
                return new BadRequestObjectResult($"Empty or missing RunId value.");

            if (!Guid.TryParse(qryCourseId, out Guid courseId))
                return new BadRequestObjectResult($"Invalid CourseId value. Expected a non-empty valid {nameof(Guid)}");
            if (!Guid.TryParse(qryRunId, out Guid runId))
                return new BadRequestObjectResult($"Invalid RunId value. Expected a non-empty valid {nameof(Guid)}");

            var persisted = await coursesService.GetCourseSearchDataById(courseId, runId);

            if (persisted == null)
                return new NotFoundResult();

            return new OkObjectResult(persisted);
        }
    }
}
