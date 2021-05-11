using SMSPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMSPortal.Repository
{
    public class ChartConcrete2 : ICharts2
    {
        private ePOD_DBEntities1 db = new ePOD_DBEntities1();
        public void smsWhatsappCounter(out string SmsWhatsappDailyCountList, out string msgTypeCountList)
        {
            var smsWhatsappData = db.sms_whatsapp_count_per_day().ToList();
            var msgTypeCount = (from temp in smsWhatsappData select temp.name).ToList();
            var smsWhatsappDailyCount = (from temp in smsWhatsappData select temp.counters).ToList();


            SmsWhatsappDailyCountList = string.Join(",", smsWhatsappDailyCount);
            //msgTypeCountList = string.Join(", ", msgTypeCount);

            msgTypeCountList = "'sms', 'whatsapp'";

        }
    }
}