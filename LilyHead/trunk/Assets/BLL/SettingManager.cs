using System;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;
using LilyHeart;

namespace AssemblyCSharp
{
	[XmlRoot("Setting")]
	public class SettingManager
	{
		private const string SHAKE="Shake";
		private const string ANIMATION="Animation";
		private const string SOUND="Sound2";
		private const string MUSIC="Music";
		private const string CHAT_BUBBLE="ChatBubble";
		private const string POKER_POINTER="PokerPointer";
		private const string CALL_IN_ROOM="CallInRoom";
		private const string FRIEND_ACTIVITY_NOTIFICATION = "FriendActivityNotification";
		private const string SYSTEM_NOTIFICATION="SystemNotification";
		
		private static SettingManager settingManager;
		public static SettingManager Singleton 
		{
			get { 
				if(settingManager==null)
				{
					settingManager=new SettingManager();
				}
				return settingManager;
			}
		} 
		
		private SettingManager ()
		{
		}
		
		public bool Shake {
			get { return PlayerPrefs.HasKey(SHAKE)?bool.Parse(PlayerPrefs.GetString(SHAKE)):true;} 
			set { PlayerPrefs.SetString(SHAKE,value.ToString());}
		}
		public bool Animation {
			get { return PlayerPrefs.HasKey(ANIMATION)?bool.Parse(PlayerPrefs.GetString(ANIMATION)):true;} 
			set { PlayerPrefs.SetString(ANIMATION,value.ToString());}
		}
		public bool Sound {
			get { return PlayerPrefs.HasKey(SOUND)?bool.Parse(PlayerPrefs.GetString(SOUND)):true;} 
			set { PlayerPrefs.SetString(SOUND,value.ToString());}
		}
		public bool Music {
			get { return PlayerPrefs.HasKey(MUSIC)?bool.Parse(PlayerPrefs.GetString(MUSIC)):true;} 
			set { PlayerPrefs.SetString(MUSIC,value.ToString());}
		}
		public bool ChatBubble {
			get { return PlayerPrefs.HasKey(CHAT_BUBBLE)?bool.Parse(PlayerPrefs.GetString(CHAT_BUBBLE)):true;} 
			set { PlayerPrefs.SetString(CHAT_BUBBLE,value.ToString());}
		}
		public bool PokerPointer {
			//remove PokerPointer Limit for UserType.Guest 
			//get { return PlayerPrefs.HasKey(POKER_POINTER)?bool.Parse(PlayerPrefs.GetString(POKER_POINTER)):User.Singleton.UserData.UserType == DataPersist.UserType.Guest ? false : true;} 
			get { return PlayerPrefs.HasKey(POKER_POINTER)?bool.Parse(PlayerPrefs.GetString(POKER_POINTER)):true;} 
			set { PlayerPrefs.SetString(POKER_POINTER,value.ToString());}
		}
		
		public bool CallInRoom {
			get { return PlayerPrefs.HasKey(CALL_IN_ROOM)?bool.Parse(PlayerPrefs.GetString(CALL_IN_ROOM)):true;} 
			set { PlayerPrefs.SetString(CALL_IN_ROOM,value.ToString());}
		}
		
		public bool FriendActivityNotification {
			get { return User.Singleton.UserData.FriendNotify;} 
			set { PlayerPrefs.SetString(FRIEND_ACTIVITY_NOTIFICATION,value.ToString());}
		}
		
		public bool SystermNotification {
			get { return User.Singleton.UserData.SystemNotify;} 
			set { PlayerPrefs.SetString(SYSTEM_NOTIFICATION,value.ToString());}
		}
		
		public void SaveFile()
		{
			MusicChanged();
			PlayerPrefs.Save();
		}
		
		public void MusicChanged()
		{
			if(this.Music)
			{
				MusicManager.Singleton.PlayBgMusic();
			}
			else
			{
				if(MusicManager.Singleton.BgAudio.isPlaying)
				{
					MusicManager.Singleton.BgAudio.Stop();
					MusicManager.Singleton.BgAudio.clip=null;
				}
			}
		}
		
		public void DeleteAll()
		{
			PlayerPrefs.DeleteKey(SHAKE);
			PlayerPrefs.DeleteKey(ANIMATION);
			PlayerPrefs.DeleteKey(SOUND);
			PlayerPrefs.DeleteKey(MUSIC);
			PlayerPrefs.DeleteKey(CHAT_BUBBLE);
			PlayerPrefs.DeleteKey(POKER_POINTER);
			PlayerPrefs.DeleteKey(CALL_IN_ROOM);
			PlayerPrefs.DeleteKey(FRIEND_ACTIVITY_NOTIFICATION);
			PlayerPrefs.DeleteKey(SYSTEM_NOTIFICATION);
		}
	}
}

