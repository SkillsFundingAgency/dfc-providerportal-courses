using Dfc.ProviderPortal.Courses.Functions;
using Dfc.ProviderPortal.Courses.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dfc.ProviderPortal.Courses.Controllers
{
    [Produces("application/json")]
    [Route("api")]
    [ApiController]
    public class DocControllers : ControllerBase
    {
        [Route("AddCourse")]
        [HttpPost]
        [ProducesResponseType(typeof(Course), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AddCourse(Course course, [Required]string code)
        {
            return Ok();
        }

        [Route("CourseDetail")]
        [HttpGet]
        [ProducesResponseType(typeof(AzureSearchCourseDetail), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CourseDetail(Guid CourseId, Guid RunId, [Required]string code)
        {
            return Ok();
        }

        [Route("DeleteCoursesByUKPRN")]
        [HttpGet]
        [ProducesResponseType(typeof(DeleteCoursesByUKPRN), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteCoursesByUKPRN(int UKPRN, [Required]string code)
        {
            return Ok();
        }
        [Route("DeleteBulkUploadCourses")]
        [HttpGet]
        [ProducesResponseType(typeof(DeleteBulkUploadCourses), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteBulkUploadCourses(int UKPRN, [Required]string code)
        {
            return Ok();
        }
        [Route("GetCourseById")]
        [HttpGet]
        [ProducesResponseType(typeof(Course), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteCoursesByUKPRN(Guid id, [Required]string code)
        {
            return Ok();
        }

        [Route("GetCoursesByLevelForUKPRN")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Course>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetCoursesByLevelForUKPRN(int UKPRN, [Required]string code)
        {
            return Ok();
        }

        [Route("GetCoursesByUKPRN")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Course>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetCoursesByUKPRN(int UKPRN, [Required]string code)
        {
            return Ok();
        }

        [Route("GetGroupedCoursesByUKPRN")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Course>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetGroupedCoursesByUKPRN(int UKPRN, [Required]string code)
        {
            return Ok();
        }

        [Route("UpdateCourse")]
        [HttpPost]
        [ProducesResponseType(typeof(Course), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateCourse(Course course, [Required]string code)
        {
            return Ok();
        }

        [Route("UpdateCourseMigrationReport")]
        [HttpPost]
        [ProducesResponseType(typeof(Course), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateCourseMigrationReport(CourseMigrationReport courseMigrationReport, [Required]string code)
        {
            return Ok();
        }

        [Route("GetCourseMigrationReportByUKPRN")]
        [HttpGet]
        [ProducesResponseType(typeof(Course), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetCourseMigrationReportByUKPRN(int UKPRN, [Required]string code)
        {
            return Ok();
        }
    }
}