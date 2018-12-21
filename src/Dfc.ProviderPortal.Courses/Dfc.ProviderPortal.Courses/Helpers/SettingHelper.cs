using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.ProviderPortal.Courses.Helpers
{
    public static class SettingsHelper
    {
        /// <summary>
        /// Built config root from settings file
        /// </summary>
        static private IConfigurationRoot config = new ConfigurationBuilder().AddEnvironmentVariables()
                                                                             .Build();

        /// <summary>
        /// Properties wrapping up app setttings
        /// </summary>
        static public string ConnectionString = config.GetValue<string>("APPSETTING_SQLConnectionString");
        static public string StorageURI = config.GetValue<string>("APPSETTING_CosmosDBStorageURI");
        static public string PrimaryKey = config.GetValue<string>("APPSETTING_CosmosDBPrimaryKey");
        static public string Database = config.GetValue<string>("APPSETTING_CosmosDBDatabase");
        static public string Collection = config.GetValue<string>("APPSETTING_CollectionName");
    }
}
