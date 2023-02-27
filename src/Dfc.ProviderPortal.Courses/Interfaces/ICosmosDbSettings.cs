namespace Dfc.ProviderPortal.Courses.Interfaces
{
    public interface ICosmosDbSettings
    {
        string EndpointUri { get; }
        string PrimaryKey { get; }
        string DatabaseId { get; }
        int RecentCount { get; }
    }
}
