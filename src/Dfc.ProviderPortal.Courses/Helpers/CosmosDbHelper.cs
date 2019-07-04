
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Options;
using Dfc.ProviderPortal.Packages;
using Dfc.ProviderPortal.Courses.Models;
using Dfc.ProviderPortal.Courses.Settings;
using Dfc.ProviderPortal.Courses.Interfaces;
using System.Net;

namespace Dfc.ProviderPortal.Courses.Helpers
{
    public class CosmosDbHelper : ICosmosDbHelper
    {
        private readonly ICosmosDbSettings _settings;

        public CosmosDbHelper(IOptions<CosmosDbSettings> settings)
        {
            Throw.IfNull(settings, nameof(settings));

            _settings = settings.Value;
        }

        public async Task<Database> CreateDatabaseIfNotExistsAsync(DocumentClient client)
        {
            Throw.IfNull(client, nameof(client));

            var db = new Database { Id = _settings.DatabaseId };

            return await client.CreateDatabaseIfNotExistsAsync(db);
        }

        public async Task<Document> CreateDocumentAsync(
            DocumentClient client, 
            string collectionId, 
            object document)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(collectionId, nameof(collectionId));
            Throw.IfNull(document, nameof(document));

            var uri = UriFactory.CreateDocumentCollectionUri(
                _settings.DatabaseId,
                collectionId);

            return await client.CreateDocumentAsync(uri, document);
        }

        public async Task<DocumentCollection> CreateDocumentCollectionIfNotExistsAsync(
            DocumentClient client, 
            string collectionId)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(collectionId, nameof(collectionId));

            var uri = UriFactory.CreateDatabaseUri(_settings.DatabaseId);
            var coll = new DocumentCollection { Id = collectionId };

            return await client.CreateDocumentCollectionIfNotExistsAsync(uri, coll);
        }

        public T DocumentTo<T>(Document document)
        {
            Throw.IfNull(document, nameof(document));
            return (T)(dynamic)document;
        }

        public IEnumerable<T> DocumentsTo<T>(IEnumerable<Document> documents)
        {
            Throw.IfNull(documents, nameof(documents));
            return (IEnumerable<T>)(IEnumerable<dynamic>)documents;
        }

        public DocumentClient GetClient()
        {
            return new DocumentClient(new Uri(_settings.EndpointUri), _settings.PrimaryKey);
        }

        public Document GetDocumentById<T>(DocumentClient client, string collectionId, T id)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(collectionId, nameof(collectionId));
            Throw.IfNull(id, nameof(id));

            var uri = UriFactory.CreateDocumentCollectionUri(
                _settings.DatabaseId,
                collectionId);

            var options = new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 };

            var doc = client.CreateDocumentQuery(uri, options)
                .Where(x => x.Id == id.ToString())
                .AsEnumerable()
                .FirstOrDefault();

            return doc;
        }


        public async Task<Document> UpdateDocumentAsync(
            DocumentClient client,
            string collectionId,
            object document)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(collectionId, nameof(collectionId));
            Throw.IfNull(document, nameof(document));

            var uri = UriFactory.CreateDocumentCollectionUri(
                _settings.DatabaseId,
                collectionId);

            return await client.UpsertDocumentAsync(uri, document);
        }

        public List<Models.Course> GetDocumentsByUKPRN(DocumentClient client, string collectionId, int UKPRN)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(collectionId, nameof(collectionId));
            Throw.IfNull(UKPRN, nameof(UKPRN));

            Uri uri = UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, collectionId);
            FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 };

            List<Models.Course> docs = client.CreateDocumentQuery<Course>(uri, options)
                                             .Where(x => x.ProviderUKPRN == UKPRN)
                                             .ToList(); // .AsEnumerable();

            return docs;
        }

        public IList<T> GetDocumentsByUKPRN<T>(DocumentClient client, string collectionId, int UKPRN)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(collectionId, nameof(collectionId));
            Throw.IfNull(UKPRN, nameof(UKPRN));

            Uri uri = UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, collectionId);
            FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 };

            
            var docs = client.CreateDocumentQuery<T>(uri, $"SELECT * FROM c WHERE c.ProviderUKPRN = {UKPRN}");

            return docs == null ? new List<T>() : docs.ToList();
        }

        public async Task<List<string>> DeleteDocumentsByUKPRN(DocumentClient client, string collectionId, int UKPRN)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(collectionId, nameof(collectionId));
            Throw.IfNull(UKPRN, nameof(UKPRN));

            Uri uri = UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, collectionId);
            FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 };

            List<Models.Course> docs = client.CreateDocumentQuery<Course>(uri, options)
                                             .Where(x => x.ProviderUKPRN == UKPRN)
                                             .ToList(); 

            var responseList = new List<string>();

            foreach (var doc in docs)
            {
                Uri docUri = UriFactory.CreateDocumentUri(_settings.DatabaseId, collectionId, doc.id.ToString());
                var result = await client.DeleteDocumentAsync(docUri);

                if(result.StatusCode == HttpStatusCode.NoContent)
                {
                    responseList.Add($"Course with LARS ( { doc.LearnAimRef } ) and Title ( { doc.QualificationCourseTitle } ) was deleted.");
                }
                else
                {
                    responseList.Add($"Course with LARS ( { doc.LearnAimRef } ) and Title ( { doc.QualificationCourseTitle } ) wasn't deleted. StatusCode: ( { result.StatusCode } )");
                }
                
            }

            return responseList;
        }

        public async Task<List<string>> DeleteBulkUploadCourses(DocumentClient client, string collectionId, int UKPRN)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(collectionId, nameof(collectionId));
            Throw.IfNull(UKPRN, nameof(UKPRN));

            Uri uri = UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, collectionId);
            FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 };

            List<Models.Course> bulkUploadDocs = client.CreateDocumentQuery<Course>(uri, options)
                                             .Where(x => x.ProviderUKPRN == UKPRN )
                                             .Where((y => ((int)y.CourseStatus & (int)RecordStatus.BulkUloadPending) > 0 || ((int)y.CourseStatus & (int)RecordStatus.BulkUploadReadyToGoLive) > 0))
                                             .ToList();

            var responseList = new List<string>();

            foreach (var doc in bulkUploadDocs)
            {
                Uri docUri = UriFactory.CreateDocumentUri(_settings.DatabaseId, collectionId, doc.id.ToString());
                var result = await client.DeleteDocumentAsync(docUri);

                if (result.StatusCode == HttpStatusCode.NoContent)
                {
                    responseList.Add($"Course with LARS ( { doc.LearnAimRef } ) and Title ( { doc.QualificationCourseTitle } ) was deleted.");
                }
                else
                {
                    responseList.Add($"Course with LARS ( { doc.LearnAimRef } ) and Title ( { doc.QualificationCourseTitle } ) wasn't deleted. StatusCode: ( { result.StatusCode } )");
                }

            }

            return responseList;
        }

    }
}
