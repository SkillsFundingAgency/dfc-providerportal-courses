using Dfc.ProviderPortal.Courses.Models.Enums;
using Dfc.ProviderPortal.Courses.Models.Models.Providers;
using Dfc.ProviderPortal.Courses.Models.Models.Qualifications;
using Dfc.ProviderPortal.Courses.Models.Models.Venues;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.ProviderPortal.Courses.Models.Models.Courses
{
    public class CourseRun
    {
        string CourseDescription { get; }
        string EntryRequirments { get; } //Requirements { get; }
        string WhatYoullLearn { get; }
        string HowYoullLearn { get; }
        string WhatYoullNeed { get; }
        string WhatYoullNeedToBring { get; }
        string HowYoullBeAssessed { get; }
        string WhereNext { get; }
        string CourseName { get; }
        string ProviderCourseID { get; } //string CourseID { get; }
        string DeliveryMode { get; }
        bool FlexibleStartDate { get; }
        DateTime StartDate { get; }
        string CourseURL { get; }
        decimal Cost { get; } //string Price { get; }
        string CostDescription { get; }
        bool AdvancedLearnerLoan { get; }
        DurationUnit DurationUnit { get; }
        int DurationValue { get; }
        StudyMode StudyMode { get; } //string StudyMode { get; }
        AttendancePattern AttendancePattern { get; } ////string Attendance { get; }
        //string Pattern { get; }
        Venue Venue { get; }
        Provider Provider { get; }
        Qualification Qualification { get; }
        DateTime CreatedDate { get; }
        string CreatedBy { get; }
        DateTime UpdatedDate { get; }
        string UpdatedBy { get; }
    }
}
