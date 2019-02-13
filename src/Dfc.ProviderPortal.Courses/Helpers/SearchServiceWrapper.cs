
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Spatial;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Dfc.GeoCoordinate;
using Dfc.ProviderPortal.Packages;
using Dfc.ProviderPortal.Courses.Models;
using Dfc.ProviderPortal.Courses.Settings;
using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Courses.Services;
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
        //private readonly IProviderServiceSettings _providerServiceSettings;
        //private readonly IVenueServiceSettings _venueServiceSettings;
        private static SearchServiceClient _queryService;
        private static SearchServiceClient _adminService;
        private static ISearchIndexClient _queryIndex;
        private static ISearchIndexClient _adminIndex;
        private static ISearchIndexClient _onspdIndex;
        private HttpClient _httpClient;
        private readonly Uri _uri;

        public SearchServiceWrapper(
            ILogger log,
            //HttpClient httpClient,
            //IOptions<ProviderServiceSettings> providerServiceSettings,
            //IOptions<VenueServiceSettings> venueServiceSettings,
            ISearchServiceSettings settings)
        {
            Throw.IfNull(log, nameof(log));
            //Throw.IfNull(httpClient, nameof(httpClient));
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
            _onspdIndex = _queryService?.Indexes?.GetClient(settings.onspdIndex);

            _httpClient = new HttpClient(); //httpClient;
            //_httpClient.DefaultRequestHeaders.Accept.Clear();
            //_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //_httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
            //_httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
            _httpClient.DefaultRequestHeaders.Add("api-key", settings.QueryKey);
            _httpClient.DefaultRequestHeaders.Add("api-version", settings.ApiVersion);
            _httpClient.DefaultRequestHeaders.Add("indexes", settings.Index);
            _uri = new Uri($"{settings.ApiUrl}?api-version={settings.ApiVersion}");
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
                                    where (x.Run?.RecordStatus != RecordStatus.Pending && (x.Venue != null || x.Region != null))
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
                                        VenueAttendancePattern = ((int)x.Run?.AttendancePattern).ToString(),
                                        VenueLocation = GeographyPoint.Create(x.Venue?.Latitude ?? 0, x.Venue?.Longitude ?? 0),
                                        ProviderName = p?.ProviderName,
                                        Region = x.Region,
                                        Status = (int?)x.Run?.RecordStatus,
                                        UpdatedOn = x.Run?.UpdatedDate
                                    };

                    if (batchdata.Any()) {
                        IndexBatch<AzureSearchCourse> batch = IndexBatch.MergeOrUpload(batchdata);

                        _log.LogInformation("Merging docs to azure search index: course");
                        Task<DocumentIndexResult> task = _adminIndex.Documents.IndexAsync(batch);
                        task.Wait();
                        succeeded = batchdata.Count();
                    }
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


        public FACSearchResult SearchCourses(SearchCriteriaStructure criteria)
        {
            //try {
            //    _log.LogInformation($"Searching by {criteria.SubjectKeywordField}");
            //    SearchParameters parms = new SearchParameters() {
            //        //OrderBy = new[] { "id" },
            //        Top = criteria.TopResults ?? _settings.DefaultTop
            //    };
            //    DocumentSearchResult<AzureSearchCourse> results =
            //        _queryIndex.Documents.Search<AzureSearchCourse>(criteria.SubjectKeywordField, parms); // SearchText, parms);
            //    _log.LogInformation($"{results.Count ?? 0} matches found");
            //    return results;

            //} catch (Exception ex) {
            //    _log.LogError(ex, "Error in SearchCourses", criteria);
            //    throw ex;
            //}


            Throw.IfNull(criteria, nameof(criteria));

            //_log.LogMethodEnter();

            try
            {
                _log.LogInformation("FAC search criteria.", criteria);
                _log.LogInformation("FAC search uri.", _uri.ToString());

                // Create filter string
                // Use a pipe char to delimit; default commas and spaces can't be used as may be in facet values
                List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
                //list.Add(new KeyValuePair<string, string>("AttendanceMode", string.Join("|", criteria.AttendanceModes ?? new string[] { } )));
                list.Add(new KeyValuePair<string, string>("VenueAttendancePattern", string.Join("|", criteria.AttendancePatterns ?? new string[] { } )));
                //list.Add(new KeyValuePair<string, string>("DFE1619FundedField", string.Join("|", criteria.DFE1619Funded ?? new string[] { } )));
                list.Add(new KeyValuePair<string, string>("NotionalNVQLevelv2", string.Join("|", criteria.QualificationLevels ?? new string[] { })));
                //list.Add(new KeyValuePair<string, string>("StudyModesField", string.Join("|", criteria.StudyModes ?? new string[] { } )));
                string filter = string.Join(" and ", list.Where(x => !string.IsNullOrWhiteSpace(x.Value))
                                                         .Select(x => "search.in(" + x.Key + ", '" + x.Value + "', '|')"));

                // Add geo distance clause if required
                float? latitude = null;
                float? longitude = null;
                bool geoSearchRequired = (criteria.Distance.GetValueOrDefault(0) > 0 && !string.IsNullOrWhiteSpace(criteria.TownOrPostcode));
                if (geoSearchRequired) {
                    _log.LogInformation($"FAC getting lat/long for location {criteria.TownOrPostcode}");

                    SearchParameters parameters = new SearchParameters {
                        Select = new[] { "pcds", "lat", "long" },
                        SearchMode = SearchMode.All,
                        Top = 1,
                        QueryType = QueryType.Full
                    };
                    DocumentSearchResult<dynamic> results = _onspdIndex.Documents.Search<dynamic>(criteria.TownOrPostcode, parameters);
                    latitude = (float?)results?.Results?.FirstOrDefault()?.Document?.lat;
                    longitude = (float?)results?.Results?.FirstOrDefault()?.Document?.@long;

                    if (latitude.HasValue && longitude.HasValue)
                        filter += $" and geo.distance(VenueLocation, geography'POINT({longitude.Value} {latitude.Value})') le {criteria.Distance}"; //-122.121513 47.673988)') le {criteria.Distance}";
                }

                // Create a search criteria object for azure search service
                IFACSearchCriteria facCriteria = new FACSearchCriteria()
                {
                    search = $"{criteria.SubjectKeyword}*", //}* {(string.IsNullOrWhiteSpace(criteria.TownOrPostcode) ? "" : criteria.TownOrPostcode)}".Trim(),
                    searchMode = "all",
                    top = criteria.TopResults ?? _settings.DefaultTop,
                    filter = filter,
                    facets = new string[] { "NotionalNVQLevelv2", "VenueAttendancePattern", "ProviderName", "Region" },
                    count = true
                };

                // Create json ready for posting
                JsonSerializerSettings settings = new JsonSerializerSettings {
                    //ContractResolver = new FACSearchCriteriaContractResolver()
                };
                settings.Converters.Add(new StringEnumConverter() { CamelCaseText = false });
                StringContent content = new StringContent(JsonConvert.SerializeObject(facCriteria, settings), Encoding.UTF8, "application/json");

                // Do the search
                Task<HttpResponseMessage> task = _httpClient.PostAsync(_uri, content);
                //https://dfc-dev-prov-sch.search.windows.net/indexes/course/docs?api-version=2017-11-11&queryType=full&$count=true&search=VenueAddress:evergreen AND QualificationCourseTitle:biology AND bell
                //Task<HttpResponseMessage> task = _httpClient.GetAsync($"{_settings.ApiUrl}?api-version={_settings.ApiVersion}&queryType=full&$count={facCriteria.Count.ToString()}&search=VenueAddress:evergreen AND QualificationCourseTitle:biology AND {criteria.SubjectKeywordField}*");

                task.Wait();
                HttpResponseMessage response = task.Result;

                _log.LogInformation("FAC search service http response.", response);

                // Handle response and deserialize results
                if (response.IsSuccessStatusCode) {
                    //var json = await response.Content.ReadAsStringAsync();
                    var json = response.Content.ReadAsStringAsync().Result;

                    _log.LogInformation("FAC search service json response.", json);
                    settings = new JsonSerializerSettings {
                        ContractResolver = new FACSearchResultContractResolver()
                    };
                    settings.Converters.Add(new StringEnumConverter() { CamelCaseText = false });

                    FACSearchResult searchResult = JsonConvert.DeserializeObject<FACSearchResult>(json, settings);
                    //return Result.Ok<IFACSearchResult>(searchResult);

                    if (geoSearchRequired) {
                        foreach (FACSearchResultItem ri in searchResult.Value) {
                            if (ri.VenueLocation != null && ri?.VenueLocation["coordinates"][0] != 0 && ri?.VenueLocation["coordinates"][1] != 0)
                                ri.GeoSearchDistance = Math.Round(
                                    GeoHelper.DistanceTo(
                                        new GeoHelper.Coordinates() { Latitude = latitude.Value,
                                                                      Longitude = longitude.Value },
                                        new GeoHelper.Coordinates() { Latitude = ri?.VenueLocation["coordinates"][1],
                                                                      Longitude = ri?.VenueLocation["coordinates"][0] }),
                                2);
                        }
                    }

                    return searchResult;

                } else {
                    _log.LogWarning($"FAC unexpected response: {response.StatusCode}", response);
                    //return Result.Fail<IFACSearchResult>("FAC search service unsuccessfull http response.");
                    return null;
                }

            } catch (HttpRequestException hre) {
                _log.LogError("FAC search service http request error.", hre);
                //return Result.Fail<IFACSearchResult>("FAC search service http request error.");
                return null;

            } catch (Exception e) {
                _log.LogError("FAC search service unknown error.", e);
                //return Result.Fail<IFACSearchResult>("FAC search service unknown error.");
                return null;

            } finally {
                //_log.LogMethodExit();
            }


        }
    }
}
