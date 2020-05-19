using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Courses.Models;
using Dfc.ProviderPortal.Packages.AzureFunctions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dfc.ProviderPortal.Courses.Functions
{
    public static class ChangeCourseRunStatusesForUKPRNSelection
    {
        [FunctionName("ChangeCourseRunStatusesForUKPRNSelection")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequestMessage req,
                                            ILogger log,
                                            [Inject] ICourseService coursesService)
        {
            var qryUKPRN = req.RequestUri.ParseQueryString()["UKPRN"]?.ToString()
                               ?? (await (dynamic)req.Content.ReadAsAsync<object>())?.UKPRN;
            var qryCurrentStatus = req.RequestUri.ParseQueryString()["CurrentStatus"]?.ToString()
                               ?? (await (dynamic)req.Content.ReadAsAsync<object>())?.CurrentStatus;
            var qryStatusToBeChangedTo = req.RequestUri.ParseQueryString()["StatusToBeChangedTo"]?.ToString()
                               ?? (await (dynamic)req.Content.ReadAsAsync<object>())?.StatusToBeChangedTo;

            if (string.IsNullOrWhiteSpace(qryUKPRN))
                return new BadRequestObjectResult($"Empty or missing UKPRN value.");

            if (!int.TryParse(qryUKPRN, out int UKPRN))
                return new BadRequestObjectResult($"Invalid UKPRN value, expected a valid integer");

            if (string.IsNullOrWhiteSpace(qryCurrentStatus))
                return new BadRequestObjectResult($"Empty or missing CurrentStatus value.");

            RecordStatus? CurrentStatus = null;          

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
                var returnCode = await coursesService.ChangeCourseRunStatusesForUKPRNSelection(UKPRN, CurrentStatus, StatusToBeChangedTo);

                return new OkObjectResult(returnCode);
            }
            catch (Exception e)
            {
                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}
