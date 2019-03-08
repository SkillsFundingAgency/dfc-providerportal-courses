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
using System.Net.Http;
using Dfc.ProviderPortal.Courses.Models;

namespace Dfc.ProviderPortal.Courses.Functions
{
    public static class UpdateStatus
    {
        [FunctionName("UpdateStatus")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log,
            [Inject] ICourseService coursesService)
        {
            string qryCourseId = req.Query["CourseId"];
            string qryCourseRunId = req.Query["CourseRunId"];
            string qryCurrentStatus = req.Query["CurrentStatus"];
            string qryStatusUpdate = req.Query["StatusUpdate"];

            //Validation
            if (string.IsNullOrWhiteSpace(qryCourseId))
                return new BadRequestObjectResult($"Empty or missing CourseId value.");
            if (string.IsNullOrWhiteSpace(qryCourseRunId))
                return new BadRequestObjectResult($"Empty or missing CourseRunId value.");
            if (string.IsNullOrWhiteSpace(qryCurrentStatus))
                return new BadRequestObjectResult($"Empty or missing Status value.");
            if (string.IsNullOrWhiteSpace(qryStatusUpdate))
                return new BadRequestObjectResult($"Empty or missing Status value.");

            if (!Guid.TryParse(qryCourseId, out Guid courseId))
                return new BadRequestObjectResult($"Invalid CourseId value. Expected a non-empty valid {nameof(Guid)}");
            if (!Guid.TryParse(qryCourseRunId, out Guid courseRunId))
                return new BadRequestObjectResult($"Invalid CourseRunId value. Expected a non-empty valid {nameof(Guid)}");
            if (!int.TryParse(qryCurrentStatus, out int currentStatus))
                return new BadRequestObjectResult($"Invalid status value. Expected a non-empty valid {nameof(Int32)}");
            if (!int.TryParse(qryStatusUpdate, out int statusUpdate))
                return new BadRequestObjectResult($"Invalid status value. Expected a non-empty valid {nameof(Int32)}");
            
            
            if((!Enum.IsDefined(typeof(RecordStatus), currentStatus)) || (!Enum.IsDefined(typeof(RecordStatus), statusUpdate)))
            {
                return new BadRequestObjectResult($"Invalid status value. Expected a valid status");
            }

            try
            {
                var updatedCourse = await coursesService.UpdateStatus(courseId, courseRunId, currentStatus, statusUpdate);
                return new OkObjectResult(StatusCodes.Status204NoContent);

            }
            catch (Exception e)
            {
                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}
