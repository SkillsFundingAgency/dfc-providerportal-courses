using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.ProviderPortal.Courses.Interfaces
{
    public interface IBulkUploadError
    {
        int LineNumber { get; }
        string Header { get; }
        string Error { get; }
    }
}
