using Dfc.ProviderPortal.Courses.Models;
using System;
using System.Collections.Generic;

namespace Dfc.ProviderPortal.Courses.Interfaces
{
    public interface ICourse
    {
        Guid id { get; set; }
        int? CourseId { get; set; }
        string QualificationCourseTitle { get; set; } 
        string LearnAimRef { get; set; } 
        string NotionalNVQLevelv2 { get; set; } 
        string AwardOrgCode { get; set; } 
        string QualificationType { get; set; } 

        int ProviderUKPRN { get; set; } 

        string CourseDescription { get; set; }
        string EntryRequirments { get; set; } 
        string WhatYoullLearn { get; set; }
        string HowYoullLearn { get; set; }
        string WhatYoullNeed { get; set; }
        string HowYoullBeAssessed { get; set; }
        string WhereNext { get; set; }

        bool AdvancedLearnerLoan { get; set; }

        IEnumerable<CourseRun> CourseRuns { get; }

        DateTime CreatedDate { get; set; }
        string CreatedBy { get; set; }
        DateTime? UpdatedDate { get; set; }
        string UpdatedBy { get; set; }
    }
}