using System;
using System.Collections.Generic;

namespace DataPersist
{
	public class UserData : PlayerInfo
	{
		#region Constructor
		public UserData ()
		{
		}
		#endregion
		
		#region public Properties
		public string NickName {get;set;}

        public string Mail { get; set; }

        public string UserName { get; set; }
		
		public string UserId {get;set;}
		
		public string Password {get;set;}

        public string RoomId { get; set; }
		
		public UserStatus Status {get;set;}
		
		//user's total money
		public long Money {get;set;}
		

		
		public long Exp {get;set;}
		
		public UserType UserType {get;set;}
		
		public byte BackgroundType {get;set;}
		
		public RoomType LivingRoomType {get;set;}
		
		public long HandsWon { get;set;}
		
		public long HandsPlayed { get;set;}
		
		//the most bouns he/she won
		public long BiggestWin {get;set;}
		
		public float WinRatio { get;set;}

        public int BestHandValue { get; set; }

		public string BestHand {get;set;}
		
		public bool SystemNotify {get;set;}
		
		public bool FriendNotify {get;set;}
		
		public List<PetType> PetTypes {get;set;}

        // STATES
        public bool IsSitting { get; set; } // Sitting

        public int CareerWins { get; set; }

        //public long Chips { get; set; }
        //public int Level { get; set; }
        //public long LevelExp { get; set; }

        public RoomType OwnRoomTypes { get; set; }
        public int TotalGame { get; set; }

        public int Wins { get; set; }

        public DeviceType DeviceType { get; set; }

        public string DeviceToken { get; set; }

        public long Achievements { get; set; }

        public bool HasBuy { get; set; }

        public string Awards { get; set; }

        public float AwardAdds { get; set; }

        public int ExpAdds { get; set; }

        public float WinAdds { get; set; }

        public int VIP { get; set; }

        public bool Jade { get; set;}

        public bool LineAge{ get;set;}


        public int CareerTotalPlayed { get; set; }

		#endregion
	}
}

