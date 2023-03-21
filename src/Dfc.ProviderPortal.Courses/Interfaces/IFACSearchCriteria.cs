using System.Collections.Generic;


namespace Dfc.ProviderPortal.Courses.Interfaces
{
    public interface IFACSearchCriteria
    {
        string scoringProfile { get; set; }
        string search { get; }
        string searchMode { get; }
        int? top { get; }
        string filter { get; }
        IEnumerable<string> facets { get; }
        bool count { get; }
    }
}
