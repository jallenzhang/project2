using System;
using DataPersist;

namespace LilyHeart
{
	public class SendChipMessage:PlayerMessage
	{
		public SendChipMessage(UserData userData,string content):base(userData)
		{
			string[] values=content.Split('|');
			
			string str=string.Empty;
			
			long currentSendChips = Convert.ToInt64(values[0]);
			if (currentSendChips > 1000000)
				str=string.Format("{0:N1}M",(currentSendChips/1000000.0f));
			else if(currentSendChips>=1000)
			    str=string.Format("{0:N1}K",(currentSendChips/1000.0f));
			else
				str=string.Format("{0}",(currentSendChips));
			
			this.Title=str;
			this.Content=sender.NickName+"send you"+values[0];
			if(values.Length>1)
			{
				User.Singleton.UserData.Chips=Convert.ToInt64(values[1]);
				if (User.Singleton.UserData.NoSeat>-1&&Room.Singleton.PokerGame.TableInfo!=null) {
					PlayerInfo curPlayer=Room.Singleton.PokerGame.TableInfo.GetPlayer (User.Singleton.UserData.NoSeat);
					if(curPlayer!=null)
						curPlayer.Chips=Convert.ToInt64(values[1]);
				}
			}
		}
	}
}

