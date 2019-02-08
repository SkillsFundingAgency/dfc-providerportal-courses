
using System;


namespace Dfc.ProviderPortal.Courses.Interfaces
{
    public interface ISearchServiceSettings
    {
        string SearchService { get; }
        string ApiUrl { get; }
        string ApiVersion { get; }
        string QueryKey { get; }
        string AdminKey { get; }
        string Index { get; }
        int DefaultTop { get; }
        int ThresholdVenueCount { get; }
    }
}
