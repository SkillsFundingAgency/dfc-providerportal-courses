using Dfc.ProviderPortal.Courses.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.ProviderPortal.Courses.Settings
{
    public class CosmosDbSettings : ICosmosDbSettings
    {
        public string EndpointUri { get; set; }
        public string PrimaryKey { get; set; }
        public string DatabaseId { get; set; }
    }
}
