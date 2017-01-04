using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Webdiyer.WebControls.Mvc;

namespace WebNotifications.Models
{
    public class DeviceChannelObj
    {
        public string ChannelId { get; set; }
        public string ChannelName { get; set; }
        public double Money { get; set; }
        public double? Proportion { get; set; }
        public List<int> DeviceNumberList { get; set; }
        public List<DateTime> DateTimeList { get; set; }
        public List<double> PayMoneyList { get; set; }
    }
    
    public class DeviceChannelModel
    {
        public PagedList<DeviceChannelObj> DChannelObjList { get; set; }
        public List<string> DateNameList { get; set; }
    }

    public class DeviceSearchResult
    {
        public string ChannelId { get; set; }
        public string ChannelName { get; set; }
        public double Money { get; set; }
        public double? Proportion { get; set; }
        public DateTime DateTime { get; set; }
        public int Amount { get; set; }
    }
}