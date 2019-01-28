
using System;
using System.Collections.Generic;
using System.ComponentModel;


namespace Dfc.ProviderPortal.Courses.Interfaces
{
    public interface ICourseRun
    {
        Guid id { get; set; }
        int? CourseInstanceId { get; set; }
        Guid? VenueId { get; set; }
        string CourseName { get; set; }
        string ProviderCourseID { get; set; }
        int DeliveryMode { get; set; }
        bool FlexibleStartDate { get; set; }
        DateTime? StartDate { get; set; }
        string CourseURL { get; set; }
        decimal? Cost { get; set; }
        string CostDescription { get; set; }
        int DurationUnit { get; set; }
        int? DurationValue { get; set; }
        int StudyMode { get; set; }
        int AttendancePattern { get; set; }
        IEnumerable<string> Regions { get; set; }

        DateTime CreatedDate { get; set; }
        string CreatedBy { get; set; }
        DateTime? UpdatedDate { get; set; }
        string UpdatedBy { get; set; }
    }
}