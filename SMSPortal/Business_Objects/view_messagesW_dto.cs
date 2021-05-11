using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMSPortal.Business_Objects
{
    public class view_messagesW_dto
    {
        public int Id { get; set; }
        public string code_b { get; set; }
        public int damaged { get; set; }
        public int caked { get; set; }
        public bool status { get; set; }
        public string complaints { get; set; }
        public Nullable<System.DateTime> created_on { get; set; }
        public int sms_request_id { get; set; }
        public int missing { get; set; }
    }
}