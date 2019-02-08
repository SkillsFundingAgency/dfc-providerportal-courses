
using System;
using System.Net.Http;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Spatial;
using Newtonsoft.Json;
using Dfc.ProviderPortal.Courses.Helpers;
using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Courses.Models;
using Dfc.ProviderPortal.Courses.Settings;
using Dfc.ProviderPortal.Packages;
using Document = Microsoft.Azure.Documents.Document;


namespace Dfc.ProviderPortal.Courses.Services
{
    public class CoursesService : ICourseService
    {
        private readonly ICosmosDbHelper _cosmosDbHelper;
        private readonly ICosmosDbCollectionSettings _settings;
        private readonly IProviderServiceSettings _providerServiceSettings;
        private readonly IVenueServiceSettings _venueServiceSettings;
        private readonly ISearchServiceSettings _searchServiceSettings;
        private readonly IQualificationServiceSettings _qualServiceSettings;
        //private readonly ISearchServiceWrapper _searchServiceWrapper;

        public CoursesService(
            ICosmosDbHelper cosmosDbHelper,
            //ISearchServiceWrapper searchServiceWrapper,
            IOptions<ProviderServiceSettings> providerServiceSettings,
            IOptions<VenueServiceSettings> venueServiceSettings,
            IOptions<SearchServiceSettings> searchServiceSettings,
            IOptions<QualificationServiceSettings> qualServiceSettings,
            IOptions<CosmosDbCollectionSettings> settings)
        {
            Throw.IfNull(cosmosDbHelper, nameof(cosmosDbHelper));
            //Throw.IfNull(searchServiceWrapper, nameof(searchServiceWrapper));
            Throw.IfNull(settings, nameof(settings));
            Throw.IfNull(providerServiceSettings, nameof(providerServiceSettings));
            Throw.IfNull(venueServiceSettings, nameof(venueServiceSettings));
            Throw.IfNull(qualServiceSettings, nameof(qualServiceSettings));
            Throw.IfNull(searchServiceSettings, nameof(searchServiceSettings));

            _cosmosDbHelper = cosmosDbHelper;
            _settings = settings.Value;
            _providerServiceSettings = providerServiceSettings.Value;
            _venueServiceSettings = venueServiceSettings.Value;
            _qualServiceSettings = qualServiceSettings.Value;
            _searchServiceSettings = searchServiceSettings.Value;
            //_searchServiceWrapper = searchServiceWrapper;
        }

        private IEnumerable<AzureSearchVenueModel> GetVenues(IEnumerable<CourseRun> runs = null)
        {
            IVenueServiceWrapper service = new VenueServiceWrapper(_venueServiceSettings);

            // Get all venues to save time & RUs if there's too many to get by Id
            if (runs == null || runs.Count() > _searchServiceSettings.ThresholdVenueCount)
                return service.GetVenues();
            else {
                List<AzureSearchVenueModel> venues = new List<AzureSearchVenueModel>();
                foreach (CourseRun r in runs.Where(x => x.VenueId != null))
                    venues.Add(service.GetById<AzureSearchVenueModel>(r.VenueId.Value));
                return venues;
            }
        }

        //private IEnumerable<dynamic> GetVenue(Guid id)
        //{
        //    return (dynamic) new VenueServiceWrapper(_venueServiceSettings).GetById(id);
        //}

        public async Task<IEnumerable<IndexingResult>> UploadCoursesToSearch(ILogger log, IReadOnlyList<Document> documents)
        {
            if (documents.Any()) {

                log.LogInformation("Getting provider data");
                IEnumerable<AzureSearchProviderModel> providers = new ProviderServiceWrapper(_providerServiceSettings).GetLiveProvidersForAzureSearch();

                log.LogInformation("Getting venue data");
                IEnumerable<AzureSearchVenueModel> venues = GetVenues(
                    documents.Select(d => new Course() { CourseRuns = d.GetPropertyValue<IEnumerable<CourseRun>>("CourseRuns") })
                             .SelectMany(c => c.CourseRuns)
                );
                
                return new SearchServiceWrapper(log, _searchServiceSettings)
                        .UploadBatch(providers, venues, documents, out int succeeded);
            } else {
                // Return empty list of failed IndexingResults
                return new List<IndexingResult>();
            }
        }

        public async Task<FACSearchResult> CourseSearch(ILogger log, SearchCriteriaStructure criteria) // string SearchText)
        {
            return new SearchServiceWrapper(log, _searchServiceSettings)
                        .SearchCourses(criteria); // SearchText);
        }

        //public async Task<IEnumerable<IAzureSearchCourse>> FindACourseAzureSearchData(ILogger log)
        //{
        //    try {
        //        IEnumerable<ICourse> persisted = await GetAllCourses(log);
        //        IEnumerable<AzureSearchProviderModel> providers = new ProviderServiceWrapper(_providerServiceSettings).GetLiveProvidersForAzureSearch();
        //        IEnumerable<AzureSearchVenueModel> venues = GetVenues(persisted.SelectMany(p => p.CourseRuns ?? new List<CourseRun>())); //new VenueServiceWrapper(_venueServiceSettings).GetVenues();

        //        IEnumerable<IAzureSearchCourse> results = from ICourse c in persisted
        //                                                  from CourseRun cr in c.CourseRuns ?? new List<CourseRun>()
        //                                                  join AzureSearchProviderModel p in providers
        //                                                  on c.ProviderUKPRN equals p.UnitedKingdomProviderReferenceNumber
        //                                                  from vm in venues.Where(v => cr.VenueId == v.id)
        //                                                                   .DefaultIfEmpty()
        //                                                  select new AzureSearchCourse()
        //                                                  {
        //                                                      id = cr.id,
        //                                                      CourseId = c.id,
        //                                                      QualificationCourseTitle = c.QualificationCourseTitle,
        //                                                      LearnAimRef = c.LearnAimRef,
        //                                                      NotionalNVQLevelv2 = c.NotionalNVQLevelv2,
        //                                                      VenueName = vm?.VENUE_NAME,
        //                                                      VenueAddress = string.Format("{0}{1}{2}{3}{4}",
        //                                                                     string.IsNullOrWhiteSpace(vm?.ADDRESS_1) ? "" : vm?.ADDRESS_1 + ", ",
        //                                                                     string.IsNullOrWhiteSpace(vm?.ADDRESS_2) ? "" : vm?.ADDRESS_2 + ", ",
        //                                                                     string.IsNullOrWhiteSpace(vm?.TOWN) ? "" : vm?.TOWN + ", ",
        //                                                                     string.IsNullOrWhiteSpace(vm?.COUNTY) ? "" : vm?.COUNTY + ", ",
        //                                                                     vm?.POSTCODE),
        //                                                      VenueAttendancePattern = cr.AttendancePattern,
        //                                                      VenueLocation = GeographyPoint.Create(vm?.Latitude ?? 0, vm?.Longitude ?? 0),
        //                                                      ProviderName = p.ProviderName,
        //                                                      UpdatedOn = c.UpdatedDate
        //                                                  };
        //        return results;

        //    } catch (Exception ex) {
        //        throw ex;
        //    }
        //}

        public async Task<IEnumerable<ICourse>> GetAllCourses(ILogger log)
        {
            try {
                // Get all course documents in the collection
                string token = null;
                Task<FeedResponse<dynamic>> task = null;
                List<dynamic> docs = new List<dynamic>();
                log.LogInformation("Getting all courses from collection");

                // Read documents in batches, using continuation token to make sure we get them all
                using (DocumentClient client = _cosmosDbHelper.GetClient()) {
                    do {
                        task = client.ReadDocumentFeedAsync(UriFactory.CreateDocumentCollectionUri("providerportal", _settings.CoursesCollectionId),
                                                            new FeedOptions { MaxItemCount = -1, RequestContinuation = token });
                        token = task.Result.ResponseContinuation;
                        log.LogInformation("Collating results");
                        docs.AddRange(task.Result.ToList());
                    } while (token != null);
                }

                // Cast the returned data by serializing to json and then deserialising into Course objects
                log.LogInformation($"Serializing data for {docs.LongCount()} courses");
                string json = JsonConvert.SerializeObject(docs);
                return JsonConvert.DeserializeObject<IEnumerable<Course>>(json);

            } catch (Exception ex) {
                throw ex;
            }
        }

        public async Task<ICourse> AddCourse(ICourse course)
        {
            Throw.IfNull(course, nameof(course));

            Course persisted;

            using (var client = _cosmosDbHelper.GetClient())
            {
                await _cosmosDbHelper.CreateDatabaseIfNotExistsAsync(client);
                await _cosmosDbHelper.CreateDocumentCollectionIfNotExistsAsync(client, _settings.CoursesCollectionId);
                var doc = await _cosmosDbHelper.CreateDocumentAsync(client, _settings.CoursesCollectionId, course);
                persisted = _cosmosDbHelper.DocumentTo<Course>(doc);
            }

            return persisted;
        }

        public async Task<ICourse> GetCourseById(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException($"Cannot be an empty {nameof(Guid)}", nameof(id));

            Course persisted = null;

            using (var client = _cosmosDbHelper.GetClient())
            {
                var doc = _cosmosDbHelper.GetDocumentById(client, _settings.CoursesCollectionId, id);
                persisted = _cosmosDbHelper.DocumentTo<Course>(doc);
            }

            return persisted;
        }

        public async Task<AzureSearchCourseDetail> GetCourseSearchDataById(Guid CourseId, Guid RunId)
        {
            if (CourseId == Guid.Empty)
                throw new ArgumentException($"Cannot be an empty {nameof(Guid)}", nameof(CourseId));
            if (RunId == Guid.Empty)
                throw new ArgumentException($"Cannot be an empty {nameof(Guid)}", nameof(RunId));

            Course course = null;
            dynamic venue = null;

            using (var client = _cosmosDbHelper.GetClient()) {
                var doc = _cosmosDbHelper.GetDocumentById(client, _settings.CoursesCollectionId, CourseId);
                course = _cosmosDbHelper.DocumentTo<Course>(doc);
            }

            //CourseRun run = course.CourseRuns.FirstOrDefault(r => r.id == RunId);
            Guid? venueid = course.CourseRuns
                                  .Where(r => r.id == RunId && r.VenueId != null)
                                  .FirstOrDefault()
                                  ?.VenueId;
            if (venueid.HasValue)
                venue = (dynamic)new VenueServiceWrapper(_venueServiceSettings).GetById<dynamic>(venueid.Value);
            var provider = new ProviderServiceWrapper(_providerServiceSettings).GetByPRN(course.ProviderUKPRN);
            var qualification = new QualificationServiceWrapper(_qualServiceSettings).GetQualificationById(course.LearnAimRef);

            //return from Course c in new List<Course>() { course }
            //       from CourseRun r in c.CourseRuns
            //       from AzureSearchProviderModel p in new List<AzureSearchProviderModel>() { provider }
            //       from AzureSearchVenueModel v in venues
            //       select new AzureSearchCourseDetail();
            return new AzureSearchCourseDetail()
            {
                Course = course,
                Provider = provider,
                Qualification = qualification,
                Venue = venue
            };
        }

        public async Task<ICourse> Update(ICourse course)
        {
            Throw.IfNull(course, nameof(course));
         
            Course updated = null;

            using (var client = _cosmosDbHelper.GetClient())
            {
                var updatedDocument = await _cosmosDbHelper.UpdateDocumentAsync(client, _settings.CoursesCollectionId, course);

                updated = _cosmosDbHelper.DocumentTo<Course>(updatedDocument);
            }

            return updated;

        }

        public async Task<IEnumerable<ICourse>> GetCoursesByUKPRN(int UKPRN)
        {
            Throw.IfNull<int>(UKPRN, nameof(UKPRN));
            Throw.IfLessThan(0, UKPRN, nameof(UKPRN));

            IEnumerable<Course> persisted = null;
            using (var client = _cosmosDbHelper.GetClient())
            {
                var docs = _cosmosDbHelper.GetDocumentsByUKPRN(client, _settings.CoursesCollectionId, UKPRN);
                persisted = docs;
            }

            return persisted;
        }
    }
}
