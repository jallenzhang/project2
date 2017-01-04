using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LilyServer.Helper
{
    internal class DateHelper
    {
        public static DateTime GetNow()
        {
            return DateTime.UtcNow.AddHours(8);
        }
    }
}
