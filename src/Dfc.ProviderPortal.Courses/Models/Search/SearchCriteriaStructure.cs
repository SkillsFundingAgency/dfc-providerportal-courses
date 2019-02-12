
using System;
using Dfc.ProviderPortal.Packages;


namespace Dfc.ProviderPortal.Courses.Models
{
    public class SearchCriteriaStructure
    {
        //public string APIKeyField { get; set; }
        public string SubjectKeyword { get; set; }
        public string DFE1619Funded { get; set; }
        //public string LocationField { get; set; }
        public float? Distance { get; set; }
        //public bool DistanceFieldSpecified { get; set; }
        //public string ProviderIDField { get; set; }
        //public string ProviderKeywordField { get; set; }
        //public string[] LDCSField { get; set; }
        //public string[] QualificationTypesField { get; set; }
        public string[] QualificationLevels { get; set; }
        public string[] StudyModes { get; set; }
        public string[] AttendanceModes { get; set; }
        public string[] AttendancePatterns { get; set; }
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

        public int? TopResults { get; set; }


        public SearchCriteriaStructure()
        {
            //if (TopResults.HasValue)
            //    Throw.IfLessThan(1, TopResults.Value, "");
        }
    }
}
