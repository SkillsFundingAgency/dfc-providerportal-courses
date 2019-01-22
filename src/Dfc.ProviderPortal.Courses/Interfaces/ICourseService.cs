
using System;
using System.Text;
using System.Threading.Tasks;

namespace Dfc.ProviderPortal.Courses.Interfaces
{
    public interface ICourseService
    {
        Task<ICourse> AddCourse(ICourse course);
        Task<ICourse> GetCourseById(Guid id);
        Task<IEnumerable<ICourse>> GetCoursesByUKPRN(int UKPRN);
        Task<ICourse> Update(ICourse doc);
        Task<IEnumerable<ICourse>> GetAllCourses(ILogger log);
        Task<IEnumerable<IAzureSearchCourse>> FindACourseAzureSearchData(ILogger log);
    }
}
