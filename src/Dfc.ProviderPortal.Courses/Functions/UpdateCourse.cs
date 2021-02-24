
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
    public class UpdateCourse
    {
        private readonly ICourseService _coursesService;

        public UpdateCourse(ICourseService coursesService)
        {
            _coursesService = coursesService;
        }

        [FunctionName("UpdateCourse")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestMessage req)
        {

            Course course = await req.Content.ReadAsAsync<Course>();

            try
            {
                var updatedCourse = (Course)await _coursesService.Update(course);
                return new OkObjectResult(updatedCourse);

            } catch (Exception e) {
                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}
