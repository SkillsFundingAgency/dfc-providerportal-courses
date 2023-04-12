namespace Dfc.ProviderPortal.Courses.Models
{
    public class AzureSearchCourseDetail
    {
        public Course Course { get; set; }
        public dynamic Provider { get; set; }
        public dynamic CourseRunVenues { get; set; }
        public dynamic Qualification { get; set; }
        public dynamic FeChoice { get; set; }
    }
}
