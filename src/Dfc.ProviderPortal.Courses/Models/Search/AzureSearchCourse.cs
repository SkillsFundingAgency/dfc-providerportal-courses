﻿
using System;
using Dfc.ProviderPortal.Courses.Interfaces;
using Microsoft.Spatial;


namespace Dfc.ProviderPortal.Courses.Models
{
    public class AzureSearchCourse : IAzureSearchCourse
    {
        public Guid? id { get; set; }
        public Guid? CourseId { get; set; }
        public Guid? CourseRunId { get; set; }
        public string QualificationCourseTitle { get; set; }
        public string LearnAimRef { get; set; }
        public string NotionalNVQLevelv2 { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string VenueName { get; set; }
        public string VenueAddress { get; set; }
        public GeographyPoint VenueLocation { get; set; }
        public string VenueAttendancePattern { get; set; }
        public string ProviderName { get; set; }
        public string Region { get; set; }
        public decimal ScoreBoost { get; set; }
        public int? Status { get; set; }
    }
}
