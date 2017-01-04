using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebNotifications.Models.Custom
{
    public class PartnerExtern 
    {
        public Partner Partner_P { get; set; }
        public string Redirect_url { get; set; }
    }
}