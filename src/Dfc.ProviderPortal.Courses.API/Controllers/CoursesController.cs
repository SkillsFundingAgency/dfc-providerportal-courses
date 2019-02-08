
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Dfc.ProviderPortal.Courses.Models;
using Dfc.ProviderPortal.Courses.Services;
using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Packages;
using Dfc.ProviderPortal.Packages.AzureFunctions.DependencyInjection;
using Dfc.ProviderPortal.Courses.Functions;
using Dfc.ProviderPortal.Courses.Settings;


namespace Dfc.ProviderPortal.Venues.API.Controllers
{
    /// <summary>
    /// Controller class for Courses API
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private ILogger _log = null;
        private ICourseService _service = null;
        private readonly ICosmosDbHelper _cosmosDbHelper;
        private readonly ICosmosDbCollectionSettings _settings;
        private readonly IProviderServiceSettings _providerServiceSettings;
        private readonly IVenueServiceSettings _venueServiceSettings;
        private readonly ISearchServiceSettings _searchServiceSettings;
        //private readonly ISearchServiceWrapper _searchServiceWrapper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        public CoursesController(
            ILogger<CoursesController> logger,
            ICourseService service,
            ICosmosDbHelper cosmosDbHelper,
            //ISearchServiceWrapper searchServiceWrapper,
            IOptions<ProviderServiceSettings> providerServiceSettings,
            IOptions<VenueServiceSettings> venueServiceSettings,
            IOptions<SearchServiceSettings> searchServiceSettings,
            IOptions<CosmosDbCollectionSettings> settings)
        {
            Throw.IfNull<ILogger<CoursesController>>(logger, nameof(logger));
            Throw.IfNull<ICourseService>(service, nameof(service));
            Throw.IfNull(cosmosDbHelper, nameof(cosmosDbHelper));
            //Throw.IfNull(searchServiceWrapper, nameof(searchServiceWrapper));
            Throw.IfNull(settings, nameof(settings));
            Throw.IfNull(providerServiceSettings, nameof(providerServiceSettings));
            Throw.IfNull(venueServiceSettings, nameof(venueServiceSettings));
            Throw.IfNull(searchServiceSettings, nameof(searchServiceSettings));

            _log = logger;
            _service = service;
            _cosmosDbHelper = cosmosDbHelper;
            _settings = settings.Value;
            _providerServiceSettings = providerServiceSettings.Value;
            _venueServiceSettings = venueServiceSettings.Value;
            _searchServiceSettings = searchServiceSettings.Value;
            //_searchServiceWrapper = searchServiceWrapper;
        }

        ///// <summary>
        ///// All courses, for example:
        ///// GET api/courses/PopulateSearch
        ///// </summary>
        ///// <returns>All courses</returns>
        //[HttpGet("PopulateSearch", Name = "PopulateSearch")]
        //public ActionResult<IEnumerable<IAzureSearchCourse>> PopulateSearch()
        //{
        //    try {
        //        Task<IEnumerable<IAzureSearchCourse>> task = _service.FindACourseAzureSearchData(_log);
        //        task.Wait();
        //        return new ActionResult<IEnumerable<IAzureSearchCourse>>(task.Result);

        //    } catch (Exception ex) {
        //        return new InternalServerErrorObjectResult(ex);
        //    }
        //}

        /// <summary>
        /// Search courses (aka Find A Course), for example:
        /// POST api/courses/coursesearch
        /// </summary>
        /// <returns>All courses</returns>
        //[HttpGet("CourseSearch", Name = "CourseSearch")]
        [Route("~/api/coursesearch")]
        //[HttpPost] //(Name = "CourseSearch")]
        public async Task<FACSearchResult> Post([FromBody]string q) //SearchCriteriaStructure criteria) //string q, int? TopResults)
        {
            try {
                Task<FACSearchResult> task = _service.CourseSearch(_log,
                    //criteria);
                    new SearchCriteriaStructure()
                    {
                        SubjectKeywordField = "biol", //criteria.SubjectKeywordField, // q,
                        TownOrPostcode = "Carlisle",
                        QualificationLevelsField = new int[] { 1, 5 },
                        AttendanceModesField = new int[] { },
                        AttendancePatternsField = new int[] { },
                        DFE1619FundedField = "",
                        StudyModesField = new int[] { },
                        DistanceField = 50,
                        TopResults = 100 // TopResults
                    });
                //task.Wait();
                //return new ActionResult<IEnumerable<AzureSearchCourse>>(
                //    task.Result.Results
                //               .AsEnumerable()
                //               .Select(r => r.Document)
                //);

                return await task;

            } catch (Exception ex) {
                //return new InternalServerErrorObjectResult(ex);
                _log.LogError(ex, "Error in CourseSearch");
                return null;
            }
        }

        [HttpGet("test", Name = "test")]
        public async Task<FACSearchResult> test(string q)
        {
            try {
                Task<FACSearchResult> task = _service.CourseSearch(_log,
                    //criteria);
                    new SearchCriteriaStructure()
                    {
                        SubjectKeywordField = "biol", //criteria.SubjectKeywordField, // q,
                        TownOrPostcode = "Carlisle",
                        QualificationLevelsField = new int[] { 1,2,3,4,5 },
                        AttendanceModesField = new int[] { 1,2,3 },
                        AttendancePatternsField = new int[] { 2,3,4 },
                        DFE1619FundedField = "",
                        StudyModesField = new int[ ] { 3,4,5 },
                        DistanceField = 50,
                        TopResults = 100 // TopResults
                    });

                return await task;

            } catch (Exception ex) {
                //return new InternalServerErrorObjectResult(ex);
                _log.LogError(ex, "Error in CourseSearch");
                return null;
            }

        }
    }

}
