
using System;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Dfc.ProviderPortal.Courses.Models;


namespace Dfc.ProviderPortal.Courses.Interfaces
{
    public interface ICourseAudit
    {
        Guid id { get; }
        string Collection { get; }
        string DocumentId { get; }
        DateTime UpdatedDate { get; }
        string UpdatedBy { get; }
        Document Document { get; }
    }
}