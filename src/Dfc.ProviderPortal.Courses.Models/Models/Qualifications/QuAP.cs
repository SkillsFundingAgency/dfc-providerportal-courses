using Dfc.ProviderPortal.Courses.Models.Models.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.ProviderPortal.Courses.Models.Models.Qualifications
{
    public class QuAP
    {
        Guid ID { get; }
        Qualification Qualification { get; }
        Provider Provider { get; }
    }
}
