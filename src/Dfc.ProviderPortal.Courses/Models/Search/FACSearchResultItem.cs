
using System;
using System.Collections.Generic;
using Dfc.ProviderPortal.Courses.Interfaces;


namespace Dfc.ProviderPortal.Courses.Models
{
    public class FACSearchResultItem : AzureSearchCourse
    {
        public decimal SearchScore { get; }
        public new dynamic VenueLocation { get; set; }

    }
}
