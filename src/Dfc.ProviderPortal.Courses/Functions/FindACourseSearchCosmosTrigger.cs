
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
        //[FunctionName("FindACourseSearchCosmosTrigger")]
        //public static async Task<IActionResult> Run(
        //    [CosmosDBTrigger("providerportal", "courses", ConnectionStringSetting = "CoursesConnectionString",
        //        LeaseCollectionName = "courses-leases", CreateLeaseCollectionIfNotExists = true)]
        //    IReadOnlyList<Document> documents,
        [FunctionName("FindACourseSearchCosmosTrigger")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log,
            [Inject] ICourseService courseService)
        {
            log.LogInformation("Entered FindACourseSearchCosmosTrigger");
            //IEnumerable<Document> failed = new List<Document>();


            var stuff = await courseService.FindACourseAzureSearchData(log);
            IReadOnlyList<Document> documents = new List<Document>(
                stuff.Select(x => new Document() { Id = Guid.NewGuid().ToString() })
            ).AsReadOnly();


            try {
                var results = await courseService.UploadCoursesToSearch(log, documents);
                return new OkObjectResult(results);

            } catch (Exception e) {
                return new InternalServerErrorObjectResult(e);
            }
        }
    }
}



//namespace NCS.DSS.Customer.AzureSearchDataSyncTrigger
//{
//    public static class CustomerSearchDataSyncTrigger
//    {
//        [FunctionName("SyncDataForCustomerSearchTrigger")]
//        public static async Task Run(
//            [CosmosDBTrigger("customers", "customers", ConnectionStringSetting = "CustomerConnectionString",
//                LeaseCollectionName = "customers-leases", CreateLeaseCollectionIfNotExists = true)]
//            IReadOnlyList<Document> documents,
//            TraceWriter log)
//        {
//            log.Info("Entered SyncDataForCustomerSearchTrigger");

//            SearchHelper.GetSearchServiceClient();

//            log.Info("get search service client");

//            var indexClient = SearchHelper.GetIndexClient();

//            log.Info("get index client");

//            log.Info("Documents modified " + documents.Count);

//            if (documents.Count > 0)
//            {
//                var customers = documents.Select(doc => new Models.Customer()
//                {
//                    CustomerId = doc.GetPropertyValue<Guid?>("id"),
//                    DateOfRegistration = doc.GetPropertyValue<DateTime?>("DateOfRegistration"),
//                    GivenName = doc.GetPropertyValue<string>("GivenName"),
//                    FamilyName = doc.GetPropertyValue<string>("FamilyName"),
//                    DateofBirth = doc.GetPropertyValue<DateTime?>("DateofBirth"),
//                    UniqueLearnerNumber = doc.GetPropertyValue<string>("UniqueLearnerNumber"),
//                    OptInUserResearch = doc.GetPropertyValue<bool?>("OptInUserResearch"),
//                    OptInMarketResearch = doc.GetPropertyValue<bool?>("OptInMarketResearch"),
//                    DateOfTermination = doc.GetPropertyValue<DateTime?>("DateOfTermination"),
//                    ReasonForTermination = doc.GetPropertyValue<ReasonForTermination?>("ReasonForTermination"),
//                    IntroducedBy = doc.GetPropertyValue<IntroducedBy?>("IntroducedBy"),
//                    IntroducedByAdditionalInfo = doc.GetPropertyValue<string>("IntroducedByAdditionalInfo"),
//                    LastModifiedDate = doc.GetPropertyValue<DateTime?>("LastModifiedDate"),
//                    LastModifiedTouchpointId = doc.GetPropertyValue<string>("LastModifiedTouchpointId")
//                })
//                    .ToList();

//                var batch = IndexBatch.MergeOrUpload(customers);

//                try
//                {
//                    log.Info("attempting to merge docs to azure search");

//                    await indexClient.Documents.IndexAsync(batch);

//                    log.Info("successfully merged docs to azure search");

//                }
//                catch (IndexBatchException e)
//                {
//                    log.Error(string.Format("Failed to index some of the documents: {0}",
//                        string.Join(", ", e.IndexingResults.Where(r => !r.Succeeded).Select(r => r.Key))));

//                    log.Error(e.ToString());
//                }
//            }
//        }
//    }
//}
