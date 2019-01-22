
using System;
using System.Net.Http;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Dfc.ProviderPortal.Courses.Helpers;
using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Courses.Models;
using Dfc.ProviderPortal.Courses.Settings;
using Dfc.ProviderPortal.Packages;


namespace Dfc.ProviderPortal.Courses.Services
{
    public class CoursesService : ICourseService
    {
        private readonly ICosmosDbHelper _cosmosDbHelper;
        private readonly ICosmosDbCollectionSettings _settings;
        private readonly IProviderServiceSettings _providerServiceSettings;
        private readonly IVenueServiceSettings _venueServiceSettings;

        public CoursesService(
            ICosmosDbHelper cosmosDbHelper,
            IOptions<ProviderServiceSettings> providerServiceSettings,
            IOptions<VenueServiceSettings> venueServiceSettings,
            IOptions<CosmosDbCollectionSettings> settings)
        {
            Throw.IfNull(cosmosDbHelper, nameof(cosmosDbHelper));
            Throw.IfNull(settings, nameof(settings));
            Throw.IfNull(providerServiceSettings, nameof(providerServiceSettings));
            Throw.IfNull(venueServiceSettings, nameof(venueServiceSettings));

            _cosmosDbHelper = cosmosDbHelper;
            _settings = settings.Value;
            _providerServiceSettings = providerServiceSettings.Value;
            _venueServiceSettings = venueServiceSettings.Value;
        }

        public async Task<IEnumerable<AzureSearchCourse>> FindACourseAzureSearchData(ILogger log)
        {
            try {
                IEnumerable<ICourse> persisted = await GetAllCourses(log);
                IEnumerable<AzureSearchProviderModel> providers = new ProviderServiceWrapper(_providerServiceSettings).GetLiveProvidersForAzureSearch();
                IEnumerable<AzureSearchVenueModel> venues = new VenueServiceWrapper(_venueServiceSettings).GetVenues();

                IEnumerable<AzureSearchCourse> results = from ICourse c in persisted
                                                          from CourseRun cr in c.CourseRuns ?? new List<CourseRun>()
                                                          join AzureSearchProviderModel p in providers
                                                          on c.ProviderUKPRN equals p.UnitedKingdomProviderReferenceNumber
                                                          join AzureSearchVenueModel v in venues
                                                          on cr.VenueId equals v.id
                                                          select new AzureSearchCourse()
                                                          {
                                                              id = c.id,
                                                              QualificationCourseTitle = c.QualificationCourseTitle,
                                                              LearnAimRef = c.LearnAimRef,
                                                              NotionalNVQLevelv2 = c.NotionalNVQLevelv2,
                                                              VenueName = (from AzureSearchVenueModel vm in venues
                                                                           join Guid id in c.CourseRuns.Where(r => r.VenueId.HasValue).Select(r => r.VenueId.Value) on vm.id equals id
                                                                           select vm.VENUE_NAME).ToArray(),
                                                              VenueAddress = (from AzureSearchVenueModel vm in venues
                                                                              join Guid id in c.CourseRuns.Where(r => r.VenueId.HasValue).Select(r => r.VenueId.Value) on vm.id equals id
                                                                              select vm.ADDRESS_1 + vm.ADDRESS_2 + vm.TOWN + vm.COUNTY + vm.POSTCODE).ToArray(),
                                                              VenueAttendancePattern = c.CourseRuns.Select(r => r.AttendancePattern).ToArray(),
                                                              //VenueLattitude = ???,
                                                              //VenueLongitude = ???,
                                                              ProviderName = p.ProviderName
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

        public async Task<IEnumerable<ICourse>> GetCoursesByUKPRN(int UKPRN)
        {
            Throw.IfNull<int>(UKPRN, nameof(UKPRN));
            Throw.IfLessThan(0, UKPRN, nameof(UKPRN));

            IEnumerable<Course> persisted = null;
            using (var client = _cosmosDbHelper.GetClient()) {
                var docs = _cosmosDbHelper.GetDocumentsByUKPRN(client, _settings.CoursesCollectionId, UKPRN);
                persisted = docs; // _cosmosDbHelper.DocumentsTo<Course>(docs);
            }

            return persisted;
        }
    }
}
