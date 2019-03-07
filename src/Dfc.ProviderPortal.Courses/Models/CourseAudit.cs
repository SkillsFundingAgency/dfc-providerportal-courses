
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Dfc.ProviderPortal.Courses.Interfaces;


namespace Dfc.ProviderPortal.Courses.Models
{
    public class CourseAudit : ICourseAudit
    {
        public Guid id { get; set; }    
        public string Collection { get; set; }
        public string DocumentId { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Document Document { get; set; }
    }
}
