
using System;
using System.Collections.Generic;
using Dfc.ProviderPortal.Courses.Models;


namespace Dfc.ProviderPortal.Courses.Interfaces
{
    public interface IFACSearchResult
    {
        string ODataContext { get; set; }
        //int? top { get; set; }
        int? ODataCount { get; set; }
        FACSearchFacets SearchFacets { get; set; }
        IEnumerable<FACSearchResultItem> Value { get; set; }
    }
}
