
using System;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Courses.Models;

namespace Dfc.ProviderPortal.Courses.Interfaces
{
    public interface ICourseMigrationReportService
    {
        Task Initialise();
        Task AddMigrationReport(CourseMigrationReport courseReport);
        CourseMigrationReport GetMigrationReport(int UKPRN);
    }
}
