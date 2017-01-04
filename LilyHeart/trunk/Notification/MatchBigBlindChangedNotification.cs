using System;

namespace LilyHeart
{
	public class MatchBigBlindChangedNotification:Notification
	{
		public int Amnt{get;set;}
		public MatchBigBlindChangedNotification (int amnt)
		{
			this.Target=TargetType.Match;
			this.Amnt=amnt;
		}
	}
}

