using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace LilyServer.Helper
{
    public class TimerHelper
    {
        public static void SetTimeout(double interval, Action action) {
            if (interval == 0d) interval = 1000d;
            Timer time = new Timer(interval);
            time.Elapsed+= delegate(object sender,System.Timers.ElapsedEventArgs e)
             {
                 time.Enabled = false;
                 action();
             };
            time.AutoReset = false;
            time.Enabled = true;
        }
    }
}
