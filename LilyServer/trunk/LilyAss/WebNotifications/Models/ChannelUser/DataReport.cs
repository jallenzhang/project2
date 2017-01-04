using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebNotifications.Models.ChannelUser
{
    public class DataReport
    {
        public string MyDateTime { get; set; }
        public int ActiveNum { get; set; }
        public int StartNum { get; set; }
        public int PayNum { get; set; }
        public decimal IncomeNum { get; set; }
    }

    public class DataReportColumn
    {
        public string MyDateTime { get; set; }
        public int Num { get; set; }
        public decimal DNum { get; set; }
    }

    public enum DataReportColumnType
    {
        Active = 1,
        Start,
        Pay,
        Income
    }
}