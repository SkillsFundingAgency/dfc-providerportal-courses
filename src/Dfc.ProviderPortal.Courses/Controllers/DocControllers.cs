﻿using Dfc.ProviderPortal.Courses.Functions;
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
        public IActionResult AddCourse(Course course)
        {
            return Ok();
        }

        [Route("CourseDetail")]
        [HttpGet]
        [ProducesResponseType(typeof(AzureSearchCourseDetail), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CourseDetail(Guid CourseId, Guid RunId)
        {
            return Ok();
        }

        [Route("DeleteCoursesByUKPRN")]
        [HttpGet]
        [ProducesResponseType(typeof(DeleteCoursesByUKPRN), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteCoursesByUKPRN(int UKPRN)
        {
            return Ok();
        }
        [Route("DeleteBulkUploadCourses")]
        [HttpGet]
        [ProducesResponseType(typeof(DeleteBulkUploadCourses), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteBulkUploadCourses(int UKPRN)
        {
            return Ok();
        }
        [Route("GetCourseById")]
        [HttpGet]
        [ProducesResponseType(typeof(Course), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteCoursesByUKPRN(Guid id)
        {
            return Ok();
        }

        [Route("GetCoursesByLevelForUKPRN")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Course>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetCoursesByLevelForUKPRN(int UKPRN)
        {
            return Ok();
        }

        [Route("GetCoursesByUKPRN")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Course>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetCoursesByUKPRN(int UKPRN)
        {
            return Ok();
        }

        [Route("GetGroupedCoursesByUKPRN")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Course>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetGroupedCoursesByUKPRN(int UKPRN)
        {
            return Ok();
        }

        [Route("UpdateCourse")]
        [HttpPost]
        [ProducesResponseType(typeof(Course), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateCourse(Course course)
        {
            return Ok();
        }
    }
}