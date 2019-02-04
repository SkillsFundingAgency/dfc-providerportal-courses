﻿
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
        public class LINQComboClass
        {
            public Course Course { get; set; }
            public CourseRun Run { get; set; }
            public string Region { get; set; }
            public AzureSearchVenueModel Venue { get; set; }
        }

        private readonly ILogger _log;
        private readonly ISearchServiceSettings _settings;
        private readonly IProviderServiceSettings _providerServiceSettings;
        private readonly IVenueServiceSettings _venueServiceSettings;
        private static SearchServiceClient _queryService;
        private static SearchServiceClient _adminService;
        private static ISearchIndexClient _queryIndex;
        private static ISearchIndexClient _adminIndex;

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

            _queryService = new SearchServiceClient(settings.SearchService, new SearchCredentials(settings.QueryKey));
            _adminService = new SearchServiceClient(settings.SearchService, new SearchCredentials(settings.AdminKey));
            _queryIndex = _queryService?.Indexes?.GetClient(settings.Index);
            _adminIndex = _adminService?.Indexes?.GetClient(settings.Index);
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

                    IEnumerable<Course> courses = documents.Select(d => new Course()
                    {
                        id = d.GetPropertyValue<Guid>("id"),
                        QualificationCourseTitle = d.GetPropertyValue<string>("QualificationCourseTitle"),
                        LearnAimRef = d.GetPropertyValue<string>("LearnAimRef"),
                        NotionalNVQLevelv2 = d.GetPropertyValue<string>("NotionalNVQLevelv2"),
                        UpdatedDate = d.GetPropertyValue<DateTime?>("UpdatedDate"),
                        ProviderUKPRN = int.Parse(d.GetPropertyValue<string>("ProviderUKPRN")),
                        CourseRuns = d.GetPropertyValue<IEnumerable<CourseRun>>("CourseRuns")
                    });

                    _log.LogInformation("Creating batch of course data to index");

                    // Courses run in classrooms have an associated venue
                    IEnumerable<LINQComboClass> classroom =    from Course c in courses
                                                               from CourseRun r in c.CourseRuns ?? new List<CourseRun>()
                                                               from AzureSearchVenueModel v in venues.Where(x => r.VenueId == x.id)
                                                                                                     .DefaultIfEmpty()
                                                               select new LINQComboClass() { Course = c, Run = r, Region = (string)null, Venue = v };

                    // Courses run elsewhere have regions instead (online, work-based, ...)
                    IEnumerable<LINQComboClass> nonclassroom = from Course c in courses
                                                               from CourseRun r in c.CourseRuns ?? new List<CourseRun>()
                                                               from string region in r.Regions?.DefaultIfEmpty() ?? new List<string>()
                                                               select new LINQComboClass() { Course = c, Run = r, Region = region, Venue = (AzureSearchVenueModel)null };

                    var batchdata = from LINQComboClass x in classroom.Union(nonclassroom)
                                    join AzureSearchProviderModel p in providers
                                    on x.Course?.ProviderUKPRN equals p.UnitedKingdomProviderReferenceNumber
                                    where (x.Venue != null || x.Region != null)
                                    select new AzureSearchCourse()
                                    {
                                        id = x.Run?.id,
                                        CourseId = x.Course?.id,
                                        QualificationCourseTitle = x.Course?.QualificationCourseTitle,
                                        LearnAimRef = x.Course?.LearnAimRef,
                                        NotionalNVQLevelv2 = x.Course?.NotionalNVQLevelv2,
                                        VenueName = x.Venue?.VENUE_NAME,
                                        VenueAddress = string.Format("{0}{1}{2}{3}{4}",
                                                       string.IsNullOrWhiteSpace(x.Venue?.ADDRESS_1) ? "" : x.Venue?.ADDRESS_1 + ", ",
                                                       string.IsNullOrWhiteSpace(x.Venue?.ADDRESS_2) ? "" : x.Venue?.ADDRESS_2 + ", ",
                                                       string.IsNullOrWhiteSpace(x.Venue?.TOWN) ? "" : x.Venue?.TOWN + ", ",
                                                       string.IsNullOrWhiteSpace(x.Venue?.COUNTY) ? "" : x.Venue?.COUNTY + ", ",
                                                       x.Venue?.POSTCODE),
                                        VenueAttendancePattern = x.Run?.AttendancePattern,
                                        VenueLocation = GeographyPoint.Create(x.Venue?.Latitude ?? 0, x.Venue?.Longitude ?? 0),
                                        ProviderName = p?.ProviderName,
                                        UpdatedOn = x.Run?.UpdatedDate
                                    };


                    IndexBatch< AzureSearchCourse > batch = IndexBatch.MergeOrUpload(batchdata);

                    _log.LogInformation("Merging docs to azure search index: course");
                    Task<DocumentIndexResult> task = _adminIndex.Documents.IndexAsync(batch);
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
                return failed;

            } catch (Exception e) {
                throw e;
            }

            // Return empty list of failed IndexingResults
            return new List<IndexingResult>();
        }

        public DocumentSearchResult<AzureSearchCourse> SearchCourses(string SearchText)
        {
            try {
                _log.LogInformation($"Searching by {SearchText}");
                SearchParameters parms = new SearchParameters() {
                    //OrderBy = new[] { "id" },
                    Top = _settings.DefaultTop
                };
                DocumentSearchResult<AzureSearchCourse> results =
                    _queryIndex.Documents.Search<AzureSearchCourse>(SearchText, parms);
                _log.LogInformation($"{results.Count ?? 0} matches found");
                return results;

            } catch (Exception ex) {
                _log.LogError(ex, "Error in SearchCourses", SearchText);
                throw ex;
            }

        }
    }
}
