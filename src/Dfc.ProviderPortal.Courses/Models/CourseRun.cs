
using System;
using System.Text;
using System.Collections.Generic;
using Dfc.ProviderPortal.Courses.Interfaces;


namespace Dfc.ProviderPortal.Courses.Models
{
    public class CourseRun : ICourseRun
    {
        public Guid id { get; set; }
        public int? CourseInstanceId { get; set; }
        public Guid? VenueId { get; set; }
        public string CourseName { get; set; }
        public string ProviderCourseID { get; set; }
        public int DeliveryMode { get; set; }
        public bool FlexibleStartDate { get; set; }
        public DateTime? StartDate { get; set; }
        public string CourseURL { get; set; }
        public decimal? Cost { get; set; }
        public string CostDescription { get; set; }
        public int DurationUnit { get; set; }
        public int? DurationValue { get; set; }
        public int StudyMode { get; set; }
        public int AttendancePattern { get; set; }

        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
