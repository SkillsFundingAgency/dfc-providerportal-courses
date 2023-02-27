﻿using System.Collections.Generic;
using Dfc.ProviderPortal.Courses.Interfaces;


namespace Dfc.ProviderPortal.Courses.Models
{
    public class FACSearchFacets : IFACSearchFacets
    {
        public IEnumerable<FACSearchFacet> NotionalNVQLevelv2 { get; set; }
        public string NotionalNVQLevelv2ODataType { get; set; }
        public IEnumerable<FACSearchFacet> ProviderName { get; set; }
        public string ProviderNameODataType { get; set; }
        public IEnumerable<FACSearchFacet> Region { get; set; }
        public string RegionODataType { get; set; }
        public IEnumerable<FACSearchFacet> VenueAttendancePattern { get; set; }
        public string VenueAttendancePatternODataType { get; set; }
    }
}
