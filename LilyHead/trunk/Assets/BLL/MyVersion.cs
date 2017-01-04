using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
	public enum DevicePlatform
	{
		Normal,
		BreakOut,
	}
	
	
	public static class MyVersion {
	
		public const string CurrentVersion = "1.3.0";
		
		public const string BuildId = "(1000)";
		
		public const string ChannelId = "test";
		
		public const DevicePlatform CurrentPlatform = DevicePlatform.BreakOut;
		
	}
}
