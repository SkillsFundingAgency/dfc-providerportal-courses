using Dfc.ProviderPortal.Courses.Models;


namespace Dfc.ProviderPortal.Courses.Interfaces
{
    public interface IAzureSearchCourseDetail //: ICourse
    {
        Course Course { get; set; }
        dynamic Provider { get; set; }
        dynamic Venue { get; set; }
        dynamic Qualification { get; set; }
    }
}
