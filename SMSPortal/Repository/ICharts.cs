using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMSPortal.Repository
{
    public interface ICharts
    {
        void smsCounter(out string SmsCountList, out string MonthList);

        
    }
}