using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerResigrassEstesies.Models
{
    public class ResponseLogin
    {
       public string message {  get; set; }
        public string Success { get; set; }

        public string token { get; set; }
    }
}