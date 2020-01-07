using System.Threading.Tasks;
using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Packages;
using Microsoft.Azure.Search;

namespace Dfc.ProviderPortal.Courses.Helpers
{
    public class QualificationServiceWrapper : IQualificationServiceWrapper
    {
        private readonly IQualificationServiceSettings _settings;

        public QualificationServiceWrapper(IQualificationServiceSettings settings)
        {
            Throw.IfNull(settings, nameof(settings));
            _settings = settings;
        }

        public async Task<dynamic> GetQualificationById(string LARSRef)
        {
            using (var service = new SearchServiceClient(_settings.SearchService, new SearchCredentials(_settings.QueryKey)))
            {
                var index = service.Indexes.GetClient(_settings.Index);

                return await index.Documents.GetAsync<dynamic>(LARSRef);
            }
        }
    }
}
