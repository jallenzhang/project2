using System;
using System.Collections.Generic;

namespace LilyHeart
{
	public class PlayersShowCardsFinishedNotification:Notification
	{
		public Dictionary<int,string> plays;
		public PlayersShowCardsFinishedNotification (Dictionary<int,string> play)
		{
			plays=play;
		}
	}
}

