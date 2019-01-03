using Dfc.ProviderPortal.Courses.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.ProviderPortal.Courses.Models
{
    public class Course : ICourse
    {
        public Guid id { get; set; }
        public string Testy { get; set; }
    }
}
