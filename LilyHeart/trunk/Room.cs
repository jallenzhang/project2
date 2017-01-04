using System;
using DataPersist;
using System.Collections.Generic;

namespace LilyHeart
{
	public class Room
	{
		private static Room room;
		
		public static Room Singleton {
			get
			{
				if(room==null)
				{
					room=new Room();
				}
				return room;
			}
		}
		public RoomData RoomData {get;set;}
		public PokerGame PokerGame {get;set;}
		public bool OnlyFriend {get;set;}
		
		private Room ()
		{
			PokerGame=new PokerGame();
		}
	}
}

