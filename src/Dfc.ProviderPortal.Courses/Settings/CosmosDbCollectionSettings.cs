using Dfc.ProviderPortal.Courses.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.ProviderPortal.Courses.Settings
{
    public class CosmosDbCollectionSettings : ICosmosDbCollectionSettings
    {
        public string CoursesCollectionId { get; set; }
    }
}
