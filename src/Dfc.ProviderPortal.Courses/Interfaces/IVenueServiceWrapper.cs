
using System;
using System.Collections.Generic;
using Dfc.ProviderPortal.Courses.Models;


namespace Dfc.ProviderPortal.Courses.Interfaces
{
    public interface IVenueServiceWrapper
    {
        IEnumerable<AzureSearchVenueModel> GetVenues();
        T GetById<T>(Guid id);
    }
}
