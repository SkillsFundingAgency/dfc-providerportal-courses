using System.Threading.Tasks;
using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Packages;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Logging;

namespace Dfc.ProviderPortal.Courses.Helpers
{
    public class QualificationServiceWrapper : IQualificationServiceWrapper
    {
        private readonly ILogger _log;
        private readonly IQualificationServiceSettings _settings;
        private static SearchServiceClient _service;
        private static ISearchIndexClient _index;

        public QualificationServiceWrapper(IQualificationServiceSettings settings)
        {
            Throw.IfNull(settings, nameof(settings));
            _settings = settings;

            _service = new SearchServiceClient(settings.SearchService, new SearchCredentials(settings.QueryKey));
            _index = _service.Indexes.GetClient(_settings.Index);
        }

        public Task<dynamic> GetQualificationById(string LARSRef)
        {
            return _index.Documents.GetAsync<dynamic>(LARSRef);
        }
    }
}
