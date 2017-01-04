using System;
using DataPersist;

namespace LilyHeart
{
	public class ErrorNotification:Notification
	{
		public ErrorCode ErrorCode {get;set;}
		public ErrorNotification (ErrorCode errorCode)
		{
			this.ErrorCode=errorCode;
			this.Target=TargetType.Empty|TargetType.LauchTable|TargetType.Room|TargetType.Game;
		}
	}
}

