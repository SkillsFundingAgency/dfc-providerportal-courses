using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Hosting;

namespace DFC.ProviderPortal.Courses.Tests.Helpers
{
    public static class TestHelper
    {
        public static void AddEnvironmentVariables()
        {
            // Add environment variables needed to test Azure Functions here (launchSettings.json doesn't get processed by test projects)
            Environment.SetEnvironmentVariable("APPSETTING_EndpointUri", "https://something.azure.com/");
            Environment.SetEnvironmentVariable("APPSETTING_PrimaryKey", "****************************==");
            Environment.SetEnvironmentVariable("APPSETTING_DatabaseId", "*******");
            //Environment.SetEnvironmentVariable("APPSETTING_Collection", "********");
        }

        /// <summary>
        /// Read the result content stream from a Azure Function and return as IEnumerable<Model>
        /// </summary>
        /// <typeparam name="T">Model type</typeparam>
        /// <param name="task">Task associated with async calling of Azure Function</param>
        /// <returns>Content deserialized into IEnumerable<T></returns>
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

        /// <summary>
        /// Read the result content stream from a Azure Function and return as IEnumerable<Model>
        /// </summary>
        /// <typeparam name="T">Model type</typeparam>
        /// <param name="task">Task associated with async calling of Azure Function</param>
        /// <returns>Content deserialized into IEnumerable<T></returns>
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

        public static HttpRequestMessage CreateRequest(Uri uri, string json)
        {

            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = uri,
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            return request;
        }
    }
}
