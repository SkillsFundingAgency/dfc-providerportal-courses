
using System;
using System.Collections.Generic;
using Dfc.ProviderPortal.Courses.Interfaces;


namespace Dfc.ProviderPortal.Courses.Models
{
    public class FACSearchResultItem : AzureSearchCourse
    {
        public dynamic SearchScore { get; set; }
        public new dynamic VenueLocation { get; set; }

    }
}
