using Dfc.ProviderPortal.Courses.Interfaces;

namespace Dfc.ProviderPortal.Courses.Settings
{
    public class ReferenceDataServiceSettings : IReferenceDataServiceSettings
    {
        public string ApiUrl { get; set; }
        public string ApiKey { get; set; }
    }
}
