
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using Dfc.ProviderPortal.Courses.Models;
using Dfc.ProviderPortal.Courses.Services;
using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Packages;
using Dfc.ProviderPortal.Packages.AzureFunctions.DependencyInjection;


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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        public CoursesController(ILogger<CoursesController> logger, ICourseService service)
        {
            Throw.IfNull<ILogger<CoursesController>>(logger, nameof(logger));
            Throw.IfNull<ICourseService>(service, nameof(service));

            _log = logger;
            _service = service;
        }

        /// <summary>
        /// All courses, for example:
        /// GET api/courses
        /// </summary>
        /// <returns>All courses</returns>
        [HttpGet("PopulateSearch", Name = "PopulateSearch")]
        public ActionResult<IEnumerable<AzureSearchCourse>> PopulateSearch()
        {
            try {
                Task<IEnumerable<AzureSearchCourse>> task = _service.FindACourseAzureSearchData(_log);
                task.Wait();
                //IEnumerable<IAzureSearchCourse> results = (IEnumerable<IAzureSearchCourse>)task.Result;
                return new ActionResult<IEnumerable<AzureSearchCourse>>(task.Result);
            }
            catch (Exception ex) {
                throw ex;
            }
        }
    }
}
