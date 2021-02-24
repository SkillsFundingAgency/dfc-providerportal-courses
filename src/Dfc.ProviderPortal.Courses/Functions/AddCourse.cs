using System;
using System.IO;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Courses.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;

namespace Dfc.ProviderPortal.Courses.Functions
{
    public class AddCourse
    {
        private readonly ICourseService _coursesService;

        public AddCourse(ICourseService coursesService)
        {
            _coursesService = coursesService;
        }

        [FunctionName("AddCourse")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
        {
            using (var streamReader = new StreamReader(req.Body))
            {
                var requestBody = await streamReader.ReadToEndAsync();

                Course fromBody = null;
                Course persisted = null;

                try
                {
                    fromBody = JsonConvert.DeserializeObject<Course>(requestBody);
                }
                catch (Exception e)
                {
                    return new BadRequestObjectResult(e);
                }

                try
                {
                    persisted = (Course) await _coursesService.AddCourse(fromBody);
                }
                catch (Exception e)
                {
                    return new InternalServerErrorObjectResult(e);
                }

                return new CreatedResult(persisted.id.ToString(), persisted);
            }
        }

    }
}
