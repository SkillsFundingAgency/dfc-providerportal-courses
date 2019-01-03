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
        Document GetDocumentById<T>(DocumentClient client, string collectionId, T id);
    }
}
