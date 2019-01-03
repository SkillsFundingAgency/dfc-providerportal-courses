using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.ProviderPortal.Courses.Interfaces
{
    public interface ICourse
    {
        Guid id { get; }
        string Testy { get; }
        //IEnumerable<ICourseRun> CourseRuns { get; }
    }
}
