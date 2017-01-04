using System;

namespace DataPersist
{
	[Serializable]
	public class FriendData
	{
		#region Constructor
        public FriendData()
		{
		}
		#endregion
		
		#region public Properties

		public string UserA {get;set;}

        public string UserB { get; set; }
   
		#endregion
	}
}

