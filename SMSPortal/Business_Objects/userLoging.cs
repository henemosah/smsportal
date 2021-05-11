using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMSPortal.Business_Objects
{
    public class userLoging
    {
        public string password { get; set; }
        public string msg { get; set; }
        public string username { get; set; }       
        public string role { get; set; }
        public string email { get; set; }
        public DateTime datecreated { get; set; }
        public string salt { get; set; }
        public Boolean status { get; set; }
    }
}