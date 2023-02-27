using System.Collections.Generic;
using Dfc.ProviderPortal.Courses.Models;


namespace Dfc.ProviderPortal.Courses.Interfaces
{
    public interface IFACSearchResult
    {
        string ODataContext { get; set; }
        int? ODataCount { get; set; }
        dynamic SearchFacets { get; set; } //FACSearchFacets SearchFacets { get; set; }
        IEnumerable<FACSearchResultItem> Value { get; set; }
    }
}
