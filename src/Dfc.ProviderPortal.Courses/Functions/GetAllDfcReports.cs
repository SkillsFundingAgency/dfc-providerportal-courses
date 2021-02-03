using System;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Courses.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Dfc.ProviderPortal.Courses.Functions
{
    public class GetAllDfcReports
    {
        private readonly IDfcReportService _dfcReportService;

        public GetAllDfcReports(IDfcReportService dfcReportService)
        {
            _dfcReportService = dfcReportService;
        }

        [FunctionName("GetAllDfcReports")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {
            try
            {
                var result = await _dfcReportService.GetDfcReports();
                return new OkObjectResult(result);

            }
            catch (Exception e)
            {
                return new InternalServerErrorObjectResult(e);
            }

        }
    }
}
