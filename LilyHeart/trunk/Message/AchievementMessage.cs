using System;
using UnityEngine;

namespace LilyHeart
{
	public class AchievementMessage:Message
	{
		public long chip;
        public int level;
        public long levelExp;
        public bool hasAward;
		
		public byte AchievementNumber {get;private set;}
		
		public AchievementMessage (byte achievementNumber)
		{
			AchievementNumber=achievementNumber;
			User.Singleton.UserData.Achievements|=1L<<(achievementNumber-1);
			this.Title="Achievement Title";
		}
		
		public AchievementMessage(byte achievementNumber,string content):this(achievementNumber)
		{
			if(!string.IsNullOrEmpty(content))
			{
				string[] values=content.Split('|');
				this.chip=Convert.ToInt64(values[0]);
				this.level=Convert.ToInt32(values[1]);
				this.levelExp=Convert.ToInt64(values[2]);
				this.hasAward=true;
			}
		}
		
		public override void ProcessMessage ()
		{
			if(this.hasAward)
			{
				User.Singleton.UserData.Chips=chip;
				User.Singleton.UserData.Level=level;
				User.Singleton.UserData.LevelExp=levelExp;
				User.Singleton.UserInfoChanged();
			}
		}

	}
}

