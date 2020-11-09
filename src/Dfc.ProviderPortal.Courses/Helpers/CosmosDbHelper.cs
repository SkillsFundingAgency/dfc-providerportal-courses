using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Courses.Models;
using Dfc.ProviderPortal.Courses.Settings;
using Dfc.ProviderPortal.Packages;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<List<Course>> GetDocumentsByUKPRN(DocumentClient client, string collectionId, int UKPRN)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(collectionId, nameof(collectionId));
            Throw.IfNull(UKPRN, nameof(UKPRN));

            Uri uri = UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, collectionId);
            FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 };

            return await client.CreateDocumentQuery<Course>(uri, options)
                .Where(x => x.ProviderUKPRN == UKPRN)
                .ToListAsync();
        }

        public async Task<List<T>> GetDocumentsByUKPRN<T>(DocumentClient client, string collectionId, int UKPRN)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(collectionId, nameof(collectionId));
            Throw.IfNull(UKPRN, nameof(UKPRN));

            Uri uri = UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, collectionId);
            FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 };

            var docs = client.CreateDocumentQuery<T>(uri, $"SELECT * FROM c WHERE c.ProviderUKPRN = {UKPRN}");

            return docs == null ? new List<T>() : await docs.ToListAsync();
        }

        public async Task<List<DfcMigrationReport>> GetAllDfcMigrationReports(DocumentClient client, string collectionId)
        {
            var reports = new List<DfcMigrationReport>();

            Uri uri = UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, collectionId);
            FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 };

            using (var queryable = client.CreateDocumentQuery<DfcMigrationReport>(uri, options).AsDocumentQuery())
            {
                while (queryable.HasMoreResults)
                {
                    foreach (DfcMigrationReport report in await queryable.ExecuteNextAsync<DfcMigrationReport>())
                    {
                        //Some Providers have ',' in there name which is breaking the CSV
                        report.ProviderName = report.ProviderName.Replace(",", "");
                        reports.Add(report);
                    }
                }
            }

            return reports;
        }

        public async Task<int> GetTotalLiveCourses(DocumentClient client, string collectionId)
        {
            Uri uri = UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, collectionId);
            FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 };

            return await client.CreateDocumentQuery<Course>(uri, options)
                .SelectMany(c => c.CourseRuns)
                .Where(cr => cr.RecordStatus == RecordStatus.Live)
                .CountAsync();
        }

        public async Task<Document> GetDocumentByIdAsync<T>(DocumentClient client, string collectionId, T id)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(collectionId, nameof(collectionId));
            Throw.IfNull(id, nameof(id));

            var uri = UriFactory.CreateDocumentCollectionUri(
                _settings.DatabaseId,
                collectionId);

            var options = new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 };

            var query = client.CreateDocumentQuery(uri, options)
                .Where(x => x.Id == id.ToString())
                .AsDocumentQuery();

            var doc = (await query.ExecuteNextAsync()).FirstOrDefault();

            return doc;
        }

        public async Task<int> UpdateRecordStatuses(DocumentClient client, string collectionId, string procedureName, int UKPRN, int? currentStatus, int statusToBeChangedTo, int partitionKey)
        {
            RequestOptions requestOptions = new RequestOptions { PartitionKey = new PartitionKey(partitionKey), EnableScriptLogging = true };

            var response = await client.ExecuteStoredProcedureAsync<SPResponse>(UriFactory.CreateStoredProcedureUri(_settings.DatabaseId, collectionId, "UpdateRecordStatuses"), requestOptions, UKPRN, currentStatus, statusToBeChangedTo);

            return response.Response.updated;
        }

        public async Task<int> ArchiveCoursesExceptBulkUploadReadytoGoLive(DocumentClient client, string collectionId, string procedureName, int UKPRN, int statusToBeChangedTo, int partitionKey)
        {
            RequestOptions requestOptions = new RequestOptions { PartitionKey = new PartitionKey(partitionKey), EnableScriptLogging = true };

            var response = await client.ExecuteStoredProcedureAsync<SPResponse>(UriFactory.CreateStoredProcedureUri(_settings.DatabaseId, collectionId, "ArchiveCoursesExceptBulkUploadReadytoGoLive"), requestOptions, UKPRN, statusToBeChangedTo);

            return response.Response.updated;
        }

        public async Task DeployStoredProcedures()
        {
            using (var client = GetClient())
            {
                await DeployStoredProcedureToCollection("courses", "UpdateRecordStatuses");
                await DeployStoredProcedureToCollection("courses", "ArchiveCoursesExceptBulkUploadReadytoGoLive");

                async Task DeployStoredProcedureToCollection(string collection, string storedProcedureName)
                {
                    var scriptFilePath = $"Dfc.ProviderPortal.Courses.Data.StoredProcedures.{storedProcedureName}.js";

                    using (var stream = typeof(CosmosDbHelper).Assembly.GetManifestResourceStream(scriptFilePath))
                    {
                        if (stream == null)
                        {
                            throw new ArgumentException(
                                $"Cannot find stored procedure '{scriptFilePath}'.",
                                nameof(storedProcedureName));
                        }

                        string script;
                        using (var reader = new StreamReader(stream))
                        {
                            script = reader.ReadToEnd();
                        }

                        var storedProcId = storedProcedureName;
                        var collectionUri = UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, collection);

                        var storedProcedure = new StoredProcedure()
                        {
                            Body = script,
                            Id = storedProcId
                        };

                        try
                        {
                            await client.CreateStoredProcedureAsync(collectionUri, storedProcedure);
                        }
                        catch (DocumentClientException dex) when (dex.StatusCode == System.Net.HttpStatusCode.Conflict)
                        {
                            // Already exists - replace it

                            var sprocUri = UriFactory.CreateStoredProcedureUri(_settings.DatabaseId, collection, storedProcId);
                            await client.ReplaceStoredProcedureAsync(sprocUri, storedProcedure);
                        }
                    }
                }
            }
        }
    }
}
