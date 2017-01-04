using System;
using UnityEngine;

namespace LilyHeart
{
	public static class HelpManager
	{
		private const string MICRO_BLOG="http://weibo.com/kxpoker";
		private const string WEBSITE="http://www.gov.cn";
		
		public static void Feedback(string suggestion)
		{
			PhotonClient.Singleton.Feedback(suggestion);
		}
		
		public static void OpenWebSite()
		{
			Application.OpenURL(WEBSITE);
		}
		
		public static void OpenMicroBlog()
		{
			Application.OpenURL(MICRO_BLOG);
		}
	}
}

