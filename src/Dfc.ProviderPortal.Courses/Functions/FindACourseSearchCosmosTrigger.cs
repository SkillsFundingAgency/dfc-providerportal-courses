
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Packages.AzureFunctions.DependencyInjection;
using Dfc.ProviderPortal.Courses.Models;
using Document = Microsoft.Azure.Documents.Document;


namespace Dfc.ProviderPortal.Courses.Functions
{
    public static class FindACourseSearchCosmosTrigger
    {
        [FunctionName("FindACourseSearchCosmosTrigger")]
        public static async Task Run(
            [CosmosDBTrigger("providerportal", "courses", ConnectionStringSetting = "SearchServiceSettings:CoursesConnectionString",
            //LeaseCollectionName = "courses-leases",
            CreateLeaseCollectionIfNotExists = true)]
            IReadOnlyList<Document> documents,
            //public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log,
            [Inject] ICourseService courseService)
        {
            //log.LogInformation("Entered FindACourseSearchCosmosTrigger");
            //Task<IEnumerable<IndexingResult>> results = null;
            //var stuff = await courseService.FindACourseAzureSearchData(log);
            //IReadOnlyList<Document> documents = new List<Document>(
            //    stuff.Select(x => new Document() { Id = Guid.NewGuid().ToString() })
            //).AsReadOnly();

            try {
                log.LogInformation("Entered FindACourseSearchCosmosTrigger");
                log.LogInformation($"Processing {documents.Count} documents for indexing to Azure search");
                IEnumerable<IndexingResult> results = await courseService.UploadCoursesToSearch(log, documents);
            } catch (Exception e) {
                log.LogError(e, "Indexing error in FindACourseSearchCosmosTrigger");
            }
        }
    }
}
