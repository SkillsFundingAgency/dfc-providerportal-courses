
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
        //private readonly ISearchServiceWrapper _searchServiceWrapper;

        public CoursesService(
            ICosmosDbHelper cosmosDbHelper,
            //ISearchServiceWrapper searchServiceWrapper,
            IOptions<ProviderServiceSettings> providerServiceSettings,
            IOptions<VenueServiceSettings> venueServiceSettings,
            IOptions<SearchServiceSettings> searchServiceSettings,
            IOptions<CosmosDbCollectionSettings> settings)
        {
            Throw.IfNull(cosmosDbHelper, nameof(cosmosDbHelper));
            //Throw.IfNull(searchServiceWrapper, nameof(searchServiceWrapper));
            Throw.IfNull(settings, nameof(settings));
            Throw.IfNull(providerServiceSettings, nameof(providerServiceSettings));
            Throw.IfNull(venueServiceSettings, nameof(venueServiceSettings));
            Throw.IfNull(searchServiceSettings, nameof(searchServiceSettings));

            _cosmosDbHelper = cosmosDbHelper;
            _settings = settings.Value;
            _providerServiceSettings = providerServiceSettings.Value;
            _venueServiceSettings = venueServiceSettings.Value;
            _searchServiceSettings = searchServiceSettings.Value;
            //_searchServiceWrapper = searchServiceWrapper;
        }

        public async Task<IEnumerable<IndexingResult>> UploadCoursesToSearch(ILogger log, IReadOnlyList<Document> documents)
        {
            if (documents.Any()) {

                log.LogInformation("Getting provider data");
                IEnumerable<AzureSearchProviderModel> providers = new ProviderServiceWrapper(_providerServiceSettings).GetLiveProvidersForAzureSearch();

                log.LogInformation("Getting venue data");
                IEnumerable<AzureSearchVenueModel> venues = new VenueServiceWrapper(_venueServiceSettings).GetVenues();
                
                //return _searchServiceWrapper.UploadBatch(documents, out int succeeded);
                return new SearchServiceWrapper(log, _searchServiceSettings)
                        .UploadBatch(providers, venues, documents, out int succeeded);
            } else {
                // Return empty list of failed IndexingResults
                return new List<IndexingResult>();
            }
        }

        public async Task<DocumentSearchResult<AzureSearchCourse>> SearchCourses(ILogger log, string SearchText)
        {
            return new SearchServiceWrapper(log, _searchServiceSettings)
                        .SearchCourses(SearchText);
        }

        public async Task<IEnumerable<IAzureSearchCourse>> FindACourseAzureSearchData(ILogger log)
        {
            try {
                IEnumerable<ICourse> persisted = await GetAllCourses(log);
                IEnumerable<AzureSearchProviderModel> providers = new ProviderServiceWrapper(_providerServiceSettings).GetLiveProvidersForAzureSearch();
                IEnumerable<AzureSearchVenueModel> venues = new VenueServiceWrapper(_venueServiceSettings).GetVenues();

                IEnumerable<IAzureSearchCourse> results = from ICourse c in persisted
                                                          from CourseRun cr in c.CourseRuns ?? new List<CourseRun>()
                                                          join AzureSearchProviderModel p in providers
                                                          on c.ProviderUKPRN equals p.UnitedKingdomProviderReferenceNumber
                                                          from vm in venues.Where(v => cr.VenueId == v.id)
                                                                           .DefaultIfEmpty()
                                                          select new AzureSearchCourse()
                                                          {
                                                              id = cr.id,
                                                              CourseId = c.id,
                                                              QualificationCourseTitle = c.QualificationCourseTitle,
                                                              LearnAimRef = c.LearnAimRef,
                                                              NotionalNVQLevelv2 = c.NotionalNVQLevelv2,
                                                              VenueName = vm?.VENUE_NAME,
                                                              VenueAddress = string.Format("{0}{1}{2}{3}{4}",
                                                                             string.IsNullOrWhiteSpace(vm?.ADDRESS_1) ? "" : vm?.ADDRESS_1 + ", ",
                                                                             string.IsNullOrWhiteSpace(vm?.ADDRESS_2) ? "" : vm?.ADDRESS_2 + ", ",
                                                                             string.IsNullOrWhiteSpace(vm?.TOWN) ? "" : vm?.TOWN + ", ",
                                                                             string.IsNullOrWhiteSpace(vm?.COUNTY) ? "" : vm?.COUNTY + ", ",
                                                                             vm?.POSTCODE),
                                                              VenueAttendancePattern = cr.AttendancePattern,
                                                              //VenueLattitude = ???,
                                                              //VenueLongitude = ???,
                                                              ProviderName = p.ProviderName,
                                                              UpdatedOn = c.UpdatedDate
                                                          };
                return results;

            } catch (Exception ex) {
                throw ex;
            }
        }

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
