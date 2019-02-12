
using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Spatial;
using Dfc.ProviderPortal.Courses.Models;


namespace Dfc.ProviderPortal.Courses.Interfaces
{
    public interface IAzureSearchCourse
    {
        Guid? id { get; set; }
        Guid? CourseId { get; set; }
        string QualificationCourseTitle { get; set; }
        string LearnAimRef { get; set; }
        string NotionalNVQLevelv2 { get; set; }
        DateTime? UpdatedOn { get; set; }
        string VenueName { get; set; }
        string VenueAddress { get; set; }
        GeographyPoint VenueLocation { get; set; }
        string VenueAttendancePattern { get; set; }
        string ProviderName { get; set; }
        string Region { get; set; }
        int? Status { get; set; }
    }
}
