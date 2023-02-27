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
