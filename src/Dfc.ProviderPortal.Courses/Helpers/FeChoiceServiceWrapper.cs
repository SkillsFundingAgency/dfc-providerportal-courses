using System;
using System.Net.Http;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Courses.Models;
using Newtonsoft.Json;

namespace Dfc.ProviderPortal.Courses.Helpers
{
    public class FeChoiceServiceWrapper : IFeChoiceServiceWrapper, IDisposable
    {
        private readonly IReferenceDataServiceSettings _settings;
        private readonly HttpClient _httpClient;

        public FeChoiceServiceWrapper(IReferenceDataServiceSettings settings)
        {
            _settings = settings;

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _settings.ApiKey);
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }

        public async Task<FeChoice> GetByUKPRNAsync(int ukprn)
        {
            var response = await _httpClient.GetAsync($"{_settings.ApiUrl}/fe-choices/{ukprn}");

            if ((int)response.StatusCode == 404)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<FeChoice>(json);
        }
    }
}
