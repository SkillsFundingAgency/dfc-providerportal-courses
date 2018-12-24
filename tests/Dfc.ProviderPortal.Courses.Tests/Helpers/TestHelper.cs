using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dfc.ProviderPortal.Courses.Tests.Helpers
{
    public class TestHelper
    {
        public static void AddEnvironmentVariables()
        {
            // Add environment variables needed to test Azure Functions here (launchSettings.json doesn't get processed by test projects)
            //Environment.SetEnvironmentVariable("APPSETTING_CosmosDBStorageURI", "");
            //Environment.SetEnvironmentVariable("APPSETTING_CosmosDBPrimaryKey", "");
            //Environment.SetEnvironmentVariable("APPSETTING_CosmosDBDatabase", "");
            //Environment.SetEnvironmentVariable("APPSETTING_CollectionName", "");
        }
        public static IEnumerable<T> GetAFReturnedObjects<T>(Task<HttpResponseMessage> task)
        {
            // Run the Azure Function to get the data, then get the returned StringContent holding returned data as JSON
            task.Wait();
            StringContent sc = (StringContent)task.Result.Content;

            // Read the content stream
            Task<string> task2 = sc.ReadAsStringAsync();
            task2.Wait();

            // Deserialize in an IEnumerable<T> to return
            return JsonConvert.DeserializeObject<IEnumerable<T>>(task2.Result);
        }
        public static T GetAFReturnedObject<T>(Task<HttpResponseMessage> task)
        {
            // Run the Azure Function to get the data, then get the returned StringContent holding returned data as JSON
            task.Wait();
            StringContent sc = (StringContent)task.Result.Content;

            // Read the content stream
            Task<string> task2 = sc.ReadAsStringAsync();
            task2.Wait();

            // Deserialize in an IEnumerable<T> to return
            return JsonConvert.DeserializeObject<T>(task2.Result);
        }
    }
}
