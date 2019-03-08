
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Logging;
using Dfc.ProviderPortal.Courses.Models;
using Document = Microsoft.Azure.Documents.Document;
using System.Net.Http;

namespace Dfc.ProviderPortal.Courses.Interfaces
{
    public interface ICourseService
    {
        Task<ICourse> AddCourse(ICourse course);
        Task<ICourse> GetCourseById(Guid id);
        Task<AzureSearchCourseDetail> GetCourseSearchDataById(Guid CourseId, Guid RunId);
        Task<IEnumerable<ICourse>> GetCoursesByUKPRN(int UKPRN);
        Task<List<string>> DeleteCoursesByUKPRN(int UKPRN);
        Task<ICourse> Update(ICourse doc);
        Task<IEnumerable<ICourse>> GetAllCourses(ILogger log);
        //Task<IEnumerable<IAzureSearchCourse>> FindACourseAzureSearchData(ILogger log);
        Task<IEnumerable<IndexingResult>> UploadCoursesToSearch(ILogger log, IReadOnlyList<Document> documents);
        Task<FACSearchResult> CourseSearch(ILogger log, SearchCriteriaStructure criteria); // string SearchText)
        Task<HttpResponseMessage> ArchiveProvidersLiveCourses(int UKPRN, int UIMode);
        Task<ICourseAudit> Audit(ILogger log, Document doc);
        Task<HttpResponseMessage> UpdateStatus(Guid courseId, Guid courseRunId, int currentStatus, int statusUpdate);
    }
}
