using System;
using DataPersist;

namespace LilyHeart
{
	public class AddFriendMessage:PlayerMessage
	{
		public AddFriendMessage (UserData userData):base(userData)
		{
			this.Title="Add Friend";
			this.Content="You are my friend now!";
		}
	}
}

