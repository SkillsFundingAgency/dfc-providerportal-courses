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

    public class UpdateCourseMigrationReport
    {
        private readonly ICourseMigrationReportService _courseMigrationReportService;

        public UpdateCourseMigrationReport(ICourseMigrationReportService courseMigrationReportService)
        {
            _courseMigrationReportService = courseMigrationReportService;
        }

        [FunctionName("UpdateCourseMigrationReport")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestMessage req)
        {
            var courseMigrationReport = await req.Content.ReadAsAsync<CourseMigrationReport>();
            
            try
            {
                await _courseMigrationReportService.AddMigrationReport(courseMigrationReport);
                return new OkResult();

            }
            catch (Exception e)
            {
                return new InternalServerErrorObjectResult(e);
            }

        }
    }
}
