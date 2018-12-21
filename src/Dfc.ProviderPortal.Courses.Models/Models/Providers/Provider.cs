using System;

namespace Dfc.ProviderPortal.Courses.Models.Models.Providers
{
    public class Provider
    {
        public Guid id { get; set; }
        public string UnitedKingdomProviderReferenceNumber { get; set; }
        public string ProviderName { get; set; }
        public string ProviderStatus { get; set; }
        public Providercontact[] ProviderContact { get; set; }
        public DateTime ProviderVerificationDate { get; set; }
        public bool ProviderVerificationDateSpecified { get; set; }
        public bool ExpiryDateSpecified { get; set; }
        public object ProviderAssociations { get; set; }
        public Provideralias[] ProviderAliases { get; set; }
        public Verificationdetail[] VerificationDetails { get; set; }
        public Status Status { get; set; }

    }

    public enum Status
    {
        Registered = 0,
        Onboarded = 1,
        Unregistered = 2
    }
}
