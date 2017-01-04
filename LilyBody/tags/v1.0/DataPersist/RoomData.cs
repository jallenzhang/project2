using System;
using System.Collections.Generic;

namespace DataPersist
{
	[Serializable]
	public class RoomData
	{
		public RoomData ()
		{
		}
		
		public string RoomId {get;set;}
		public RoomType RoomType {get;set;}
		public List<UserData> Users{get;set;}
        public GameType GameType { get; set; }
	}
}

