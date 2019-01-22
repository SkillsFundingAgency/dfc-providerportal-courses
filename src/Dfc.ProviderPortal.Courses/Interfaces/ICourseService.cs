
using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Dfc.ProviderPortal.Courses.Models;


namespace Dfc.ProviderPortal.Courses.Interfaces
{
    public interface ICourseService
    {
        Task<ICourse> AddCourse(ICourse course);
        Task<ICourse> GetCourseById(Guid id);
        Task<IEnumerable<ICourse>> GetCoursesByUKPRN(int UKPRN);
        Task<IEnumerable<ICourse>> GetAllCourses(ILogger log);
        //Task<IEnumerable<ICourse>> FindACourse(ILogger log, IFACSearchCriteria criteria);
        Task<IEnumerable<AzureSearchCourse>> FindACourseAzureSearchData(ILogger log);
    }
}
