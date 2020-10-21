using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Courses.Models;
using Microsoft.Extensions.Logging;

namespace Dfc.ProviderPortal.Courses.Interfaces
{
    public interface ICourseService
    {
        Task<ICourse> AddCourse(ICourse course);
        Task<ICourse> GetCourseById(Guid id);
        Task<AzureSearchCourseDetail> GetCourseSearchDataById(Guid CourseId, Guid RunId);
        Task<IEnumerable<ICourse>> GetCoursesByUKPRN(int UKPRN);
        Task<List<string>> DeleteCoursesByUKPRN(int UKPRN);
        Task<List<string>> DeleteBulkUploadCourses(int UKPRN);
        Task<ICourse> Update(ICourse doc);
        Task<IEnumerable<ICourse>> GetAllCourses(ILogger log);
        Task<HttpResponseMessage> ArchiveProvidersLiveCourses(int UKPRN, int UIMode);
        Task<HttpResponseMessage> ChangeCourseRunStatusesForUKPRNSelection(int UKPRN, RecordStatus CurrentStatus, RecordStatus StatusToBeChangedTo);
        Task<HttpResponseMessage> ArchiveCourseRunsByUKPRN(int UKPRN);
        Task<HttpResponseMessage> ChangeAllCourseRunStatusesForUKPRNSelection(int UKPRN, RecordStatus StatusToBeChangedTo);
        Task<HttpResponseMessage> UpdateStatus(Guid courseId, Guid courseRunId, int status);
        Task<int> GetTotalLiveCourses();
        Task<HttpResponseMessage> ArchiveCoursesExceptBulkUploadReadytoGoLive(int UKPRN, RecordStatus StatusToBeChangedTo);
    }
}
