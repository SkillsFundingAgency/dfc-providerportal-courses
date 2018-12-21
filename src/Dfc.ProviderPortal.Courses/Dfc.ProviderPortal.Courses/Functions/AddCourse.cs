using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Dfc.ProviderPortal.Courses.Helpers;
using Dfc.ProviderPortal.Courses.Models.Models.Courses;
using Dfc.ProviderPortal.Courses.Storage;

namespace Dfc.ProviderPortal.Courses.Functions
{
    public static class AddCourse
    {
        [FunctionName("AddCourse")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req,
                                                  ILogger log)
        {
            Course course = null;
            HttpResponseMessage response = req.CreateResponse(HttpStatusCode.InternalServerError);

            try
            {
                // Get passed argument (from query if present, if from JSON posted in body if not)
                log.LogInformation($"AddVenue starting");
                course = await req.Content.ReadAsAsync<Course>(); //(dynamic)<object>


                if (course.ID == null)

                    response = req.CreateResponse(HttpStatusCode.BadRequest, ResponseHelper.ErrorMessage("Missing ADDRESS_1 argument"));

                else
                {
                    // Insert data as new document in collection
                    var results = await new CourseStorage().InsertDocAsync(course, log);

                    // Return results
                    response = req.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(JsonConvert.SerializeObject(course), Encoding.UTF8, "application/json");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }
    }
}
