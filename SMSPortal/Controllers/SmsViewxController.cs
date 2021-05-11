using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMSPortal.Business_Objects;
using SMSPortal.Models;

namespace SMSPortal.Controllers
{
    public class SmsViewController : Controller
    {
        // GET: SmsView
        private ePOD_DBEntities1 db = new ePOD_DBEntities1();
        public ActionResult Index()
        {
            DateTime selected_date = DateTime.Today.Date;
            ViewBag.selected_date = selected_date;

            var xx = selected_date.ToString("d");

            var baseSmsx = db.baseSMS.Where(a => a.create_datetime.Value.Date == selected_date).OrderByDescending(s => s.create_datetime);
            List <view_message_dto> message_full_list = new List<view_message_dto>();

            message_full_list.AddRange(
                from msgs in baseSmsx
                select new view_message_dto
                {
                    Id = msgs.Id,
                    atc_no = msgs.atc_no,
                    delivery_no = msgs.delivery_no,
                    customer_no = msgs.customer_no,
                    phone_no = msgs.phone_no,
                    waybill_no = msgs.waybill_no,
                    customer_name = msgs.customer_name,
                    ip_address = msgs.ip_address,
                    code_a = msgs.code_a,
                    code_b = msgs.code_b,
                    sms_status = true,
                    response = msgs.response,
                    create_datetime = msgs.create_datetime,
                    response_datetime = msgs.response_datetime

                }
                );

            return View();
        }
    }
}