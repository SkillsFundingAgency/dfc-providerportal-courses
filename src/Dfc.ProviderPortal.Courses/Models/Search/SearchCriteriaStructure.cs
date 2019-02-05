
using System;


namespace Dfc.ProviderPortal.Courses.Models
{
    public class SearchCriteriaStructure
    {
        //public string APIKeyField { get; set; }
        public string SubjectKeywordField { get; set; }
        public string DFE1619FundedField { get; set; }
        //public string LocationField { get; set; }
        public float DistanceField { get; set; }
        //public bool DistanceFieldSpecified { get; set; }
        //public string ProviderIDField { get; set; }
        //public string ProviderKeywordField { get; set; }
        //public string[] LDCSField { get; set; }
        //public string[] QualificationTypesField { get; set; }
        public string[] QualificationLevelsField { get; set; }
        public string[] StudyModesField { get; set; }
        public string[] AttendanceModesField { get; set; }
        public string[] AttendancePatternsField { get; set; }
        //public string[] A10CodesField { get; set; }
        //public string EarliestStartDateField { get; set; }
        //public string TTGFlagField { get; set; }
        //public string TQSFlagField { get; set; }
        //public string IESFlagField { get; set; }
        //public string FlexStartFlagField { get; set; }
        //public string OppsAppClosedFlagField { get; set; }
        //public string[] ERAppStatusField { get; set; }
        //public string[] ERTtgStatusField { get; set; }
        //public string[] AdultLRStatusField { get; set; }
        //public string[] OtherFundingStatusField { get; set; }
        //public string SFLFlagField { get; set; }
        //public string ILSFlagField { get; set; }
        public string TownOrPostcode { get; set; }
    }
}
