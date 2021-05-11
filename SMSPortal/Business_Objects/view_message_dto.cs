using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMSPortal.Business_Objects
{
    public class view_message_dto
    {
        public int Id { get; set; }
        public string atc_no { get; set; }
        public string delivery_no { get; set; }
        public string customer_no { get; set; }
        public string phone_no { get; set; }
        public string waybill_no { get; set; }
        public string customer_name { get; set; }
        public string ip_address { get; set; }
        public string code_a { get; set; }
        public string code_b { get; set; }
        public bool sms_status { get; set; }
        public string response { get; set; }
        public Nullable<System.DateTime> create_datetime { get; set; }
        public Nullable<System.DateTime> response_datetime { get; set; }
    }
}