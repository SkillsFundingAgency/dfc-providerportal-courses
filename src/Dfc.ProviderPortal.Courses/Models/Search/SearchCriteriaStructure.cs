namespace Dfc.ProviderPortal.Courses.Models
{
    public class SearchCriteriaStructure
    {
        public string SubjectKeyword { get; set; }
        public string DFE1619Funded { get; set; }
        public float? Distance { get; set; }
        public string[] QualificationLevels { get; set; }
        public string[] StudyModes { get; set; }
        public string[] AttendanceModes { get; set; }
        public string[] AttendancePatterns { get; set; }
        public string TownOrPostcode { get; set; }

        public int? TopResults { get; set; }


        public SearchCriteriaStructure()
        {
        }
    }
}
