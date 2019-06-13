using Dfc.ProviderPortal.Courses.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.ProviderPortal.Courses.Models
{
    public class BulkUploadError : IBulkUploadError
    {
        public int LineNumber { get; set; }
        public string Header { get; set; }
        public string Error { get; set; }
    }
}
