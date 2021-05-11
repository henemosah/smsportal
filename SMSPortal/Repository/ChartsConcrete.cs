using SMSPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMSPortal.Repository
{
    public class ChartsConcrete : ICharts
    {
        private ePOD_DBEntities1 db = new ePOD_DBEntities1();
        public void smsCounter(out string SmsCountList, out string MonthNamex)
        {
            var smsdata = db.sms_count_per_month().ToList();
            var smsCount = (from temp in smsdata select temp.cx).ToList();
            var monthName = (from temp in smsdata select temp.name).ToList();
            

            SmsCountList = string.Join(",", smsCount);
            //MonthNamex = string.Join(", ", monthName);

            MonthNamex = "'jan', 'feb', 'mar', 'apr', 'may', 'jun', 'jul', 'aug', 'sep', 'oct', 'nov', 'dec'";

        }



    }
}