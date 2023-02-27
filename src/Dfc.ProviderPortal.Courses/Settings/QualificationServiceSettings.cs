using Dfc.ProviderPortal.Courses.Interfaces;


namespace Dfc.ProviderPortal.Courses.Settings
{
    public class QualificationServiceSettings : IQualificationServiceSettings
    {
        public string SearchService { get; set; }
        public string QueryKey { get; set; }
        public string AdminKey { get; set; }
        public string Index { get; set; }
    }
}
