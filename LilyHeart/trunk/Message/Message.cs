using System;

namespace LilyHeart
{
	public abstract class Message
	{
		public string Title {get;protected set;}
		public string Content {get;protected set;}
		
		public Message ()
		{
		}
		
		public virtual void ProcessMessage()
		{
		}
	}
}

