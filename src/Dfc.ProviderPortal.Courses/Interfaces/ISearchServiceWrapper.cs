
using System;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Courses.Models;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Logging;


namespace Dfc.ProviderPortal.Courses.Interfaces
{
    public interface ISearchServiceWrapper
    {
        DocumentSearchResult<AzureSearchCourse> DeleteCoursesByPRN(ILogger log, string UKPRN);
        DocumentSearchResult<AzureSearchCourse> DeleteCoursesBeforeDate(ILogger _log, DateTime deleteBefore);
        Task UpdateCourseIndex(bool recreate);
    }
}
