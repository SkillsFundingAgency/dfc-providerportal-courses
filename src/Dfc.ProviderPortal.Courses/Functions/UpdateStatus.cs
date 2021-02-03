using System;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Courses.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Dfc.ProviderPortal.Courses.Functions
{
    public class UpdateStatus
    {
        private readonly ICourseService _coursesService;

        public UpdateStatus(ICourseService coursesService)
        {
            _coursesService = coursesService;
        }

        [FunctionName("UpdateStatus")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {
            string qryCourseId = req.Query["CourseId"];
            string qryCourseRunId = req.Query["CourseRunId"];
            string qryStatus = req.Query["Status"];

            //Validation
            if (string.IsNullOrWhiteSpace(qryCourseId))
                return new BadRequestObjectResult($"Empty or missing CourseId value.");
            if (string.IsNullOrWhiteSpace(qryCourseRunId))
                return new BadRequestObjectResult($"Empty or missing CourseRunId value.");
            if (string.IsNullOrWhiteSpace(qryStatus))
                return new BadRequestObjectResult($"Empty or missing Status value.");

            if (!Guid.TryParse(qryCourseId, out Guid courseId))
                return new BadRequestObjectResult($"Invalid CourseId value. Expected a non-empty valid {nameof(Guid)}");
            if (!Guid.TryParse(qryCourseRunId, out Guid courseRunId))
                return new BadRequestObjectResult($"Invalid CourseRunId value. Expected a non-empty valid {nameof(Guid)}");
            if (!int.TryParse(qryStatus, out int status))
                return new BadRequestObjectResult($"Invalid status value. Expected a non-empty valid {nameof(Int32)}");


            if (!Enum.IsDefined(typeof(RecordStatus), status))
            {
                return new BadRequestObjectResult($"Invalid status value. Expected a valid status");
            }

            try
            {
                var updatedCourse = await _coursesService.UpdateStatus(courseId, courseRunId, status);
                return new OkObjectResult(StatusCodes.Status204NoContent);

            }
            catch (Exception e)
            {
                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}
