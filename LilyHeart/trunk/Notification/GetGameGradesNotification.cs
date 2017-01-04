using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LilyHeart
{
    public class GetGameGradesNotification:Notification    
    {
        public GetGameGradesNotification()
        {
            this.Target = TargetType.Room;
        }
    }
}
