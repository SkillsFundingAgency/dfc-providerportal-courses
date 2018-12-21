using Dfc.ProviderPortal.Courses.Models.Models.Qualifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.ProviderPortal.Courses.Models.Models.Courses
{
    public class Course
    {
        public Guid ID { get; set; }
        public QuAP QuAP { get; set; }
        public IEnumerable<CourseRun> CourseRun { get; set; }
    }
}
