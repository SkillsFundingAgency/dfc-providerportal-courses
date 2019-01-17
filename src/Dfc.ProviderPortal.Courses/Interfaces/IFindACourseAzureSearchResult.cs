
using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Dfc.ProviderPortal.Courses.Models;


namespace Dfc.ProviderPortal.Courses.Interfaces
{
    public interface IFindACourseAzureSearchResult
    {
        Course Course { get; }
        dynamic Provider { get; }
        dynamic Venue { get; }
    }
}
