using System;
using DataPersist;

namespace LilyHeart
{
	public class SystemNoticeNotification:Notification
	{
		public string m_notice = string.Empty;
		public string m_nickName = string.Empty;
		public StatusTipsType m_mytype;
		public object [] m_myparams;
		public WorldMessageType m_kmMessageType;
		public SystemNoticeNotification (string notice,string nickName,StatusTipsType mytype,object [] myparams,WorldMessageType kmMessageType)
		{
			this.Target = TargetType.Room;
			this.m_notice = notice;
			this.m_nickName = nickName;
			this.m_mytype = mytype;
			this.m_myparams = myparams;
			this.m_kmMessageType = kmMessageType;
		}
	}
}

