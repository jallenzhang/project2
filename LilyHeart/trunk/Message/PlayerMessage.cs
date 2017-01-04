using System;
using DataPersist;

namespace LilyHeart
{
	public class PlayerMessage:Message
	{
		public string NickName {get {return sender.NickName;}}
		public byte Avator {get {return sender.Avator;}}
		public int Level {get {return sender.Level;}}
		public HonorType Honor {get;protected set;}
		public long Chips {get {return sender.Chips;}}
		
		protected UserData sender;
		
		public PlayerMessage (UserData userData)
		{
			this.sender=userData;
			this.Honor=HonorHelper.GetHonorRecuise(userData,HonorType.Citizen);
		}
		
		public override void ProcessMessage()
		{
		}
	}
}

