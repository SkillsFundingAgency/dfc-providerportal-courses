
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Spatial;
using Dfc.ProviderPortal.Packages;
using Dfc.ProviderPortal.Courses.Models;
using Dfc.ProviderPortal.Courses.Settings;
using Dfc.ProviderPortal.Courses.Interfaces;
using Document = Microsoft.Azure.Documents.Document;


namespace Dfc.ProviderPortal.Courses.Helpers
{
    public class SearchServiceWrapper : ISearchServiceWrapper
    {
        private readonly ILogger _log;
        private readonly ISearchServiceSettings _settings;
        private readonly IProviderServiceSettings _providerServiceSettings;
        private readonly IVenueServiceSettings _venueServiceSettings;
        private static SearchServiceClient _service;
        private static ISearchIndexClient _index;

        public SearchServiceWrapper(
            ILogger log,
            //IOptions<ProviderServiceSettings> providerServiceSettings,
            //IOptions<VenueServiceSettings> venueServiceSettings,
            ISearchServiceSettings settings)
        {
            Throw.IfNull(log, nameof(log));
            //Throw.IfNull(providerServiceSettings, nameof(providerServiceSettings));
            //Throw.IfNull(venueServiceSettings, nameof(venueServiceSettings));
            Throw.IfNull(settings, nameof(settings));

            _log = log;
            _settings = settings;
            //_providerServiceSettings = providerServiceSettings.Value;
            //_venueServiceSettings = venueServiceSettings.Value;

            _service = new SearchServiceClient(settings.SearchService, new SearchCredentials(settings.AdminKey)); // QueryKey));
            _index = _service?.Indexes?.GetClient(settings.Index);
        }

        public IEnumerable<IndexingResult> UploadBatch(
            IEnumerable<AzureSearchProviderModel> providers,
            IEnumerable<AzureSearchVenueModel> venues,
            IReadOnlyList<Document> documents,
            out int succeeded)
        {
            try {
                succeeded = 0;
                if (documents.Any()) {

                    //IEnumerable<AzureSearchCourse> courses = documents.Select(d => new AzureSearchCourse()
                    IEnumerable<Course> courses = documents.Select(d => new Course()
                    {
                        id = d.GetPropertyValue<Guid>("id"),
                        //CourseId = d.GetPropertyValue<IEnumerable<CourseRun>>("CourseRuns"), //CourseId"),
                        QualificationCourseTitle = d.GetPropertyValue<string>("QualificationCourseTitle"),
                        LearnAimRef = d.GetPropertyValue<string>("LearnAimRef"),
                        NotionalNVQLevelv2 = d.GetPropertyValue<string>("NotionalNVQLevelv2"),
                        UpdatedDate = d.GetPropertyValue<DateTime?>("UpdatedDate"),
                        //VenueName = d.GetPropertyValue<string>("VenueName"),
                        //VenueAddress = d.GetPropertyValue<string>("VenueAddress"),
                        //VenueLattitude = d.GetPropertyValue<string>("VenueLattitude"),
                        //VenueLongitude = d.GetPropertyValue<string>("VenueLongitude"),
                        //VenueLocation = d.GetPropertyValue<GeographyPoint>("VenueLocation"),
                        //VenueAttendancePattern = d.GetPropertyValue<int?>("VenueAttendancePattern"),
                        //ProviderName = d.GetPropertyValue<string>("ProviderName")
                        ProviderUKPRN = int.Parse(d.GetPropertyValue<string>("ProviderUKPRN")),
                        CourseRuns = d.GetPropertyValue<IEnumerable<CourseRun>>("CourseRuns")
                    }); //.ToList();

                    _log.LogInformation("Creating batch of course data to index");
                    IEnumerable<AzureSearchCourse> batchdata = from Course c in courses
                                                               from CourseRun r in c.CourseRuns ?? new List<CourseRun>()
                                                               join AzureSearchProviderModel p in providers
                                                               on c.ProviderUKPRN equals p.UnitedKingdomProviderReferenceNumber
                                                               //join AzureSearchVenueModel v in venues
                                                               from AzureSearchVenueModel v in venues.Where(v => r.VenueId == v.id)
                                                                                                     .DefaultIfEmpty()
                                                               select new AzureSearchCourse()
                                                               {
                                                                   id = r?.id,
                                                                   CourseId = c?.id,
                                                                   QualificationCourseTitle = c?.QualificationCourseTitle,
                                                                   LearnAimRef = c?.LearnAimRef,
                                                                   NotionalNVQLevelv2 = c?.NotionalNVQLevelv2,
                                                                   VenueName = v?.VENUE_NAME,
                                                                   VenueAddress = string.Format("{0}{1}{2}{3}{4}",
                                                                                  string.IsNullOrWhiteSpace(v?.ADDRESS_1) ? "" : v?.ADDRESS_1 + ", ",
                                                                                  string.IsNullOrWhiteSpace(v?.ADDRESS_2) ? "" : v?.ADDRESS_2 + ", ",
                                                                                  string.IsNullOrWhiteSpace(v?.TOWN) ? "" : v?.TOWN + ", ",
                                                                                  string.IsNullOrWhiteSpace(v?.COUNTY) ? "" : v?.COUNTY + ", ",
                                                                                  v?.POSTCODE),
                                                                   VenueAttendancePattern = r?.AttendancePattern,
                                                                   //VenueLocation = [to be populated later],
                                                                   ProviderName = p?.ProviderName,
                                                                   UpdatedOn = r?.UpdatedDate
                                                               };

                    IndexBatch<AzureSearchCourse> batch = IndexBatch.MergeOrUpload(batchdata);

                    _log.LogInformation("Merging docs to azure search index: course");
                    Task<DocumentIndexResult> task = _index.Documents.IndexAsync(batch);
                    task.Wait();
                    succeeded = batchdata.Count();
                    _log.LogInformation($"Successfully merged {succeeded} docs to azure search index: course");
                }

            } catch (IndexBatchException ex) {
                IEnumerable<IndexingResult> failed = ex.IndexingResults.Where(r => !r.Succeeded);
                _log.LogError(ex, string.Format("Failed to index some of the documents: {0}",
                                                string.Join(", ", failed)));
                //_log.LogError(ex.ToString());
                succeeded = ex.IndexingResults.Count(x => x.Succeeded);
                //return from Document d in documents
                //            join IndexingResult r in ex.IndexingResults.Where(x => !x.Succeeded)
                //            on d.Id equals r.Key
                //            select d;
                return failed;

            } catch (Exception e) {
                throw e;
            }

            // Return empty list of failed IndexingResults
            return new List<IndexingResult>();
        }
    }
}
