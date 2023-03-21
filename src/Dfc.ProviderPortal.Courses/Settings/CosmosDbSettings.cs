using Dfc.ProviderPortal.Courses.Interfaces;

namespace Dfc.ProviderPortal.Courses.Settings
{
    public class CosmosDbSettings : ICosmosDbSettings
    {
        public string EndpointUri { get; set; }
        public string PrimaryKey { get; set; }
        public string DatabaseId { get; set; }
        public int RecentCount { get; set; }
    }
}
