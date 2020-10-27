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

        public async Task<List<string>> DeleteBulkUploadCourses(DocumentClient client, string collectionId, int UKPRN)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(collectionId, nameof(collectionId));
            Throw.IfNull(UKPRN, nameof(UKPRN));

            Uri uri = UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, collectionId);
            FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 };

            List<Models.Course> bulkUploadDocs = client.CreateDocumentQuery<Course>(uri, options)
                                             .Where(x => x.ProviderUKPRN == UKPRN)
                                             .Where((y => ((int)y.CourseStatus & (int)RecordStatus.BulkUploadPending) > 0 || ((int)y.CourseStatus & (int)RecordStatus.BulkUploadReadyToGoLive) > 0))
                                             .ToList();

            var responseList = new List<string>();

            foreach (var doc in bulkUploadDocs)
            {
                Uri docUri = UriFactory.CreateDocumentUri(_settings.DatabaseId, collectionId, doc.id.ToString());
                var result = await client.DeleteDocumentAsync(docUri, new RequestOptions() { PartitionKey = new PartitionKey(doc.ProviderUKPRN) });

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

        public async Task CreateStoredProcedures()
        {
            string scriptFileName = @"Data/UpdateRecordStatuses.js";
            string ArchiveCoursesSPName = @"Data/ArchiveCoursesExceptBulkUploadReadytoGoLive.js";
            string StoredProcedureName = Path.GetFileNameWithoutExtension(scriptFileName);
            string ArchiveCoursesStoredProcedureName = Path.GetFileNameWithoutExtension(ArchiveCoursesSPName);

            await UpdateRecordStatuses(GetClient(), _settings.DatabaseId, StoredProcedureName, scriptFileName);

            await ArchiveCoursesExceptBulkUploadReadytoGoLive(GetClient(), _settings.DatabaseId, ArchiveCoursesStoredProcedureName, ArchiveCoursesSPName);
        }

        public async Task UpdateRecordStatuses(DocumentClient client, string collectionId, string procedureName, string procedurePath)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(collectionId, nameof(collectionId));

            string StoredProcedureName = Path.GetFileNameWithoutExtension(procedurePath);

            var collectionLink = string.Join(@",", UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, "courses") + "/sprocs/");

            StoredProcedure isStoredProcedureExist = client.CreateStoredProcedureQuery(collectionLink)
                                   .Where(sp => sp.Id == StoredProcedureName)
                                   .AsEnumerable()
                                   .FirstOrDefault();
            try
            {
                if (isStoredProcedureExist == null)
                {
                    string sProcresult;
                    Assembly assembly = this.GetType().Assembly;
                    var resourceStream = assembly.GetManifestResourceStream(assembly.GetName().Name + "." + "Data.StoredProcedures" + ".UpdateRecordStatuses.js");
                    using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
                    {
                        sProcresult = await reader.ReadToEndAsync();
                    }

                    StoredProcedure sproc = await client.CreateStoredProcedureAsync(collectionLink, new StoredProcedure
                    {
                        Id = StoredProcedureName,
                        Body = sProcresult
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> ArchiveCoursesExceptBulkUploadReadytoGoLive(DocumentClient client, string collectionId, string procedureName, int UKPRN, int statusToBeChangedTo, int partitionKey)
        {
            RequestOptions requestOptions = new RequestOptions { PartitionKey = new PartitionKey(partitionKey), EnableScriptLogging = true };

            var response = await client.ExecuteStoredProcedureAsync<SPResponse>(UriFactory.CreateStoredProcedureUri(_settings.DatabaseId, collectionId, "ArchiveCoursesExceptBulkUploadReadytoGoLive"), requestOptions, UKPRN, statusToBeChangedTo);

            return response.Response.updated;
        }

        public async Task ArchiveCoursesExceptBulkUploadReadytoGoLive(DocumentClient client, string collectionId, string procedureName, string procedurePath)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(collectionId, nameof(collectionId));

            string StoredProcedureName = Path.GetFileNameWithoutExtension(procedurePath);

            var collectionLink = string.Join(@",", UriFactory.CreateDocumentCollectionUri(_settings.DatabaseId, "courses") + "/sprocs/");

            StoredProcedure isStoredProcedureExist = client.CreateStoredProcedureQuery(collectionLink)
                                   .Where(sp => sp.Id == StoredProcedureName)
                                   .AsEnumerable()
                                   .FirstOrDefault();

            if (isStoredProcedureExist == null)
            {
                string sProcresult;
                Assembly assembly = this.GetType().Assembly;
                var resourceStream = assembly.GetManifestResourceStream(assembly.GetName().Name + "." + "Data.StoredProcedures" + ".ArchiveCoursesExceptBulkUploadReadytoGoLive.js");
                using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
                {
                    sProcresult = await reader.ReadToEndAsync();
                }

                StoredProcedure sproc = await client.CreateStoredProcedureAsync(collectionLink, new StoredProcedure
                {
                    Id = StoredProcedureName,
                    Body = sProcresult
                });
            }
        }
    }
}