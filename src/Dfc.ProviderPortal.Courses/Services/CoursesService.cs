
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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

        public CoursesService(
            ICosmosDbHelper cosmosDbHelper,
            IOptions<CosmosDbCollectionSettings> settings)
        {
            Throw.IfNull(cosmosDbHelper, nameof(cosmosDbHelper));
            Throw.IfNull(settings, nameof(settings));

            _cosmosDbHelper = cosmosDbHelper;
            _settings = settings.Value;
        }

        //public Task<IEnumerable<ICourse>> FindACourse(ILogger log, IFACSearchCriteria criteria)
        //{
        //    Throw.IfNull(criteria, nameof(criteria));
        //    Throw.IfNullOrWhiteSpace(criteria?.Keyword, nameof(criteria.Keyword));
        //    //Throw.IfNullOrWhiteSpace(criteria?.QualificationLevel, nameof(criteria.QualificationLevel));
        //    //Throw.IfNullOrWhiteSpace(criteria?.LocationPostcode, nameof(criteria.LocationPostcode));
        //    //Throw.IfNullOrWhiteSpace(criteria?.DistanceInMiles, nameof(criteria.DistanceInMiles));

        //    IEnumerable<Course> persisted = null;
        //    using (var client = _cosmosDbHelper.GetClient())
        //    {
        //        var docs = _cosmosDbHelper.GetDocumentsByFACSearchCriteria(client, _settings.CoursesCollectionId, criteria);
        //        persisted = docs; // _cosmosDbHelper.DocumentsTo<Course>(docs);
        //    }

        //    return persisted;
        //}

        public async Task<IEnumerable<ICourse>> FindACourseAzureSearchData(ILogger log)
        {
            try {
                //// Get all course documents in the collection
                //string token = null;
                //Task<FeedResponse<dynamic>> task = null;
                //List<dynamic> docs = new List<dynamic>();
                //log.LogInformation("Getting all courses from collection");

                //// Read documents in batches, using continuation token to make sure we get them all
                //using (DocumentClient client = _cosmosDbHelper.GetClient()) {
                //    do {
                //        task = client.ReadDocumentFeedAsync(UriFactory.CreateDocumentCollectionUri("providerportal", _settings.CoursesCollectionId),
                //                                            new FeedOptions { MaxItemCount = -1, RequestContinuation = token });
                //        token = task.Result.ResponseContinuation;
                //        log.LogInformation("Collating results");
                //        docs.AddRange(task.Result.ToList());
                //    } while (token != null);
                //}

                //// Cast the returned data by serializing to json and then deserialising into Course objects
                //log.LogInformation($"Serializing data for {docs.LongCount()} courses");
                //string json = JsonConvert.SerializeObject(docs);
                //return JsonConvert.DeserializeObject<IEnumerable<Course>>(json);
                return await GetAllCourses(log); // List<Course>();

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
