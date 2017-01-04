using System;
using System.Runtime.InteropServices;

namespace AssemblyCSharp
{
	public static class BaiduIOSBinding
	{
		[DllImport("__Internal")]
		public static extern void bdStartSession(string appId,string channelId);
		
		[DllImport("__Internal")]
		public static extern void bdLogEventWithParameters(string eventId,string eventLabel);
	}
}

