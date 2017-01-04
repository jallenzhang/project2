using UnityEngine;
using System.Collections;

namespace LilyHeart
{
	public class KickedByVIPMessage : Message {
	
		public KickedByVIPMessage(string nickName)
		{
			this.Title = "kicked by vip";
			this.Content = nickName;
		}
	}
}
