using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMSPortal.Repository
{
    public interface ICharts2
    {
        void smsWhatsappCounter(out string SmsWhatsappDailyCountList, out string msgTypeList);
    }
}