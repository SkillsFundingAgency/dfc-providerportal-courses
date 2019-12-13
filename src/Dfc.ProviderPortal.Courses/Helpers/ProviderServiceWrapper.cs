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
    public class ProviderServiceWrapper : IProviderServiceWrapper
    {
        private readonly IProviderServiceSettings _settings;

        public ProviderServiceWrapper(IProviderServiceSettings settings)
        {
            Throw.IfNull(settings, nameof(settings));
            _settings = settings;
        }

        public async Task<dynamic> GetByPRN(int PRN)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _settings.ApiKey);

                var response = await httpClient.GetAsync($"{_settings.ApiUrl}GetProviderByPRN?PRN={PRN}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<dynamic>(json);
            }
        }

        public IEnumerable<AzureSearchProviderModel> GetLiveProvidersForAzureSearch()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _settings.ApiKey);

                // Call service to get data
                var criteria = new object();
                StringContent content = new StringContent(JsonConvert.SerializeObject(criteria), Encoding.UTF8, "application/json");
                Task<HttpResponseMessage> taskResponse = httpClient.PostAsync($"{_settings.ApiUrl}GetLiveProvidersForAzureSearch", content);
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
}
