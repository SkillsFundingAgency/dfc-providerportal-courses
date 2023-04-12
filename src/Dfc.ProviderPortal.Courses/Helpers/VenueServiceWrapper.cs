
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Courses.Models;
using Dfc.ProviderPortal.Packages;
using Newtonsoft.Json;


namespace Dfc.ProviderPortal.Courses.Helpers
{
    public class VenueServiceWrapper : IVenueServiceWrapper, IDisposable
    {
        private readonly IVenueServiceSettings _settings;
        private readonly HttpClient _httpClient;

        public VenueServiceWrapper(IVenueServiceSettings settings)
        {
            Throw.IfNull(settings, nameof(settings));
            _settings = settings;

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _settings.ApiKey);
        }

        public T GetById<T>(Guid id)
        {
            // Call service to get data
            var criteria = new { id };
            StringContent content = new StringContent(JsonConvert.SerializeObject(criteria), Encoding.UTF8, "application/json");
            Task<HttpResponseMessage> taskResponse = _httpClient.PostAsync($"{_settings.ApiUrl}GetVenueById?code={_settings.ApiKey}", content);
            taskResponse.Wait();
            Task<string> taskJSON = taskResponse.Result.Content.ReadAsStringAsync();
            taskJSON.Wait();
            string json = taskJSON.Result;

            // Return data as model object
            return JsonConvert.DeserializeObject<T>(json);
        }

        public async Task<IEnumerable<dynamic>> GetVenuesByPRN(int prn)
        {
            var response = await _httpClient.GetAsync($"{_settings.ApiUrl}GetVenuesByPRN?prn={prn}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<IEnumerable<dynamic>>(json);
        }

        public IEnumerable<AzureSearchVenueModel> GetVenues()
        {
            // Call service to get data
            var criteria = new object();
            StringContent content = new StringContent(JsonConvert.SerializeObject(criteria), Encoding.UTF8, "application/json");
            Task<HttpResponseMessage> taskResponse = _httpClient.PostAsync($"{_settings.ApiUrl}GetAllVenues", content);
            taskResponse.Wait();
            Task<string> taskJSON = taskResponse.Result.Content.ReadAsStringAsync();
            taskJSON.Wait();
            string json = taskJSON.Result;

            // Return data as model objects
            if (!json.StartsWith("["))
                json = "[" + json + "]";
            return JsonConvert.DeserializeObject<IEnumerable<AzureSearchVenueModel>>(json);
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
