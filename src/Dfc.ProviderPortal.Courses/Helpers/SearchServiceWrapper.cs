using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Courses.Models;
using Dfc.ProviderPortal.Courses.Settings;
using Dfc.ProviderPortal.Packages;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dfc.ProviderPortal.Courses.Helpers
{
    public class SearchServiceWrapper : ISearchServiceWrapper
    {
        public class LINQComboClass
        {
            public Course Course { get; set; }
            public CourseRun Run { get; set; }
            public SubRegionItemModel SubRegion { get; set; }
            public AzureSearchVenueModel Venue { get; set; }
        }

        //private readonly ILogger _log;
        private readonly ISearchServiceSettings _settings;
        private readonly SearchServiceClient _queryService;
        private readonly SearchServiceClient _adminService;
        private readonly ISearchIndexClient _queryIndex;
        private readonly ISearchIndexClient _adminIndex;
        private readonly ISearchIndexClient _onspdIndex;
        private readonly HttpClient _httpClient;
        private readonly Uri _uri;

        public SearchServiceWrapper(
            IOptions<SearchServiceSettings> settings)
        {
            Throw.IfNull(settings, nameof(settings));

            _settings = settings.Value;

            _queryService = new SearchServiceClient(_settings.SearchService, new SearchCredentials(_settings.QueryKey));
            _adminService = new SearchServiceClient(_settings.SearchService, new SearchCredentials(_settings.AdminKey));
            _queryIndex = _queryService?.Indexes?.GetClient(_settings.Index);
            _adminIndex = _adminService?.Indexes?.GetClient(_settings.Index);
            _onspdIndex = _queryService?.Indexes?.GetClient(_settings.onspdIndex);

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("api-key", _settings.QueryKey);
            _httpClient.DefaultRequestHeaders.Add("api-version", _settings.ApiVersion);
            _httpClient.DefaultRequestHeaders.Add("indexes", _settings.Index);
            _uri = new Uri($"{_settings.ApiUrl}?api-version={_settings.ApiVersion}");
        }

        public DocumentSearchResult<AzureSearchCourse> DeleteCoursesByPRN(ILogger _log, string UKPRN)
        {
            try
            {
                _log.LogInformation($"Removing course index decuments for PRN {UKPRN}");
                SearchParameters parms = new()
                {
                    Select = new[] { "id" },
                    Filter = $"UKPRN eq '{UKPRN}'",
                    SearchMode = SearchMode.All,
                    QueryType = QueryType.Full,
                    Top = 99999
                };
                IEnumerable<dynamic> docs = _adminIndex.Documents
                                                      .Search<dynamic>("*", parms)
                                                     ?.Results
                                                     ?.Select(x => x.Document);
                if (docs.Any())
                {
                    IndexBatch<Microsoft.Azure.Search.Models.Document> batch = IndexBatch.Delete("id", docs.Select(x => (string)x.id.ToString()));

                    _log.LogInformation($"Deleting {docs.Count()} documents from index");
                    Task<DocumentIndexResult> task = _adminIndex.Documents.IndexAsync(batch);
                    task.Wait();
                    _log.LogInformation($"Successfully deleted {docs.Count()} docs from Azure search index: course");
                    return null; //docs;
                }

            }
            catch (IndexBatchException ex)
            {
                IEnumerable<IndexingResult> failed = ex.IndexingResults.Where(r => !r.Succeeded);
                _log.LogError(ex, string.Format("Failed to index some of the documents: {0}",
                                                string.Join(", ", failed)));
                _log.LogError(ex.ToString());
                return null;

            }
            catch (Exception)
            {
                throw;
            }

            // Return empty list of failed IndexingResults
            return null;
        }

        public DocumentSearchResult<AzureSearchCourse> DeleteCoursesBeforeDate(ILogger _log, DateTime deleteBefore)
        {
            try
            {
                IEnumerable<dynamic> docs;
                do
                {
                    _log.LogInformation($"Removing course index decuments updated before {deleteBefore}");
                    SearchParameters parms = new()
                    {
                        Select = new[] { "id" },
                        Filter = $"UpdatedOn eq null or UpdatedOn lt {deleteBefore:yyyy-MM-ddTHH:mm:ssZ}",
                        SearchMode = SearchMode.All,
                        QueryType = QueryType.Full,
                        Top = 99999
                    };
                    docs = _adminIndex.Documents
                                      .Search<dynamic>("*", parms)
                                     ?.Results
                                     ?.Select(x => x.Document);
                    if (docs.Any())
                    {
                        IndexBatch<Microsoft.Azure.Search.Models.Document> batch = IndexBatch.Delete("id", docs.Select(x => (string)x.id.ToString()));

                        _log.LogInformation($"Deleting {docs.Count()} documents from index");
                        Task<DocumentIndexResult> task = _adminIndex.Documents.IndexAsync(batch);
                        task.Wait();
                        _log.LogInformation($"Successfully deleted {docs.Count()} docs from Azure search index: course");
                    }
                } while (docs.Any());
                return null; //docs;

            }
            catch (IndexBatchException ex)
            {
                IEnumerable<IndexingResult> failed = ex.IndexingResults.Where(r => !r.Succeeded);
                _log.LogError(ex, string.Format("Failed to index some of the documents: {0}",
                                                string.Join(", ", failed)));
                _log.LogError(ex.ToString());
                return null;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateCourseIndex(bool recreate)
        {
            var indexName = _settings.Index;

            if (recreate && await _adminService.Indexes.ExistsAsync(indexName))
            {
                await _adminService.Indexes.DeleteAsync(indexName);
            }

            var index = new Microsoft.Azure.Search.Models.Index()
            {
                Name = indexName,
                Fields = new List<Field>()
                {
                    new Field("id", DataType.String) { IsKey = true, IsFacetable = false, IsFilterable = false, IsSearchable = false, IsSortable = false },
                    new Field("CourseId", DataType.String) { IsFacetable = false, IsFilterable = true, IsSearchable = true, IsSortable = false },
                    new Field("CourseRunId", DataType.String) { IsFacetable = false, IsFilterable = true, IsSearchable = true, IsSortable = false },
                    new Field("QualificationCourseTitle", DataType.String) { IsFacetable = false, IsFilterable = true, IsSearchable = true, IsSortable = false },
                    new Field("LearnAimRef", DataType.String) { IsFacetable = false, IsFilterable = true, IsSearchable = true, IsSortable = false },
                    new Field("NotionalNVQLevelv2", DataType.String) { IsFacetable = true, IsFilterable = true, IsSearchable = false, IsSortable = false },
                    new Field("Status", DataType.Int32) { IsFacetable = false, IsFilterable = true, IsSearchable = false, IsSortable = false },
                    new Field("VenueName", DataType.String) { IsFacetable = false, IsFilterable = true, IsSearchable = true, IsSortable = false },
                    new Field("VenueAddress", DataType.String) { IsFacetable = false, IsFilterable = true, IsSearchable = true, IsSortable = false },
                    new Field("VenueLocation", DataType.GeographyPoint) { IsFacetable = false, IsFilterable = true, IsSearchable = false, IsSortable = true },
                    new Field("VenueAttendancePattern", DataType.String) { IsFacetable = true, IsFilterable = true, IsSearchable = true, IsSortable = false },
                    new Field("ProviderName", DataType.String) { IsFacetable = true, IsFilterable = true, IsSearchable = true, IsSortable = false },
                    new Field("Region", DataType.String) { IsFacetable = true, IsFilterable = true, IsSearchable = true, IsSortable = false },
                    new Field("Weighting", DataType.String) { IsFacetable = true, IsFilterable = true, IsSearchable = true, IsSortable = true },
                    new Field("ScoreBoost", DataType.Double) { IsFacetable = true, IsFilterable = true, IsSearchable = false, IsSortable = true },
                    new Field("UpdatedOn", DataType.DateTimeOffset) { IsFacetable = false, IsFilterable = true, IsSearchable = false, IsSortable = false },
                    new Field("VenueStudyMode", DataType.String) { IsFacetable = true, IsFilterable = true, IsSearchable = true, IsSortable = false },
                    new Field("DeliveryMode", DataType.String) { IsFacetable = true, IsFilterable = true, IsSearchable = true, IsSortable = false },
                    new Field("StartDate", DataType.DateTimeOffset) { IsFacetable = false, IsFilterable = true, IsSearchable = false, IsSortable = true },
                    new Field("VenueTown", DataType.String) { IsFacetable = true, IsFilterable = true, IsSearchable = true, IsSortable = false },
                    new Field("Cost", DataType.Int32) { IsFacetable = false, IsFilterable = true, IsSearchable = false, IsSortable = true },
                    new Field("CostDescription", DataType.String) { IsFacetable = false, IsFilterable = true, IsSearchable = false, IsSortable = false },
                    new Field("CourseText", DataType.String) { IsFacetable = false, IsFilterable = true, IsSearchable = false, IsSortable = false },
                    new Field("VenueAttendancePatternDescription", DataType.String) { IsFacetable = false, IsFilterable = true, IsSearchable = false, IsSortable = false },
                    new Field("VenueStudyModeDescription", DataType.String) { IsFacetable = false, IsFilterable = true, IsSearchable = false, IsSortable = false },
                    new Field("DeliveryModeDescription", DataType.String) { IsFacetable = false, IsFilterable = true, IsSearchable = false, IsSortable = false },
                    new Field("UKPRN", DataType.String) { IsFacetable = true, IsFilterable = true, IsSearchable = true, IsSortable = false },
                    new Field("CourseDescription", DataType.String) { IsFacetable = false, IsFilterable = true, IsSearchable = false, IsSortable = false },
                    new Field("CourseName", DataType.String) { IsFacetable = false, IsFilterable = true, IsSearchable = true, IsSortable = false },
                    new Field("FlexibleStartDate", DataType.Boolean) { IsFacetable = false, IsFilterable = true, IsSearchable = false, IsSortable = false },
                    new Field("DurationUnit", DataType.Int32) { IsFacetable = false, IsFilterable = false, IsSearchable = false, IsSortable = false },
                    new Field("DurationValue", DataType.Int32) { IsFacetable = false, IsFilterable = false, IsSearchable = false, IsSortable = false },
                    new Field("National", DataType.Boolean) { IsFacetable = false, IsFilterable = true, IsSearchable = false, IsSortable = false },
                    new Field("UpdateBatchId", DataType.String) { IsFacetable = false, IsFilterable = true, IsSearchable = false, IsSortable = false },
                },
                ScoringProfiles = new List<ScoringProfile>()
                {
                    new ScoringProfile()
                    {
                        Name = "region-boost",
                        Functions = new List<ScoringFunction>()
                        {
                            new MagnitudeScoringFunction(
                                "ScoreBoost",
                                boost: 100,
                                parameters: new MagnitudeScoringParameters()
                                {
                                    BoostingRangeStart = 1,
                                    BoostingRangeEnd = 100,
                                    ShouldBoostBeyondRangeByConstant = true
                                },
                                interpolation: ScoringFunctionInterpolation.Linear)
                        }
                    }
                }
            };

            await _adminService.Indexes.CreateOrUpdateAsync(index);
        }
    }
}
