﻿
using System;
using System.Collections.Generic;


namespace Dfc.ProviderPortal.Courses.Interfaces
{
    public interface IFACSearchFacet
    {
        int? count { get; }
        dynamic value { get; }
    }
}
