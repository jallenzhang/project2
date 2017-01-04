using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataPersist
{
     public class RobotInfo:UserData
    {
         public int Alive { get; set; }
         public int QueueIndex { get; set; }
         public RobotInfo() {
             //for test
             this.Name = "Robot1";
             this.NickName = "Robot1";
             this.MoneyInitAmnt = 100000;
             this.MoneySafeAmnt = 100000;
             this.NoSeat = -1;
         }
    }
}
