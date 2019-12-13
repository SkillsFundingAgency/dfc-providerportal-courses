
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Courses.Helpers;
using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Courses.Models;
using Dfc.ProviderPortal.Courses.Settings;
using Dfc.ProviderPortal.Packages;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dfc.ProviderPortal.Courses.Services
{
    public class CoursesService : ICourseService
    {
        //private readonly ILogger _log;
        private readonly ICosmosDbHelper _cosmosDbHelper;
        private readonly ICosmosDbCollectionSettings _settings;
        private readonly IProviderServiceSettings _providerServiceSettings;
        private readonly IVenueServiceSettings _venueServiceSettings;
        private readonly ISearchServiceSettings _searchServiceSettings;
        private readonly IQualificationServiceSettings _qualServiceSettings;
        private readonly ISearchServiceWrapper _searchServiceWrapper;

        public CoursesService(
            //ILogger log,
            ICosmosDbHelper cosmosDbHelper,
            ISearchServiceWrapper searchServiceWrapper,
            IOptions<ProviderServiceSettings> providerServiceSettings,
            IOptions<VenueServiceSettings> venueServiceSettings,
            IOptions<SearchServiceSettings> searchServiceSettings,
            IOptions<QualificationServiceSettings> qualServiceSettings,
            IOptions<CosmosDbCollectionSettings> settings)
        {
            //Throw.IfNull(log, nameof(log));
            Throw.IfNull(cosmosDbHelper, nameof(cosmosDbHelper));
            Throw.IfNull(searchServiceWrapper, nameof(searchServiceWrapper));
            Throw.IfNull(settings, nameof(settings));
            Throw.IfNull(providerServiceSettings, nameof(providerServiceSettings));
            Throw.IfNull(venueServiceSettings, nameof(venueServiceSettings));
            Throw.IfNull(qualServiceSettings, nameof(qualServiceSettings));
            Throw.IfNull(searchServiceSettings, nameof(searchServiceSettings));

            //_log = log;
            _cosmosDbHelper = cosmosDbHelper;
            _settings = settings.Value;
            _providerServiceSettings = providerServiceSettings.Value;
            _venueServiceSettings = venueServiceSettings.Value;
            _qualServiceSettings = qualServiceSettings.Value;
            _searchServiceSettings = searchServiceSettings.Value;
            _searchServiceWrapper = searchServiceWrapper;
        }

        /// <summary>
        /// Make trivial update to all courses in collection
        /// This will cause change feed listener to fire and update the courses index for each document
        /// </summary>
        /// <param name="log">ILogger</param>
        /// <returns>Affected courses</returns>
        public async Task<IEnumerable<ICourse>> TouchAllCourses(ILogger log)
        {
            try {
                DateTime start = DateTime.Now;

                IEnumerable<ICourse> courses = await GetAllCourses(log);
                var grouped = courses.Where(c => (c.CourseStatus & RecordStatus.Live) != 0)
                                     .GroupBy(c => c.ProviderUKPRN,
                                              c => c,
                                              (key, g) => new { PRN = key.ToString(), Courses = g.ToList() });

                int groupedCount = grouped.Count();
                int i = 0;
                log.LogInformation($"Refreshing course index for {groupedCount} providers");
                foreach (var group in grouped) {

                    // Remove existing courses for this PRN from the index
                    log.LogInformation($"Processing provider {i++} of {groupedCount}");
                    _searchServiceWrapper.DeleteCoursesByPRN(log, group.PRN);

                    // Make trivial change to each course in cosmos to ensure it is reindexed
                    log.LogInformation($"Refreshing {group.Courses.Count()} courses");
                    foreach(Course c in group.Courses) {
                        c.UpdatedDate = (c.UpdatedDate.HasValue ? c.UpdatedDate.Value.AddSeconds(0.25)
                                                                : DateTime.Now
                                        );
                        Task task = Update(c);
                        task.Wait();
                    }
                }

                // Delete documents more than 24hrs old, as we haven't just re-indexed it above,
                // so it doesn't represent a current course and shouldn't be there
                _searchServiceWrapper.DeleteCoursesBeforeDate(log, start.AddDays(-1));
                return courses;

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
                //List<dynamic> docs = new List<dynamic>();
                List<Course> docs = new List<Course>();
                log.LogInformation("Getting all courses from collection");

                // Read documents in batches, using continuation token to make sure we get them all
                using (DocumentClient client = _cosmosDbHelper.GetClient())
                {
                    do {
                        task = client.ReadDocumentFeedAsync(UriFactory.CreateDocumentCollectionUri("providerportal", _settings.CoursesCollectionId),
                                                            new FeedOptions { MaxItemCount = -1, RequestContinuation = token });
                        token = task.Result.ResponseContinuation;
                        //log.LogInformation("Collating results");
                        //docs.AddRange(task.Result.ToList());

                        // Cast the data by serializing to json and then deserialising into Course objects
                        log.LogInformation($"Serializing data for {task.Result.LongCount()} courses");
                        string json = JsonConvert.SerializeObject(task.Result.ToList());
                        docs.AddRange(JsonConvert.DeserializeObject<IEnumerable<Course>>(json));
                    } while (token != null);
                }
                return docs;

                //// Cast the returned data by serializing to json and then deserialising into Course objects
                //log.LogInformation($"Serializing data for {docs.LongCount()} courses");
                //string json = JsonConvert.SerializeObject(docs);
                //return JsonConvert.DeserializeObject<IEnumerable<Course>>(json);

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

            using (var client = _cosmosDbHelper.GetClient())
            {
                var doc = _cosmosDbHelper.GetDocumentById(client, _settings.CoursesCollectionId, CourseId);
                course = _cosmosDbHelper.DocumentTo<Course>(doc);
            }

            if (course == null || !(course?.CourseRuns.Any(cr => cr.id == RunId) ?? false))
            {
                return null;
            }

            var providerTask = new ProviderServiceWrapper(_providerServiceSettings).GetByPRN(course.ProviderUKPRN);
            var qualificationTask = new QualificationServiceWrapper(_qualServiceSettings).GetQualificationById(course.LearnAimRef);
            var providerVenuesTask = new VenueServiceWrapper(_venueServiceSettings).GetVenuesByPRN(course.ProviderUKPRN);

            await Task.WhenAll(providerTask, qualificationTask, providerVenuesTask);

            var provider = providerTask.Result;
            var qualification = qualificationTask.Result;
            var providerVenues = providerVenuesTask.Result;

            var courseRunVenueIds = new HashSet<Guid>(course.CourseRuns.Where(cr => cr.VenueId.HasValue).Select(cr => cr.VenueId.Value));
            var courseRunVenues = providerVenues.Where(v => courseRunVenueIds.Contains(((JObject)v)["id"].ToObject<Guid>()));

            return new AzureSearchCourseDetail()
            {
                Course = course,
                Provider = provider,
                Qualification = qualification,
                CourseRunVenues = courseRunVenues
            };
        }

        public async Task<ICourse> Update(ICourse course)
        {
            Throw.IfNull(course, nameof(course));

            try {
                Course updated = null;
                using (var client = _cosmosDbHelper.GetClient())
                {
                    var updatedDocument = await _cosmosDbHelper.UpdateDocumentAsync(client, _settings.CoursesCollectionId, course);
                    updated = _cosmosDbHelper.DocumentTo<Course>(updatedDocument);
                }
                return updated;

            } catch (Exception ex) {
                throw ex;
            }
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

        public async Task<List<string>> DeleteCoursesByUKPRN(int UKPRN)
        {
            Throw.IfNull<int>(UKPRN, nameof(UKPRN));
            Throw.IfLessThan(0, UKPRN, nameof(UKPRN));

            List<string> results = null;
            using (var client = _cosmosDbHelper.GetClient())
            {
                results = await _cosmosDbHelper.DeleteDocumentsByUKPRN(client, _settings.CoursesCollectionId, UKPRN);
            }

            return results;
        }
        public async Task<List<string>> DeleteBulkUploadCourses(int UKPRN)
        {
            Throw.IfNull<int>(UKPRN, nameof(UKPRN));
            Throw.IfLessThan(0, UKPRN, nameof(UKPRN));

            List<string> results = null;
            using (var client = _cosmosDbHelper.GetClient())
            {
                results = await _cosmosDbHelper.DeleteBulkUploadCourses(client, _settings.CoursesCollectionId, UKPRN);
            }

            return results;
        }
        public async Task<HttpResponseMessage> ArchiveProvidersLiveCourses(int UKPRN, int UIMode)
        {
            Throw.IfNull<int>(UKPRN, nameof(UKPRN));
            Throw.IfLessThan(0, UKPRN, nameof(UKPRN));
            Throw.IfNull(UIMode, nameof(UIMode));
            Throw.IfLessThan(0, UIMode, nameof(UIMode));
            Throw.IfGreaterThan(Enum.GetValues(typeof(RecordStatus)).Cast<int>().Max(), UIMode, nameof(UIMode));

            var status = RecordStatus.Undefined;

            switch ((UIMode)UIMode)
            {
                case Models.UIMode.BulkUpload:
                    {
                        status = RecordStatus.BulkUploadReadyToGoLive;
                        break;
                    }
                case Models.UIMode.Migration:
                    {
                        status = RecordStatus.MigrationReadyToGoLive;
                        break;
                    }

            }

            var allCourses = GetCoursesByUKPRN(UKPRN).Result;
            var coursesToArchive = allCourses.Where(x => x.CourseStatus == RecordStatus.Live).ToList();
            var coursesToMakeLive = allCourses.Where(x => x.CourseStatus == status).ToList();

            try
            {
                foreach (var course in coursesToArchive)
                {
                    foreach (var courseRun in course.CourseRuns)
                    {
                        if (courseRun.RecordStatus == RecordStatus.Live)
                            courseRun.RecordStatus = RecordStatus.Archived;
                    }
                    var result = Update(course);
                }

                if ((UIMode)UIMode != Models.UIMode.DeactivatedProvider)    // ensure nothing set live if provider is deactivated
                {
                    foreach (var course in coursesToMakeLive)
                    {
                        foreach (var courseRun in course.CourseRuns)
                        {
                            if (courseRun.RecordStatus == status)
                                courseRun.RecordStatus = RecordStatus.Live;
                        }
                        var result = Update(course);
                    }
                }

                return new HttpResponseMessage(HttpStatusCode.OK);

            } catch (Exception ex) {
                return new HttpResponseMessage(HttpStatusCode.ExpectationFailed);
            }
        }

        public async Task<HttpResponseMessage> ChangeCourseRunStatusesForUKPRNSelection(int UKPRN, RecordStatus CurrentStatus, RecordStatus StatusToBeChangedTo)
        {
            Throw.IfNull<int>(UKPRN, nameof(UKPRN));
            Throw.IfLessThan(0, UKPRN, nameof(UKPRN));

            var allCourses = GetCoursesByUKPRN(UKPRN).Result;
            var coursesToBeChanged = allCourses.Where(x => x.CourseRuns.Any(cr => cr.RecordStatus == CurrentStatus)).ToList();

            try
            {
                foreach (var course in coursesToBeChanged)
                {
                    foreach (var courseRun in course.CourseRuns)
                    {
                        if (courseRun.RecordStatus == CurrentStatus)
                            courseRun.RecordStatus = StatusToBeChangedTo;
                    }
                    var result = await Update(course);
                }

                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.ExpectationFailed);
            }
        }

        public async Task<HttpResponseMessage> ChangeAllCourseRunStatusesForUKPRNSelection(int UKPRN, RecordStatus StatusToBeChangedTo)
        {
            Throw.IfNull<int>(UKPRN, nameof(UKPRN));
            Throw.IfLessThan(0, UKPRN, nameof(UKPRN));

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var allCourses = GetCoursesByUKPRN(UKPRN).Result;
            sw.Stop();

            try
            {
                System.Diagnostics.Stopwatch sw2 = new System.Diagnostics.Stopwatch();
                sw2.Start();
                foreach (var course in allCourses)
                {
                    foreach (var courseRun in course.CourseRuns)
                    {
                        courseRun.RecordStatus = StatusToBeChangedTo;
                    }
                    var result = await Update(course);
                }

                sw2.Stop();

                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.ExpectationFailed);
            }
        }

        public async Task<HttpResponseMessage> UpdateStatus(Guid courseId, Guid courseRunId, int status)
        {
            Throw.IfNull(courseId, nameof(courseId));
            Throw.IfGreaterThan(Enum.GetValues(typeof(RecordStatus)).Cast<int>().Max(), status, nameof(status));
            Throw.IfLessThan(0, status, nameof(status));

            var course = GetCourseById(courseId).Result;

            if (course != null)
            {
                foreach (var courseRun in course.CourseRuns)
                {
                    if (courseRun.id == courseRunId)
                    {
                        courseRun.RecordStatus = (RecordStatus)status;
                    }
                }

                var result = await Update(course);
                if (result != null)
                {
                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.NotModified);
                }

            }

            return new HttpResponseMessage(HttpStatusCode.NotFound);

        }

        public async Task<int> GetTotalLiveCourses()
        {
            using (var documentClient = _cosmosDbHelper.GetClient())
            {
                return await _cosmosDbHelper.GetTotalLiveCourses(documentClient, _settings.CoursesCollectionId);
            }
        }
    }
}
