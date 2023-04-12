using Dfc.ProviderPortal.Courses.Interfaces;

namespace Dfc.ProviderPortal.Courses.Models
{
    public class BulkUploadError : IBulkUploadError
    {
        public int LineNumber { get; set; }
        public string Header { get; set; }
        public string Error { get; set; }
    }
}
