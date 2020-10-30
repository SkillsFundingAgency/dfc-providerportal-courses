using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Dfc.ProviderPortal.Courses.Models;


namespace Dfc.ProviderPortal.Courses.Interfaces
{
    public interface ICosmosDbHelper
    {
        DocumentClient GetClient();
        Task<Database> CreateDatabaseIfNotExistsAsync(DocumentClient client);
        Task<DocumentCollection> CreateDocumentCollectionIfNotExistsAsync(DocumentClient client, string collectionId);
        Task<Document> CreateDocumentAsync(DocumentClient client, string collectionId, object document);
        T DocumentTo<T>(Document document);
        IEnumerable<T> DocumentsTo<T>(IEnumerable<Document> documents);
        Document GetDocumentById<T>(DocumentClient client, string collectionId, T id);
        Task<Document> GetDocumentByIdAsync<T>(DocumentClient client, string collectionId, T id);
        Task<Document> UpdateDocumentAsync(DocumentClient client, string collectionId, object document);
        Task<List<Course>> GetDocumentsByUKPRN(DocumentClient client, string collectionId, int UKPRN);
        Task<List<string>> DeleteDocumentsByUKPRN(DocumentClient client, string collectionId, int UKPRN);
        Task<List<string>> DeleteBulkUploadCourses(DocumentClient client, string collectionId, int UKPRN);
        Task<List<T>> GetDocumentsByUKPRN<T>(DocumentClient client, string collectionId, int UKPRN);
        Task<List<DfcMigrationReport>> GetAllDfcMigrationReports(DocumentClient client, string collectionId);
        Task<int> GetTotalLiveCourses(DocumentClient client, string collectionId);
        Task<int> UpdateRecordStatuses(DocumentClient client, string collectionId, string procedureName, int UKPRN, int? currentStatus, int statusToBeChangedTo, int partitionKey);

        Task DeployStoredProcedures();
        Task<int> ArchiveCoursesExceptBulkUploadReadytoGoLive(DocumentClient client, string collectionId, string procedureName, int UKPRN, int statusToBeChangedTo, int partitionKey);

    }
}
