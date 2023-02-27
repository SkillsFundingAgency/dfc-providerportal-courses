using System;
using Dfc.ProviderPortal.Courses.Helpers;
using Newtonsoft.Json;


namespace Dfc.ProviderPortal.Courses.Models
{
    [JsonConverter(typeof(AzureSearchProviderModelJsonConverter))]
    public class AzureSearchProviderModel
    {
        public Guid? id { get; set; }
        public int UnitedKingdomProviderReferenceNumber { get; set; }
        public string ProviderName { get; set; }
    }
}
