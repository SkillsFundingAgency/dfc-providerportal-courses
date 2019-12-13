using System.Net.Http;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Courses.Models;
using Newtonsoft.Json;

namespace Dfc.ProviderPortal.Courses.Helpers
{
    public class FeChoiceServiceWrapper : IFeChoiceServiceWrapper
    {
        private readonly IReferenceDataServiceSettings _settings;

        public FeChoiceServiceWrapper(IReferenceDataServiceSettings settings)
        {
            _settings = settings;
        }

        public async Task<FeChoice> GetByUKPRNAsync(int ukprn)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _settings.ApiKey);

                var response = await httpClient.GetAsync($"{_settings.ApiUrl}fe-choices/{ukprn}");

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
}
