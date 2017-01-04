using System;
using DataPersist;
//using AssemblyCSharp.Helper;

namespace LilyHeart
{
	public class RequestFriendMessage:PlayerMessage
	{
		public RequestFriendMessage (UserData userData):base(userData)
		{
			Title="REQUEST_FRIEND_PROMPT_MESSAGE";
			//Title = LocalizeHelper.Translate("REQUEST_FRIEND_PROMPT_MESSAGE");
		}
		
		public override void ProcessMessage()
		{
			User.Singleton.AddFriend(sender.UserId);
		}
		
		public override bool Equals (object obj)
		{
			if(obj is RequestFriendMessage)
			{
				RequestFriendMessage requestFriendMessage=obj as RequestFriendMessage;
				return this.sender.UserId==requestFriendMessage.sender.UserId;
			}
			return base.Equals (obj);
		}
	}
}

