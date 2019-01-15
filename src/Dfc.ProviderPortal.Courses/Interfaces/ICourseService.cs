using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;

namespace Dfc.ProviderPortal.Courses.Interfaces
{
    public interface ICourseService
    {
        Task<ICourse> AddCourse(ICourse course);
        Task<ICourse> GetCourseById(Guid id);
        Task<IEnumerable<ICourse>> GetCoursesByUKPRN(int UKPRN);
      

        Task<ICourse> UpdateById(ICourse doc);
    }
}
