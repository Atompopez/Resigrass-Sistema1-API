using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ServerResigrassEstesies.Logic
{
    public static class Globals
    {
        public static string url = ConfigurationManager.AppSettings["BaseApiUrl"];
    }
}