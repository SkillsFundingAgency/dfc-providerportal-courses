
using System;
using Dfc.ProviderPortal.Courses.Interfaces;


namespace Dfc.ProviderPortal.Courses.Settings
{
    public class SearchServiceSettings : ISearchServiceSettings
    {
        public string SearchService { get; set; }
        public string QueryKey { get; set; }
        public string AdminKey { get; set; }
        public string Index { get; set; }
        public int DefaultTop { get; set; }
        public int ThresholdVenueCount { get; set; }
    }
}
