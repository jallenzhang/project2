
using System;

namespace DataPersist
{
	[Serializable]
	public class User
	{
		#region Constructor
		public User ()
		{
		}
		#endregion
		
		#region public Properties
		public string NickName
		{
			get;
			protected set;
		}
		
		public string UserId
		{
			get;
			protected set;
		}
		
		public string Password
		{
			get;
			protected set;
		}
		
		public string RoomId
		{
			get;
			set;
		}
		
		public byte Status
		{
			get;
			set;
		}
		
		//user's total money
		public long Money
		{
			get;
			protected set;
		}
		
		public long Exp
		{
			get;
			set;
		}
		
		public byte UserType
		{
			get;
			protected set;
		}
		
		public byte Avator
		{
			get;
			set;
		}
		
		public byte BackgroundType
		{
			get;
			protected set;
		}
		
		public byte LivingRoomType
		{
			get;
			protected set;
		}
		
		public long HandsWon
		{
			get;
			private set;
		}
		
		public long HandsPlayed
		{
			get;
			protected set;
		}
		
		//the most bouns he/she won
		public long BiggestWin
		{
			get;
			protected set;
		}
		
		public float WinRatio
		{
			get;
			protected set;
		}
		
		public string BestHand
		{
			get;
			set;
		}
		
		#endregion
	}
}

