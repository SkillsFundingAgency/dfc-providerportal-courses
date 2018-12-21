using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Dfc.ProviderPortal.Courses.Models.Models.Courses;
using Dfc.ProviderPortal.Courses.Helpers;

namespace Dfc.ProviderPortal.Courses.Storage
{
    public class CourseStorage
    {
        /// <summary>
        /// CosmosDB client and collection
        /// </summary>
        static private DocumentClient docClient = StorageFactory.DocumentClient;
        static private DocumentCollection Collection = StorageFactory.DocumentCollection;

        /// <summary>
        /// Public constructor
        /// </summary>
        public CourseStorage() { }

        /// <summary>
        /// Inserts passed objects as documents into CosmosDB collection
        /// </summary>
        /// <param name="courses">Course data from SQL database</param>
        /// <param name="log">ILogger for logging info/errors</param>
        /// 
        public async Task<bool> InsertDocsAsync(IEnumerable<Course> courses, ILogger log)
        {
            // Insert documents into collection
            try
            {
                // Truncate collection first
                bool ret = await TruncateCollectionAsync(log);

                // Insert each course in turn as a document
                foreach (Course c in courses)
                {
                    // Add course doc to collection
                    //v.id = Guid.NewGuid();
                    //Task<ResourceResponse<Document>> task = docClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(SettingsHelper.Database, SettingsHelper.Collection),
                    //                                                                      v);
                    Task<ResourceResponse<Document>> task = InsertDocAsync(c, log);
                    task.Wait();
                }

            }
            catch (DocumentClientException ex)
            {
                Exception be = ex.GetBaseException();
                log.LogError($"Exception rasied at: {DateTime.Now}\n {be.Message}", ex);
                throw;
            }
            catch (Exception ex)
            {
                Exception be = ex.GetBaseException();
                log.LogError($"Exception rasied at: {DateTime.Now}\n {be.Message}", ex);
                throw;
            }
            finally
            {
            }
            return true;
        }
        /// <summary>
        /// Inserts a single Course document into the collection
        /// </summary>
        /// <param name="course">The Course to insert</param>
        /// <param name="log">ILogger for logging info/errors</param>
        public async Task<ResourceResponse<Document>> InsertDocAsync(Course course, ILogger log)
        {
            // Add Course doc to collection
            try
            {
                if (course.ID == Guid.Empty)
                    course.ID = Guid.NewGuid();
                return await docClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(SettingsHelper.Database, SettingsHelper.Collection),
                                                           course);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        async private Task<bool> TruncateCollectionAsync(ILogger log)
        {
            try
            {
                log.LogInformation("Deleting all docs from Course collection");
                IEnumerable<Document> docs = docClient.CreateDocumentQuery<Document>(Collection.SelfLink,
                                                                                     new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 })
                                                      .AsEnumerable();
                foreach (Document d in docs)
                {
                    await docClient.DeleteDocumentAsync(d.SelfLink);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }
    }
}
