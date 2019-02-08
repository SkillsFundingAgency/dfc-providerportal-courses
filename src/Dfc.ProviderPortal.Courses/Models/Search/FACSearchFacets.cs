
using System;
using System.Collections.Generic;
using Dfc.ProviderPortal.Courses.Interfaces;


namespace Dfc.ProviderPortal.Courses.Models
{
    public class FACSearchFacets : IFACSearchFacets
    {
        public IEnumerable<FACSearchFacet> NotionalNVQLevelv2 { get; }
        //public string NotionalNVQLevelv2ODataType { get; }
        public IEnumerable<FACSearchFacet> ProviderName { get; }
        //public string ProviderNameODataType { get; }
        public IEnumerable<FACSearchFacet> Region{ get; }
        //public string RegionODataType { get; }
    }
}
