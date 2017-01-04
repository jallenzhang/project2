using System;

namespace LilyHeart
{
	public class PlayerWonPotImproveNotification:Notification
	{
		public int potId;
		public int[] winner;
		public long[] winamnt;
		public int[] attachedplayer;
		public PlayerWonPotImproveNotification (int potid,int[] winners,long[] winamnts,int[] attachedplayers)
		{
			potId=potid;
			winner=winners;
			winamnt=winamnts;
			attachedplayer=attachedplayers;
		}
	}
}

