namespace Dfc.ProviderPortal.Courses.Interfaces
{
    public interface ICosmosDbCollectionSettings
    {
        string CoursesCollectionId { get; }
        string AuditCollectionId { get; }
        string CoursesMigrationReportCollectionId { get; }
        string DfcReportCollectionId { get; }
    }
}
