using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
        List<Models.Course> GetDocumentsByUKPRN(DocumentClient client, string collectionId, int UKPRN);
    }
}
