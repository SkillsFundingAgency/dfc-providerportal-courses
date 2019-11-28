
using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Search.Models;
using Dfc.ProviderPortal.Courses.Models;
using Document = Microsoft.Azure.Documents.Document;


namespace Dfc.ProviderPortal.Courses.Interfaces
{
    public interface ISearchServiceWrapper
    {
        //IEnumerable<IndexingResult> UploadBatch(
        //    ILogger log,
        //    IEnumerable<AzureSearchProviderModel> providers,
        //    IEnumerable<AzureSearchVenueModel> venues,
        //    IReadOnlyList<Document> documents,
        //    out int succeeded);

        //Task Initialise();
        DocumentSearchResult<AzureSearchCourse> DeleteCoursesByPRN(ILogger log, string UKPRN);
        DocumentSearchResult<AzureSearchCourse> DeleteCoursesBeforeDate(ILogger _log, DateTime deleteBefore);
        Task UpdateCourseIndex(bool recreate);
    }
}
