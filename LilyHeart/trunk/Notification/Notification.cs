using System;

namespace LilyHeart
{
	public enum TargetType
	{
		LauchTable=1,
		Room=2,
		Game=4,
		Empty=8,
		Match=16
	}
	
	public abstract class Notification
	{
		public TargetType Target {get;protected set;}
		public Notification ()
		{
		}
	}
}

