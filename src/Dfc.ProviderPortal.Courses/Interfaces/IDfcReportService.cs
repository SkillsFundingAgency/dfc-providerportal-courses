using Dfc.ProviderPortal.Courses.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dfc.ProviderPortal.Courses.Interfaces
{
    public interface IDfcReportService
    {
        Task<IEnumerable<DfcMigrationReport>> GetDfcReports();
        Task Initialise();
    }
}
