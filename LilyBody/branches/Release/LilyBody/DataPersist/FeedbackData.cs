using System;

namespace DataPersist
{
	[Serializable]
	public class FeedbackData
	{
		#region Constructor
        public FeedbackData()
		{
		}
		#endregion
		
		#region public Properties

		public string UserId {get;set;}

        public string Content { get; set; }
   
		#endregion
	}
}

