using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Courses.Models;
using Dfc.ProviderPortal.Courses.Settings;
using Microsoft.Extensions.Options;

namespace Dfc.ProviderPortal.Courses.Services
{
    public class DfcReportService : IDfcReportService
    {
        private readonly ICosmosDbHelper _cosmosDbHelper;
        private readonly ICosmosDbCollectionSettings _settings;

        public DfcReportService(ICosmosDbHelper cosmosDbHelper, IOptions<CosmosDbCollectionSettings> settings)
        {
            _cosmosDbHelper = cosmosDbHelper;
            _settings = settings.Value;
        }

        public async Task<IEnumerable<DfcMigrationReport>> GetDfcReports()
        {
            try
            {
                using (var client = _cosmosDbHelper.GetClient())
                {
                    var result = await _cosmosDbHelper.GetAllDfcMigrationReports(client,
                        _settings.DfcReportCollectionId);
                    return result;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task Initialise()
        {
            using (var client = _cosmosDbHelper.GetClient())
            {
                await _cosmosDbHelper.CreateDocumentCollectionIfNotExistsAsync(client,
                    _settings.DfcReportCollectionId);
            }
        }
    }
}
