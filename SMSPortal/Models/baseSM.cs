//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SMSPortal.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class baseSM
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
        public Nullable<bool> sms_status { get; set; }
        public string response { get; set; }
        public Nullable<System.DateTime> create_datetime { get; set; }
        public Nullable<System.DateTime> response_datetime { get; set; }
    }
}