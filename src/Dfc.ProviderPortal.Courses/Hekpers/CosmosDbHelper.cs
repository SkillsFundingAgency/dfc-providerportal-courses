﻿using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Courses.Settings;
using Dfc.ProviderPortal.Packages;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dfc.ProviderPortal.Courses.Hekpers
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

        public DocumentClient GetClient()
        {
            return new DocumentClient(new Uri(_settings.EndpointUri), _settings.PrimaryKey);
        }

        public Document GetDocumentById<T>(DocumentClient client, string collectionId, T id)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(collectionId, nameof(collectionId));
            Throw.IfNull(id, nameof(id));

            var uri = UriFactory.CreateCollectionUri(
                _settings.DatabaseId,
                collectionId);

            var doc = client.CreateDocumentQuery(uri, new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 })
                .Where(x => x.Id == id.ToString())
                .AsEnumerable()
                .FirstOrDefault();

            return doc;
        }
    }
}
