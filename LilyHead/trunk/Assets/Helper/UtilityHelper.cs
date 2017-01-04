using UnityEngine;
using System.Collections;
using AssemblyCSharp.Helper;
using AssemblyCSharp;
using System;
using DataPersist;
using System.Text.RegularExpressions;
using LilyHeart;

namespace AssemblyCSharp
{
	public class UtilityHelper {
		public static event Action MaskTableCloseEvent;
		public static event Action MaskTableTryAgainEvent;
		public static event Action MaskTableAdditionalBehaviourEvent;
		
		public static event Action<bool> PreConditionEvent;
		
		public static event Action<bool> ChooseFirstOneEvent;
		
		public static event Action ResetTimerEvent;
		public static event Action TimerStartEvent;
		public static event Action TimerEndEvent;
		
		public static event Action RoomDataChangedEvent;
		
		
		
		public static void RoomDataChanged()
		{
			if (RoomDataChangedEvent != null)
				RoomDataChangedEvent();
		}
		
		public static void ResetTimer()
		{
			if (ResetTimerEvent != null)
				ResetTimerEvent();
		}
		
		public static void TimerStart()
		{
			if (TimerStartEvent != null)
				TimerStartEvent();
		}
		
		public static void TimerEnd()
		{
			if (TimerEndEvent != null)
				TimerEndEvent();
		}
		
		public static void PreConditionFinished(bool result)
		{
			if (PreConditionEvent != null)
			{
				PreConditionEvent(result);
			}
		}
		
		public static void ChooseFirstOneFinished(bool result)
		{
			if (ChooseFirstOneEvent != null)
			{
				ChooseFirstOneEvent(result);
			}
		}
		
		
		public static void CloseMaskingTable()
		{
			if (MaskTableCloseEvent != null)
			{
				MaskTableCloseEvent();
			}
		}
		
		public static void MaskTableTryAgain()
		{
			if (MaskTableTryAgainEvent != null)
			{
				MaskTableTryAgainEvent();
			}
		}
		
		public static void MaskTableAdditionalBehaviour()
		{
			if (MaskTableAdditionalBehaviourEvent != null)
			{
				MaskTableAdditionalBehaviourEvent();
			}
		}
		
		public static bool checkEmain(string email)
		{
			email = email.Trim();
			//the lenght more than 30 or email is nil
			if(email.Equals(string.Empty)||email.Length>30 || email.Length < 6) 
			{
				PopUpErrorDialog(LocalizeHelper.Translate("LOGIN_MAIL_LENGTH_ERROR"));
				return false;
			}
			
			int index1,index2;
			//index1="@" index2="."
			index1=index2=0;
			
			char[] values=email.ToCharArray();			
			for(int i=0;i<email.Length-1;i++)
			{
				if(values[i]=='@')
				{
					index1++;
					if (i == 0)
					{
						PopUpErrorDialog(LocalizeHelper.Translate("LOGIN_MAIL_FORMATTER_ERROR"));
						return false;
					}
					continue;
				}
				else if(values[i]=='.')
				{
					index2++;
					if (index1 == 0)
					{
						PopUpErrorDialog(LocalizeHelper.Translate("LOGIN_MAIL_FORMATTER_ERROR"));
						return false;
					}
					
					continue;
				}
				
				int iascii = StringHelper.Asc(values[i]);
				
				//0~9, a~z, A~Z
				if ((iascii >= 48 && iascii <=57)
					|| (iascii >= 65 && iascii <= 90)
					|| (iascii >= 97 && iascii <= 122))
				{
					continue;
				}
				else
				{
					PopUpErrorDialog(LocalizeHelper.Translate("LOGIN_MAIL_FORMATTER_ERROR"));
					return false;
				}
			}
			
			if (index1 == 0 || index1 > 1 || index2 == 0 || index2 > 1)
			{
				PopUpErrorDialog(LocalizeHelper.Translate("LOGIN_MAIL_FORMATTER_ERROR"));
				return false;
			}
			
			return true;
		}
		
		public static void  PopUpErrorDialog(string errorContent)
		{
			Debug.Log(errorContent);
	#if UNITY_IPHONE
			EtceteraBinding.showAlertWithTitleMessageAndButton(LocalizeHelper.Translate("DIALOG_TITLE_ERROR"), errorContent, LocalizeHelper.Translate("DIALOG_BUTTON_OK"));
	#endif
			
	#if UNITY_ANDROID
			EtceteraAndroid.showAlert(LocalizeHelper.Translate("DIALOG_TITLE_ERROR"), errorContent, LocalizeHelper.Translate("DIALOG_BUTTON_OK"));
	#endif
		}
		
		//check user password 
		public static  bool checkPwd(string pwd)
		{
			//password lenght
			if(pwd.Length<6||pwd.Length>12) 
			{
				PopUpErrorDialog(LocalizeHelper.Translate("LOGIN_PASSWORD_LENGTH_ERROR"));
				return false;
			}
			
			char[] values=pwd.ToCharArray();
			
			for(int i=0;i<pwd.Length;i++)
			{
				bool b=false;
				//A~Z
				if(values[i]>='A'&&values[i]<='Z')
					b=true;
				//a~z
				else if(values[i]>='a'&&values[i]<='z')
					b=true;
				//0~9
				else if(values[i]>='0'&&values[i]<='9')
					b=true;
				if(!b)
				{
					PopUpErrorDialog(LocalizeHelper.Translate("LOGIN_PASSWORD_FORMATTER_ERROR"));
					return false;
				}
			}
			
			return true;
		}
// check nick name length
		public static string checkNickName(string nick,int nNeedLimit)
		{
			int nLimit =14;
			if(nNeedLimit != 0 )
				nLimit = nLimit;
			Debug.Log("nick length:"+nick.Length);
			string resultString = string.Empty;
			int nLength = 0;
			Regex regChina = new Regex("^[^\x00-\xFF]");
	        //Regex regEnglish = new Regex("^[a-zA-Z]");
			string tmp = string.Empty;
			for(int i = 0;i<nick.Length;i++)
			{
				tmp = nick.Substring(i,1);
		        if (regChina.IsMatch(tmp))
		        {
					if(nLength+2<=nLimit)
					{
						nLength +=2;
						resultString += tmp;
					}
					else
						break;
		        }
				else
				{
					if(nLength+1<=nLimit)
					{
						nLength +=1;
						resultString += tmp;
					}
					else
						break;
				}
			}
			return resultString;
		}
			
		public static void AutoLogin()
		{
			Debug.LogWarning ("AutoLogin:"+DateTime.Now.ToString());
			string account = string.Empty;
			account = FileIOHelper.ReadFile(FileType.Account);
			string[] infos = account.Split('|');
			Debug.Log("the account is: " + account);
			ResetFlags();
			if (Convert.ToInt32(infos[2]) == (int)UserType.Guest)
			{
				User.Singleton.UserData.NickName=string.Empty;
				User.Singleton.UserData.Mail = infos[0];
				User.Singleton.UserData.Password = string.Empty;
				User.Singleton.UserData.UserType = UserType.Guest;
				User.Singleton.UserData.DeviceToken = DeviceTokenHelper.myDeviceToken;
				PhotonClient.Singleton.Login(User.Singleton.UserData);
			}
			else
			{
				User.Singleton.UserData.Mail = infos[0];
				User.Singleton.UserData.Password = infos[1];
				User.Singleton.UserData.DeviceToken = DeviceTokenHelper.myDeviceToken;
//				PhotonClient.Singleton.Login(User.Singleton.UserData);
				User.Singleton.Login(infos[0],infos[1],DeviceTokenHelper.myDeviceToken);
			}
			
		}
		
		public static UserData ReadAccount()
		{
			UserData user=new UserData();
			
			string account = string.Empty;
			account = FileIOHelper.ReadFile(FileType.Account);
			Debug.Log(account);
			if(!string.IsNullOrEmpty(account))
			{
				string[] infos = account.Split('|');
				user.Mail = infos[0];
				user.Password = infos[1];
				user.UserType = (UserType)Convert.ToInt32(infos[2]);
				user.DeviceToken = DeviceTokenHelper.myDeviceToken;
			}
			else
			{
				user=null;
			}
			return user;
		}
		
		public static bool CanAutoLogin()
		{
			string account = string.Empty;
			account = FileIOHelper.ReadFile(FileType.Account);
				
			if (string.IsNullOrEmpty(account)||account.Split('|').Length>3)
			{
				return false;
			}
			return true;
		}
		
		/// <summary>
		/// Resets the flags.
		/// </summary>
		public static void ResetFlags()
		{
			User.bShowFriend_Flag = false;
			User.Singleton.MessageOperating = false;
			User.bShowGameSetting_Flag = false;
		}
		
		public static void LoadResources()
		{
			LoadLanguage();
			LoadHonor();
		}
		
		private static void LoadLanguage()
		{
			Debug.Log("In function LoadLanguage");
			string languageFile = Application.systemLanguage.ToString();// + ".xml";
			TextAsset languageTextAsset = null;
			string filePath = "Languages/"+languageFile;
			
			languageTextAsset = (TextAsset)Resources.Load(filePath);
			
			if (languageTextAsset == null)
			{
				languageTextAsset = (TextAsset)Resources.Load("Languages/Chinese.xml");
				
				if (languageTextAsset == null)
					return;
			}
				
			LocalizeHelper.CurrentLanguage = languageTextAsset;
			LocalizeHelper.LoadTranslation(ref languageFile, languageTextAsset);
		}
		
		private static void LoadHonor()
		{
			TextAsset honorAsset = (TextAsset)Resources.Load("Honors");
			
			if (honorAsset == null)
			{
				Debug.Log("Load honor.xml failed");
				return;
			}
			
			HonorHelper.LoadXmlFile(honorAsset);
		}
		
		public static void ShowAlertDialog(string title,string message,string buttontitle)
		{
		#if UNITY_IPHONE
					EtceteraBinding.showAlertWithTitleMessageAndButton(title,message,buttontitle);
					Debug.Log("AlertDialog");
		#endif
		
		#if UNITY_ANDROID
					EtceteraAndroid.showAlert(title,message,buttontitle);
		#endif
		}
		public static void ShowWebByUrl(string AboutUrl)
		{
		#if UNITY_IPHONE
			EtceteraBinding.showWebPage(AboutUrl);
		#endif
				
		#if UNITY_ANDROID
				EtceteraAndroid.showWebView(AboutUrl);
		#endif
		}
		public static void OnUpgradeFinish(bool success)
		{
			PhotonClient.RegisterOrUpgradeEvent -= OnUpgradeFinish;
			if (!success)
			{
				//ShowAlertDialog(LocalizeHelper.Translate("DIALOG_TITLE_ERROR"), PhotonClient.Singleton.ErrorMessage, LocalizeHelper.Translate("DIALOG_BUTTON_OK"));
			}
		}
		public static string  GetDeviceVersion()
		{
			string result = string.Empty;
			#if UNITY_IPHONE
			result = EtceteraBinding.GetDeviceType();
			Debug.Log("GetDeviceType:"+result);
			#endif
			return result;
		}
		public static string  GetOSVersion()
		{
			string result = string.Empty;
			#if UNITY_IPHONE
			result = EtceteraBinding.GetOSVersion();
			result = "IOS" + result;
			Debug.Log("GetOSVersion:"+result);
			#endif
			return result;
		}
	    public static string GetChannelId()
		{
			string result = string.Empty;
			#if UNITY_IPHONE
			result = EtceteraBinding.GetChannelId();
			Debug.Log("GetChannelId:"+result);
			#endif
			return result;
		}
		public static void setReciveDeviceInfo(String gameObjectName,String methodNameDeviceType,String methodNameOSVersion,String methodNameDeviceToken)
		{
			#if UNITY_ANDROID
			EtceteraAndroid.setReciveDeviceInfo(gameObjectName,methodNameDeviceType,methodNameOSVersion,methodNameDeviceToken);
			#endif
		}
		
		public static int CalculationMessageLeghtIncludeChinese(string message)
		{
			int lenght=0;
			string submassage=null;
			for(int i=0;i<message.Length;i++)
			{
				submassage=message.Substring(i,1);
				Match  mInfo=Regex.Match(submassage,@"[\u4e00-\u9fa5]");
				if(mInfo.Success)
				{
					lenght+=2;
				}
				else
				{
					lenght++;
				}
			}
			
			return lenght;
		 
		}
		
		public static string GetHonorName(UserData user)
		{
			string result=string.Empty;
			
			HonorType currentHonor = HonorHelper.GetHonorRecuise(user, HonorType.Citizen);
				
			switch(currentHonor)
			{
			case HonorType.Citizen:
				result = LocalizeHelper.Translate("HONOR_CITIZEN");
				break;
			case HonorType.Knight:
				result = LocalizeHelper.Translate("HONOR_KNIGHT");
				break;
			case HonorType.Baron:
				result = LocalizeHelper.Translate("HONOR_BARON");
				break;
			case HonorType.Vicomte:
				result = LocalizeHelper.Translate("HONOR_VICOMTE");
				break;
			case HonorType.Comte:
				result = LocalizeHelper.Translate("HONOR_COMTE");
				break;
			case HonorType.Marquis:
				result = LocalizeHelper.Translate("HONOR_MARQUIS");
				break;
			case HonorType.Duke:
				result = LocalizeHelper.Translate("HONOR_DUKE");
				break;
			case HonorType.Archduke:
				result = LocalizeHelper.Translate("HONOR_ARCHDUKE");
				break;
			case HonorType.Infante:
				if(HonorHelper.IsHonorMaleSex((int)user.Avator))
					result = LocalizeHelper.Translate("HONOR_INFANTE");
				else
					result = LocalizeHelper.Translate("HONOR_PRINCESS");
				break;
			case HonorType.Prince:
				result = LocalizeHelper.Translate("HONOR_PRINCE");
				break;
			case HonorType.King:
				if(HonorHelper.IsHonorMaleSex((int)user.Avator))
					result = LocalizeHelper.Translate("HONOR_KING");
				else
					result = LocalizeHelper.Translate("HONOR_QUEUE");
				break;
			default:
				break;
			}
			
			return result;
		}
	}
}
