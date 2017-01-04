using System;
using System.Collections.Generic;
using DataPersist;
using UnityEngine;
using System.Threading;
using System.Xml.Serialization;
using System.IO;

namespace LilyHeart
{
	public enum KindOfVersion
	{
		Basic,
		Ultimate
	}
	public enum WorldMessageType
	{
		SystemNotice,
		WorldSpeak,
		StatusTip
	}
	public class LilyBoardMessages
	{
		public string Messages;
		public WorldMessageType kmMessagesType;
	}
	public enum AppType:byte
	{
		Normal,
		NinetyOne
	}
	
	public class GlobalManager
	{	
		private const int DURATION=3600*3;
		
		private static GlobalManager globalManager;
		
		public static GlobalManager Singleton
		{
			get
			{
				if(globalManager==null)
				{
					globalManager=new GlobalManager();
				}
				return globalManager;
			}
		}
		
		private bool isTiming=false;
		private Dictionary<string,Duration> durations;
		private readonly HashSet<string> languages;
		
		public List<UserData> SearchUsers {get;set;}
		public Queue<Message> Messages {get;set;}
		public Queue<Notification> Notifications {get;set;}
		public Achievement[] Achievements {get;private set;}
		public Queue<LilyBoardMessages> BoardMessages{get;set;}
		public string CurrentVersion { get; set; }
		public long AwardChip {get;set;}
		public LilyBoardMessages CurrentBoardMessage {get;set;}
		public System.Timers.Timer reconnectTimer;
		public string ParamEmail {get;set;}
		public string ParamPassword {get;set;}
		public bool InitGuestSignIn {get;set;}
		public KindOfVersion version {get;set;}
		public AppType ApplicationType {get;set;}
		public DeviceInfo deviceInfo {get;set;}
		public bool bIsSubmitDeviceInfo {get;set;}
		public bool isFastStart {get;set;}
		public string ClientChannelId {get;set;}

        public GameGrade[] GameGrades { get; set; }

	    public GlobalManager ()
		{
			ParamEmail="";
			ParamPassword="";
			InitGuestSignIn = false;
			deviceInfo = null;
			bIsSubmitDeviceInfo = false;
			isFastStart = false;
			languages=new HashSet<string>();
			languages.Add("English");
			languages.Add("Chinese");
			SearchUsers=new List<UserData>();
			Messages=new Queue<Message>();
			Notifications=new Queue<Notification>();
			BoardMessages = new Queue<LilyBoardMessages>();
			reconnectTimer=new System.Timers.Timer(5000d);
			reconnectTimer.Elapsed+=Reconnect;
			LoadAchievementFile();
		}
		
		public void StartReconnect()
		{
			reconnectTimer.Enabled=true;
		}
		
		public void Reconnect(object sender,System.Timers.ElapsedEventArgs e)
		{
			if(!PhotonClient.Singleton.IsConnected)
			{
				PhotonClient.Singleton.ReconnectOperation();
			}
			else
			{
				reconnectTimer.Enabled=false;
			}
		}
		
		public static void Log(object message)
		{
			if(Debug.isDebugBuild)
			{
				Debug.Log(message);
			}
		}
		
		public void GetClientVersion()
		{
			PhotonClient.Singleton.GetClientVersion();
		}
		
		public void PushInBoardMessage(LilyBoardMessages message)
		{
			if (BoardMessages.Count > 30)
				BoardMessages.Dequeue();
			if(message!=null)
				BoardMessages.Enqueue(message);
		}
		
		private void LoadAchievementFile()
		{
			try
			{
				string language=languages.Contains(Application.systemLanguage.ToString())?Application.systemLanguage.ToString():"Chinese";
				Debug.Log ("Achievement File:"+language);
				TextAsset textAsset=Resources.Load("Achievement/"+language) as TextAsset;
				MemoryStream memoryStream=new MemoryStream(textAsset.bytes);
				XmlSerializer xmlSerializer=new XmlSerializer(typeof(Achievement[]),new XmlRootAttribute("Achievements"));
				this.Achievements=xmlSerializer.Deserialize(memoryStream) as Achievement[];
				memoryStream.Close();
			}
			catch
			{
				Debug.LogError("Load Achievement File Error");
			}
		}
		
		public void StartTiming()
		{
			if((User.Singleton.UserData.Achievements&(1L<<9))==0)
			{
				durations=FileIOHelper.ReadDurationFile();
				foreach(Duration duration in durations.Values)
				{
					if(duration.DateTime!=DateTime.Today)
					{
						duration.Seconds=0;
						duration.DateTime=DateTime.Today;
					}
				}
				if(!durations.ContainsKey(User.Singleton.UserData.UserId))
				{
					durations[User.Singleton.UserData.UserId]=new Duration{Seconds=0,DateTime=DateTime.Today};
				}
				Thread thread=new Thread(new ThreadStart(()=>{
					isTiming=true;
					while(isTiming)
					{
						durations[User.Singleton.UserData.UserId].Seconds++;
						if(durations[User.Singleton.UserData.UserId].Seconds>=DURATION)
						{
							PhotonClient.Singleton.Achieve(10);
							isTiming=false;
						}
						Thread.Sleep(1000);
					}
				}));
				thread.IsBackground=true;
				thread.Start();
			}
		}
		
		public void EndTiming()
		{
			if(isTiming)
			{
				isTiming=false;
				FileIOHelper.SaveDurationFile(durations);
				//string content=duration.ToString()+","+DateTime.Today;
				//FileIOHelper.WriteFile(FileType.Duration,content);
			}
		}
	}
}

