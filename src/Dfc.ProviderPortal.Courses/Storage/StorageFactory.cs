using Dfc.ProviderPortal.Courses.Helpers;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dfc.ProviderPortal.Courses.Storage
{
    public static class StorageFactory
    {
        /// <summary>
        /// CosmosDB connection created using settings
        /// </summary>
        static public DocumentClient DocumentClient = new DocumentClient(new Uri(SettingsHelper.StorageURI),
                                                                         SettingsHelper.PrimaryKey
                                                                        );
        // Find collection to query
        static public DocumentCollection DocumentCollection = GetDocumentCollectionAsync().Result;

        static public async Task<ResourceResponse<DocumentCollection>> GetDocumentCollectionAsync()
        {
            Task<ResourceResponse<DocumentCollection>> task = DocumentClient.ReadDocumentCollectionAsync(
                                                                    UriFactory.CreateDocumentCollectionUri(SettingsHelper.Database,
                                                                                                           SettingsHelper.Collection
                                                                                                         ));
            return task.Result;
        }
    }
}
