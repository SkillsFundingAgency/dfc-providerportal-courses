﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.ProviderPortal.Courses.Models
{
    public class CourseMigrationReport
    {
        public Guid Id { get; set; }
        public IList<Course> LarslessCourses { get; set; }
        public int PreviousLiveCourseCount { get; set; }
        public int ProviderUKPRN { get; set; }
    }
}
