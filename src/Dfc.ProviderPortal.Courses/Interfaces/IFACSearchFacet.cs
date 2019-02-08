
using System;
using System.Collections.Generic;


namespace Dfc.ProviderPortal.Courses.Interfaces
{
    public interface IFACSearchFacet
    {
        int? Count { get; }
        string Value { get; }
    }
}
