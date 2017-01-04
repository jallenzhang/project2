using System;
using DataPersist;

namespace LilyHeart
{
	public class PlayerLeavedNotification:Notification
	{
		public int NoSeat{get;private set;}
		public bool click{get;private set;}
		public PlayerLeaveType gameleaveType{get;private set;}
		public PlayerLeavedNotification (int Noseat,bool isclick,PlayerLeaveType leavetype)
		{
			NoSeat=Noseat;
			click=isclick;
			gameleaveType=leavetype;
		}
	}
}

