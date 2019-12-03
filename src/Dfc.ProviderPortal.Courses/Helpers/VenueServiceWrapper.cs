
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Dfc.ProviderPortal.Packages;
using Dfc.ProviderPortal.Courses.Models;
using Dfc.ProviderPortal.Courses.Settings;
using Dfc.ProviderPortal.Courses.Interfaces;


namespace Dfc.ProviderPortal.Courses.Helpers
{
    public class VenueServiceWrapper : IVenueServiceWrapper
    {
        private readonly IVenueServiceSettings _settings;

        public VenueServiceWrapper(IVenueServiceSettings settings)
        {
            Throw.IfNull(settings, nameof(settings));
            _settings = settings;
        }

        public T GetById<T>(Guid id)
        {
            // Call service to get data
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _settings.ApiKey);
            var criteria = new { id };
            StringContent content = new StringContent(JsonConvert.SerializeObject(criteria), Encoding.UTF8, "application/json");
            Task<HttpResponseMessage> taskResponse = client.PostAsync($"{_settings.ApiUrl}GetVenueById?code={_settings.ApiKey}", content);
            taskResponse.Wait();
            Task<string> taskJSON = taskResponse.Result.Content.ReadAsStringAsync();
            taskJSON.Wait();
            string json = taskJSON.Result;

            // Return data as model object
            return JsonConvert.DeserializeObject<T>(json);
        }

        public IEnumerable<dynamic> GetVenuesByPRN(int prn)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _settings.ApiKey);

                var response = httpClient.GetAsync($"{_settings.ApiUrl}GetVenuesByPRN?prn={prn}").Result;
                response.EnsureSuccessStatusCode();

                var json = response.Content.ReadAsStringAsync().Result;

                return JsonConvert.DeserializeObject<IEnumerable<dynamic>>(json);
            }
        }

        public IEnumerable<AzureSearchVenueModel> GetVenues()
        {
            // Call service to get data
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _settings.ApiKey);
            var criteria = new object();
            StringContent content = new StringContent(JsonConvert.SerializeObject(criteria), Encoding.UTF8, "application/json");
            Task<HttpResponseMessage> taskResponse = client.PostAsync($"{_settings.ApiUrl}GetAllVenues", content);
            taskResponse.Wait();
            Task<string> taskJSON = taskResponse.Result.Content.ReadAsStringAsync();
            taskJSON.Wait();
            string json = taskJSON.Result;
                
            // Return data as model objects
            if (!json.StartsWith("["))
                json = "[" + json + "]";
            return JsonConvert.DeserializeObject<IEnumerable<AzureSearchVenueModel>>(json);
        }
    }
}
