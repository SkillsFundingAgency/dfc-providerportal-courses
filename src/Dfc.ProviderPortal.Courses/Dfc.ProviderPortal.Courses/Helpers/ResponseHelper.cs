using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.ProviderPortal.Courses.Helpers
{
    public static class ResponseHelper
    {
        static public string ErrorMessage(string msg)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(new { message = msg });
        }
    }
}
