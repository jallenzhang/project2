using System;

namespace LilyHeart
{
	public class FindPasswordNotification:Notification
	{
		public bool IsSuccess {get;set;}
		public FindPasswordNotification (bool isSuccess)
		{
			this.Target=TargetType.LauchTable;
			this.IsSuccess=isSuccess;
		}
	}
}

