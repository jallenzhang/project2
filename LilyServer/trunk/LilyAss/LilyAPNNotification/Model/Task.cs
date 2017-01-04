using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LilyAPNNotification.Model
{
    public class Task
    {
        public List<string> Params { get; set; }
        public bool IsEnded { get; set; }

        public Task() {
            IsEnded = false;
            Params = new List<string>();
        }

        public void Excute()
        {
            string deviceToken = Params[0];
            string msg = Params[1];
            Excute(deviceToken, msg);
        }

        private void Excute(string deviceToken, string msg)
        {
            //APNHelper apnHelper = new APNHelper();
            //apnHelper.DeviceToken = deviceToken;
            //apnHelper.AddNotification(msg);

            //apnHelper.Service.CleanUp();
            //IsEnded = true;
        }
    }
}
