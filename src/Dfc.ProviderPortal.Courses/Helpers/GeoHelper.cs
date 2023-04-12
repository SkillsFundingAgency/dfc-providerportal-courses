
using System;


namespace Dfc.GeoCoordinate
{
    public class GeoHelper
    {
        public class Coordinates
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

        public static double DistanceTo(Coordinates dest, Coordinates home)
        {
            double rlat1 = Math.PI * dest.Latitude / 180;
            double rlat2 = Math.PI * home.Latitude / 180;
            double theta = dest.Longitude - home.Longitude;
            double rtheta = Math.PI * theta / 180;
            double dist = Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) * Math.Cos(rlat2) * Math.Cos(rtheta);
            return Math.Acos(dist) * 180 / Math.PI * 60 * 1.1515;
        }
    }
}
