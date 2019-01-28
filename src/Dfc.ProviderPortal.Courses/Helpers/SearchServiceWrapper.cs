
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Spatial;
using Dfc.ProviderPortal.Packages;
using Dfc.ProviderPortal.Courses.Models;
using Dfc.ProviderPortal.Courses.Interfaces;
using Document = Microsoft.Azure.Documents.Document;


namespace Dfc.ProviderPortal.Courses.Helpers
{
    public class SearchServiceWrapper : ISearchServiceWrapper
    {
        private readonly ILogger _log;
        private readonly ISearchServiceSettings _settings;
        private static SearchServiceClient _service;
        private static ISearchIndexClient _index;

        public SearchServiceWrapper(ILogger log, ISearchServiceSettings settings)
        {
            Throw.IfNull(log, nameof(log));
            Throw.IfNull(settings, nameof(settings));

            _log = log;
            _settings = settings;

            _service = new SearchServiceClient(settings.SearchService, new SearchCredentials(settings.QueryKey));
            _index = _service?.Indexes?.GetClient(settings.Index);
        }

        public IEnumerable<Document> UploadBatch(IReadOnlyList<Document> documents, out int succeeded)
        {
            try {
                succeeded = 0;
                if (documents.Any()) {

                    IEnumerable<AzureSearchCourse> courses = documents.Select(d => new AzureSearchCourse()
                    {
                        id = d.GetPropertyValue<Guid?>("id"),
                        CourseId = d.GetPropertyValue<Guid?>("CourseId"),
                        QualificationCourseTitle = d.GetPropertyValue<string>("QualificationCourseTitle"),
                        LearnAimRef = d.GetPropertyValue<string>("id"),
                        NotionalNVQLevelv2 = d.GetPropertyValue<string>("NotionalNVQLevelv2"),
                        UpdatedOn = d.GetPropertyValue<DateTime?>("UpdatedOn"),
                        VenueName = d.GetPropertyValue<string>("VenueName"),
                        VenueAddress = d.GetPropertyValue<string>("VenueAddress"),
                        //VenueLattitude = d.GetPropertyValue<string>("VenueLattitude"),
                        //VenueLongitude = d.GetPropertyValue<string>("VenueLongitude"),
                        VenueLocation = d.GetPropertyValue<GeographyPoint>("VenueLocation"),
                        VenueAttendancePattern = d.GetPropertyValue<int?>("VenueAttendancePattern"),
                        ProviderName = d.GetPropertyValue<string>("ProviderName")
                    }).ToList();

                    IndexBatch<AzureSearchCourse> batch = IndexBatch.MergeOrUpload(courses);

                    _log.LogInformation("Merging docs to azure search index: course");
                    Task<DocumentIndexResult> task = _index.Documents.IndexAsync(batch);
                    task.Wait();
                    _log.LogInformation("Successfully merged docs to azure search index: course");
                    succeeded = courses.Count();
                }

            } catch (IndexBatchException ex) {
                _log.LogError(ex, string.Format("Failed to index some of the documents: {0}",
                                                string.Join(", ", ex.IndexingResults.Where(r => !r.Succeeded)
                                                                                    .Select(r => r.Key))));
                //_log.LogError(ex.ToString());
                succeeded = ex.IndexingResults.Count(x => x.Succeeded);
                return from Document d in documents
                            join IndexingResult r in ex.IndexingResults.Where(x => !x.Succeeded)
                            on d.Id equals r.Key
                            select d;

            } catch (Exception e) {
                throw e;
            }

            return new List<Document>();
        }
    }
}
