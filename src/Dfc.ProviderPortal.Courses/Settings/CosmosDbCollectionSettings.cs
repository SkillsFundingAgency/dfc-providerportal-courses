using Dfc.ProviderPortal.Courses.Interfaces;

namespace Dfc.ProviderPortal.Courses.Settings
{
    public class CosmosDbCollectionSettings : ICosmosDbCollectionSettings
    {
        public string CoursesCollectionId { get; set; }
        public string AuditCollectionId { get; set; }
        public string CoursesMigrationReportCollectionId { get; set; }
        public string DfcReportCollectionId { get; set; }
    }
}
