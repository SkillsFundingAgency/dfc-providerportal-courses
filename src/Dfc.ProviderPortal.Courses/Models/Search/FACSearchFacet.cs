
using System;
using System.Collections.Generic;
using Dfc.ProviderPortal.Courses.Interfaces;


namespace Dfc.ProviderPortal.Courses.Models
{
    public class FACSearchFacet : IFACSearchFacet
    {
        public int? count { get; }
        public dynamic value { get; }
    }
}
