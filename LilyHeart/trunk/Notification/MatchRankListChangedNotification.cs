using System;

namespace LilyHeart
{
	public class MatchRankListChangedNotification:Notification
	{
		public byte[] RankList {get;set;}
		public MatchRankListChangedNotification (byte[] rankList)
		{
			this.Target=TargetType.Match;
			this.RankList=rankList;
		}
	}
}

