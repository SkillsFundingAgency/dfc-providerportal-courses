using System.Collections.Generic;
using Dfc.ProviderPortal.Courses.Models;


namespace Dfc.ProviderPortal.Courses.Interfaces
{
    public interface IProviderServiceWrapper
    {
        IEnumerable<AzureSearchProviderModel> GetLiveProvidersForAzureSearch();
    }
}
