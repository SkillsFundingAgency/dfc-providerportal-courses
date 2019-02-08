
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;


namespace Dfc.ProviderPortal.Courses.Services
{
    public class FACSearchResultContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            var names = new Dictionary<string, string>
            {
                { "ODataContext", "@odata.context" },
                { "ODataCount", "@odata.count" },
                { "SearchFacets", "@search.facets" },
                { "NotionalNVQLevelv2ODataType", "NotionalNVQLevelv2@odata.type" },
                { "ProviderNameODataType", "ProviderName@odata.type" },
                { "RegionODataType", "Region@odata.type" },
                { "SearchScore", "@search.score" }
            };

            if (names.ContainsKey(propertyName))
            {
                names.TryGetValue(propertyName, out string name);
                return name;
            }

            return propertyName;
        }
    }
}