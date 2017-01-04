using UnityEngine;
using System.Collections;
using LilyHeart;
using DataPersist;
using AssemblyCSharp.Helper;
using AssemblyCSharp;

public class BuyItemHelper {
	
	public static string GameObjectName = string.Empty;
	
	public static void StartBuy(int type, long price)
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer && MyVersion.CurrentPlatform == DevicePlatform.Normal)
		{
#if UNITY_IPHONE
			User.bBuyThingsInIos = true;
			EtceteraBinding.etceteraPurchaseWithIndex(type);
#endif
		}
		else if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
		{
			string title = string.Empty;
			
			switch(type)
			{
			case (int)RoomType.China:
				title = LocalizeHelper.Translate("SCENE_CLASSIC");
				break;
			case (int)RoomType.Egypt:
				title = LocalizeHelper.Translate("SCENE_EGYPT");
				break;
			case (int)RoomType.Hawaii:
				title = LocalizeHelper.Translate("SCENE_HAWAII");
				break;
			case (int)RoomType.Japan:
				title = LocalizeHelper.Translate("SCENE_JAPAN");
				break;
			case (int)RoomType.West:
				title = LocalizeHelper.Translate("SCENE_WEST");
				break;
			case 60000:
				title = LocalizeHelper.Translate("BUY_CHIPS_INDEX_60K");
				break;
			case 300000:
				title = LocalizeHelper.Translate("BUY_CHIPS_INDEX_300K");
				break;
			case 700000:
				title = LocalizeHelper.Translate("BUY_CHIPS_INDEX_700K");
				break;
			case 3400000:
				title = LocalizeHelper.Translate("BUY_CHIPS_INDEX_3.4M");
				break;
			case 6800000:
				title = LocalizeHelper.Translate("BUY_CHIPS_INDEX_6.8M");
				break;
			case 120000:
				title = LocalizeHelper.Translate("BUY_CHIPS_INDEX_120K");
				break;
			case 250000:
				title = LocalizeHelper.Translate("BUY_CHIPS_INDEX_250K");
				break;
			case 41:
				title = LocalizeHelper.Translate("PROP_4_1");
				break;
			case 42:
				title = LocalizeHelper.Translate("PROP_4_2");
				break;
			case 43:
				title = LocalizeHelper.Translate("PROP_4_3");
				break;
			case 44:
				title = LocalizeHelper.Translate("PROP_4_4");
				break;
			case 45:
				title = LocalizeHelper.Translate("PROP_4_5");
				break;
			case 51:
				title = LocalizeHelper.Translate("PROP_5_1");
				break;
			case 52:
				title = LocalizeHelper.Translate("PROP_5_2");
				break;
			case 53:
				title = LocalizeHelper.Translate("PROP_5_3");
				break;
			case 54:
				title = LocalizeHelper.Translate("PROP_5_4");
				break;
			case 55:
				title = LocalizeHelper.Translate("PROP_5_5");
				break;
			case 301:
				title = LocalizeHelper.Translate("NICKNAME_OIL");
				break;
			case 302:
				title = LocalizeHelper.Translate("NICKNAME_BLACK");
				break;
			case 303:
				title = LocalizeHelper.Translate("NICKNAME_PIRATE");
				break;
			case 304:
				title = LocalizeHelper.Translate("NICKNAME_PRINCE");
				break;
			case 305:
				title = LocalizeHelper.Translate("NICKNAME_QIANJIN");
				break;
			case 306:
				title = LocalizeHelper.Translate("NICKNAME_WEALTHYWIFE");
				break;
			case 307:
				title = LocalizeHelper.Translate("NICKNAME_SONG_WOMEN");
				break;
			case 308:
				title = LocalizeHelper.Translate("NICKNAME_LOLI");
				break;
				
			}
#if UNITY_IPHONE
			if(GlobalManager.Singleton.ApplicationType == AppType.NinetyOne)
				EtceteraBinding.etceteraPurchaseWithUnityCall91Pay(title, price, User.Singleton.UserData.UserId, UtilityHelper.GetChannelId());
			else
				EtceteraBinding.etceteraPurchaseWithUnityCallAlixPay(title, price, User.Singleton.UserData.UserId, UtilityHelper.GetChannelId());
#endif
			
#if UNITY_ANDROID
			PaySwitchScript.OnShowPaySwitchView(GameObjectName,title, "Toufe", price.ToString());
			//EtceteraAndroid.StartBuy(title, "Toufe", price.ToString());
#endif
		}
	}
}
