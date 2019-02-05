﻿
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

        public dynamic GetByPRN(int PRN)
        {
            // Call service to get data
            HttpClient client = new HttpClient();
            var criteria = new { PRN };
            StringContent content = new StringContent(JsonConvert.SerializeObject(criteria), Encoding.UTF8, "application/json");
            Task<HttpResponseMessage> taskResponse = client.PostAsync($"{_settings.ApiUrl}GetProviderByPRN?code={_settings.ApiKey}", content);
            taskResponse.Wait();
            Task<string> taskJSON = taskResponse.Result.Content.ReadAsStringAsync();
            taskJSON.Wait();
            string json = taskJSON.Result;

            // Return data as model object
            return JsonConvert.DeserializeObject<dynamic>(json);
        }

        public IEnumerable<AzureSearchProviderModel> GetLiveProvidersForAzureSearch()
        {
            // Call service to get data
            HttpClient client = new HttpClient();
            var criteria = new object();
            StringContent content = new StringContent(JsonConvert.SerializeObject(criteria), Encoding.UTF8, "application/json");
            Task<HttpResponseMessage> taskResponse = client.PostAsync($"{_settings.ApiUrl}GetLiveProvidersForAzureSearch?code={_settings.ApiKey}", content);
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
