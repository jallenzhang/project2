using System;
using DataPersist;

namespace LilyHeart
{
	public class GameSitDownNotification:Notification
	{
		public TypeState gameType;
		public GameSitDownNotification (TypeState type)
		{
			gameType=type;
		}
	}
}

