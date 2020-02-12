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
    public class ProviderServiceWrapper : IProviderServiceWrapper, IDisposable
    {
        private readonly IProviderServiceSettings _settings;
        private readonly HttpClient _httpClient;

        public ProviderServiceWrapper(IProviderServiceSettings settings)
        {
            Throw.IfNull(settings, nameof(settings));
            _settings = settings;

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _settings.ApiKey);
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }

        public async Task<dynamic> GetByPRN(int PRN)
        {
            var response = await _httpClient.GetAsync($"{_settings.ApiUrl}GetProviderByPRN?PRN={PRN}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<dynamic>(json);
        }

        public IEnumerable<AzureSearchProviderModel> GetLiveProvidersForAzureSearch()
        {
            // Call service to get data
            var criteria = new object();
            StringContent content = new StringContent(JsonConvert.SerializeObject(criteria), Encoding.UTF8, "application/json");
            Task<HttpResponseMessage> taskResponse = _httpClient.PostAsync($"{_settings.ApiUrl}GetLiveProvidersForAzureSearch", content);
            taskResponse.Wait();
            Task<string> taskJSON = taskResponse.Result.Content.ReadAsStringAsync();
            taskJSON.Wait();
            string json = taskJSON.Result;

            // Return data as model objects
            if (!json.StartsWith("["))
                json = "[" + json + "]";
            return JsonConvert.DeserializeObject<IEnumerable<AzureSearchProviderModel>>(json);
        }
    }
}
