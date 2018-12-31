using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.ProviderPortal.Courses.Interfaces
{
    public interface ICourse
    {
        Guid Id { get; }
        IEnumerable<ICourseRun> Items { get; }
    }
}
