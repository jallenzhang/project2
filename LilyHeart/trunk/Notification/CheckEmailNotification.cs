using System;

namespace LilyHeart
{
	public class CheckEmailNotification:Notification
	{
		public bool IsSuccess {get; private set;}
		public CheckEmailNotification (bool isSuccess)
		{
			this.Target=TargetType.LauchTable|TargetType.Room|TargetType.Game;
			this.IsSuccess=isSuccess;
		}
	}
}

