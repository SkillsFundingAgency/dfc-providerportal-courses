using System.Collections.Generic;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Courses.Models;

namespace Dfc.ProviderPortal.Courses.Interfaces
{
    public interface IDfcReportService
    {
        Task<IEnumerable<DfcMigrationReport>> GetDfcReports();
        Task Initialise();
    }
}
