using System;
using System.Net.Http;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Courses.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Dfc.ProviderPortal.Courses.Functions
{
    public class ArchiveCoursesExceptBulkUploadReadytoGoLive
    {
        private readonly ICourseService _coursesService;

        public ArchiveCoursesExceptBulkUploadReadytoGoLive(ICourseService coursesService)
        {
            _coursesService = coursesService;
        }

        [FunctionName("ArchiveCoursesExceptBulkUploadReadytoGoLive")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequestMessage req)
        {
            var qryUKPRN = req.RequestUri.ParseQueryString()["UKPRN"]?.ToString()
                               ?? (await (dynamic)req.Content.ReadAsAsync<object>())?.UKPRN;
            var qryStatusToBeChangedTo = req.RequestUri.ParseQueryString()["StatusToBeChangedTo"]?.ToString()
                               ?? (await (dynamic)req.Content.ReadAsAsync<object>())?.StatusToBeChangedTo;

            if (string.IsNullOrWhiteSpace(qryUKPRN))
                return new BadRequestObjectResult($"Empty or missing UKPRN value.");

            if (!int.TryParse(qryUKPRN, out int UKPRN))
                return new BadRequestObjectResult($"Invalid UKPRN value, expected a valid integer");


            if (string.IsNullOrWhiteSpace(qryStatusToBeChangedTo))
                return new BadRequestObjectResult($"Empty or missing StatusToBeChangedTo value.");

            if (!int.TryParse(qryStatusToBeChangedTo, out int intStatusToBeChangedTo))
                return new BadRequestObjectResult($"Invalid StatusToBeChangedTo value, expected a valid integer");

            RecordStatus StatusToBeChangedTo = RecordStatus.Undefined;
            if (Enum.IsDefined(typeof(RecordStatus), intStatusToBeChangedTo))
            {
                StatusToBeChangedTo = (RecordStatus)Enum.ToObject(typeof(RecordStatus), intStatusToBeChangedTo);
            }
            else
            {
                return new BadRequestObjectResult($"StatusToBeChangedTo value cannot be parse into valid RecordStatus");
            }

            if (StatusToBeChangedTo.Equals(RecordStatus.Undefined))
            {
                return new BadRequestObjectResult($"StatusToBeChangedTo value is not allowed to be with  Undefined RecordStatus");
            }


            try
            {
                var returnCode = await _coursesService.ArchiveCoursesExceptBulkUploadReadytoGoLive(UKPRN, StatusToBeChangedTo);

                return new OkObjectResult(returnCode);
            }
            catch (Exception e)
            {
                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}
