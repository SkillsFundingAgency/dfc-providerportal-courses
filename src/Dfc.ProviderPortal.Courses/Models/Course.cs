﻿
using System;
using System.Text;
using System.Collections.Generic;
using Dfc.ProviderPortal.Courses.Interfaces;


namespace Dfc.ProviderPortal.Courses.Models
{
    public class Course : ICourse 
    {
        public Guid id { get; set; }
        public int? CourseId { get; set; }
        public string QualificationCourseTitle { get; set; }
        public string LearnAimRef { get; set; }
        public string NotionalNVQLevelv2 { get; set; }
        public string AwardOrgCode { get; set; }
        public string QualificationType { get; set; }
        public int ProviderUKPRN { get; set; }
        public string CourseDescription { get; set; }
        public string EntryRequirments { get; set; }
        public string WhatYoullLearn { get; set; }
        public string HowYoullLearn { get; set; }
        public string WhatYoullNeed { get; set; }
        public string HowYoullBeAssessed { get; set; }
        public string WhereNext { get; set; }
        public bool AdvancedLearnerLoan { get; set; }
      
        public IEnumerable<CourseRun> CourseRuns { get; set; }

        public RecordStatus RecordStatus { get; set; }

        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}