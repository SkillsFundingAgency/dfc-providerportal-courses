using Dfc.ProviderPortal.Courses.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.ProviderPortal.Courses.Interfaces
{
    public interface ICourse
    {
        Guid id { get; }

        string QualificationCourseTitle { get; }
        string LearnAimRef { get; }
        string NotionalNVQLevelv2 { get; }
        string AwardOrgCode { get; }
        string QualificationType { get; }

        int ProviderUKPRN { get; }

        string CourseDescription { get; }
        string EntryRequirments { get; }
        string WhatYoullLearn { get; }
        string HowYoullLearn { get; }
        string WhatYoullNeed { get; }
        string HowYoullBeAssessed { get; }
        string WhereNext { get; }

        bool AdvancedLearnerLoan { get; }

        IEnumerable<CourseRun> CourseRuns { get; }
    }
}
