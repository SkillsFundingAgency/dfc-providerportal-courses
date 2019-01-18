
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using Dfc.ProviderPortal.Courses.Models;
using Dfc.ProviderPortal.Courses.Services;
using Dfc.ProviderPortal.Packages.AzureFunctions.DependencyInjection;
using Dfc.ProviderPortal.Courses.Interfaces;


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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        public CoursesController(ILogger<CoursesController> logger)
        {
            _log = logger;
        }

        /// <summary>
        /// All courses, for example:
        /// GET api/courses
        /// </summary>
        /// <returns>All courses</returns>
        [HttpGet(Name = "GetAllCourses")]
        public ActionResult<IEnumerable<ICourse>> Get() //[Inject] ICourseService service)
        {
            return new List<Course>();
            //Task<IEnumerable<ICourse>> task = service.GetAllCourses(_log);
            //task.Wait();
            //return new ActionResult<IEnumerable<ICourse>>(task.Result);
        }
    }
}
