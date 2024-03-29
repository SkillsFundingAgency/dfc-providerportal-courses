﻿
using System;
using System.Collections.Generic;
using System.Linq;


namespace Dfc.ProviderPortal.Courses.Models
{
    public class Region
    {
        public enum Regions
        {
            E12000001,
            E12000002,
            E12000003,
            E12000004,
            E12000005,
            E12000006,
            E12000007,
            E12000008,
            E12000009
        }

        public static IEnumerable<string> RegionList()
        {
            return Enum.GetValues(typeof(Regions)).Cast<string>();
        }
    }
}
