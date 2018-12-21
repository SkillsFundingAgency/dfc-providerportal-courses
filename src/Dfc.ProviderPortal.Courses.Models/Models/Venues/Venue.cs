using Dfc.ProviderPortal.Courses.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.ProviderPortal.Courses.Models.Models.Venues
{
    public class Venue
    {
        string ID { get; }
        int UKPRN { get; }
        int ProviderID { get; }
        int VenueID { get; }
        string VenueName { get; }
        string ProvVenueID { get; }
        string Address1 { get; }
        string Address2 { get; }
        string Town { get; }
        string County { get; }
        string PostCode { get; }
        VenueStatus Status { get; }
        DateTime DateAdded { get; }
        DateTime DateUpdated { get; }
        string UpdatedBy { get; }
    }
}
