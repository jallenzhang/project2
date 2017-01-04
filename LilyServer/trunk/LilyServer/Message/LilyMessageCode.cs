using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LilyServer
{
    public enum LilyMessageCode:byte
    {
        AddRobotInGame=255,
        RobotAction=254,
        RoomTypeChanged=253
    }
}
