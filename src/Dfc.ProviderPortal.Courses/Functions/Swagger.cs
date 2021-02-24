using System.Net;
using System.Net.Http;
using System.Reflection;
using DFC.Swagger.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Dfc.ProviderPortal.Courses.Functions
{
    public class Swagger
    {
        public const string ApiTitle = "Course Directory Course API";
        public const string ApiDefinitionName = "swagger.json";
        public const string ApiDefRoute = ApiTitle + "/" + ApiDefinitionName;
        public const string ApiVersion = "v1";

        private readonly ISwaggerDocumentGenerator _swaggerDocumentGenerator;

        public Swagger(ISwaggerDocumentGenerator swaggerDocumentGenerator)
        {
            _swaggerDocumentGenerator = swaggerDocumentGenerator;
        }

        [FunctionName("Swagger")]
        public HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "swagger.json")] HttpRequest req,
            ILogger log)
        {
            var swagger = _swaggerDocumentGenerator.GenerateSwaggerDocument(
                req,
                ApiTitle,
                apiDescription: ApiTitle,  // Can't be empty
                ApiDefinitionName,
                ApiVersion,
                Assembly.GetExecutingAssembly());

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(swagger)
            };
        }
    }
}
