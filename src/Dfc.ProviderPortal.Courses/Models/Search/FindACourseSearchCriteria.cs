
//using System;
//using System.Text;
//using System.Collections.Generic;
//using Dfc.ProviderPortal.Packages;
//using Dfc.ProviderPortal.Courses.Interfaces;


//namespace Dfc.ProviderPortal.Courses.Models
//{
//    public class FACSearchCriteria : IFACSearchCriteria
//    {
//        //public string aPIKeyField { get; set; }
//        public string Keyword { get; } //public string subjectKeywordField { get; set; }
//        //public string dFE1619FundedField { get; set; }
//        public string LocationPostcode { get; } //public string locationField { get; set; }
//        public int DistanceInMiles { get; } //public float distanceField { get; set; }
//        //public bool distanceFieldSpecified { get; set; }
//        //public string providerIDField { get; set; }
//        //public string providerKeywordField { get; set; }
//        //public string[] lDCSField { get; set; }
//        //public string[] qualificationTypesField { get; set; }
//        public string QualificationLevel { get; } //public string[] qualificationLevelsField { get; }
//        //public string[] studyModesField { get; set; }
//        //public string[] attendanceModesField { get; set; }
//        //public string[] attendancePatternsField { get; set; }
//        //public string[] a10CodesField { get; set; }
//        //public string earliestStartDateField { get; set; }
//        //public string tTGFlagField { get; set; }
//        //public string tQSFlagField { get; set; }
//        //public string iESFlagField { get; set; }
//        //public string flexStartFlagField { get; set; }
//        //public string oppsAppClosedFlagField { get; set; }
//        //public string[] eRAppStatusField { get; set; }
//        //public string[] eRTtgStatusField { get; set; }
//        //public string[] adultLRStatusField { get; set; }
//        //public string[] otherFundingStatusField { get; set; }
//        //public string sFLFlagField { get; set; }
//        //public string iLSFlagField { get; set; }

//        public FACSearchCriteria(string keyword, string locationpostcode, int distanceinmiles, string qualificationlevel)
//        {
//            Throw.IfNullOrWhiteSpace(keyword, nameof(keyword));

//            Keyword = keyword;
//            LocationPostcode = locationpostcode; // string.IsNullOrWhiteSpace(locationpostcode) ? ;
//            DistanceInMiles = distanceinmiles;
//            QualificationLevel = qualificationlevel;
//        }
//    }
//}
