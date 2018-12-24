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
//using Dfc.ProviderPortal.Courses.Models.Models.Courses;
using Dfc.CourseDirectory.Models.Models.Courses;
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
                log.LogInformation($"AddCourse starting");
                course = await req.Content.ReadAsAsync<Course>(); //(dynamic)<object>

                if (string.IsNullOrEmpty(course?.CourseData.CourseTitle))
                { 
                    response = req.CreateResponse(HttpStatusCode.BadRequest, ResponseHelper.ErrorMessage("Missing Course Title argument"));
                    log.LogInformation($"Missing Course Title argument");
                }
                else
                {
                    // Insert data as new document in collection
                    var results = await new CourseStorage().InsertDocAsync(course, log);

                    // We should check the status of InsertDocAsync operation and act accordingly
                    // https://docs.microsoft.com/en-us/rest/api/cosmos-db/create-a-document
                    // 201 - Created - The operation was successful.
                    //var resultsStatusCode = results.StatusCode;

                    if (results.StatusCode.Equals(HttpStatusCode.Created))
                    {
                        // Return results
                        log.LogInformation($"AddCourse Created new Course with id: " + course.id);
                        response = req.CreateResponse(HttpStatusCode.OK);
                        response.Content = new StringContent(JsonConvert.SerializeObject(course), Encoding.UTF8, "application/json");
                    }
                    else
                    {
                        log.LogInformation($"AddCourse did NOT create new course - StatusCode - " + results.StatusCode.ToString());
                        response = req.CreateResponse(results.StatusCode);
                        response.Content = new StringContent(JsonConvert.SerializeObject(course), Encoding.UTF8, "application/json");
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError("Course add service unknown error.", ex);
                log.LogInformation($"AddCourse ending in error");
                throw ex;
            }
            log.LogInformation($"AddCourse ending");
            return response;
        }
    }
}
