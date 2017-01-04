using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using System;
using DataPersist;
using AssemblyCSharp.Helper;
using System.Collections.Generic;
using LilyHeart;

public class BuyItem : MonoBehaviour {
	
	public static event Action<bool, int, string, PayWay> BuySceneFinished;
	private const string out_trade_no = "out_trade_no";
	private const string subject = "subject";
	private const string body = "body";
	private const string total_fee = "total_fee";
	private Dictionary<string, string> resultDic = new Dictionary<string, string>();
	
	
	int type = 0;
	int price = 0;
	string appleStr = string.Empty;
    PayWay payWay = PayWay.UnKown;
	
	// Use this for initialization
	void Start () {
		StartTestConnection();
	}
	
	void TimerEnd()
	{
		StopCoroutine("TestConnection");
	}
	
	void TimerStart()
	{
		//StopCoroutine("TestConnection");
		StartTestConnection();
	}
	
	void ResetTimer()
	{
		StopCoroutine("TestConnection");
		StartTestConnection();
	}
	
	void StartTestConnection()
	{
		StartCoroutine("TestConnection");
	}
	
	IEnumerator TestConnection()
    {
		yield return new WaitForSeconds( 1.0f );
		
		//PhotonClient.Singleton.PopUpMaskingTable();
		//if (User.Singleton.GameStatus != GameStatus.InGame)
		PhotonClient.Singleton.CheckConnection();
		{
			ResetTimer();
		}
    }
	
	void OnDestroy()
	{
		StopCoroutine("TestConnection");
		UtilityHelper.ResetTimerEvent -= ResetTimer;
		UtilityHelper.TimerEndEvent -= TimerEnd;
		UtilityHelper.TimerStartEvent -= TimerStart;
	}
	
	// Update is called once per frame
	void Update () {
		StartCoroutine(NetworkUpdate());
	}
	
	IEnumerator NetworkUpdate()
	{
		PhotonClient.Singleton.Update();
		yield return null;
	}
	
	string GetResultInComma(string originalStr, string valueStr)
	{
		int pos = originalStr.IndexOf(valueStr);
		string subString = originalStr.Substring(pos + (valueStr.Length+1));
		
		return subString.Trim('\"');
	}
	
	void ParseResultString(string result)
	{
		string[] results = result.Split('&');
		
		if (results.Length > 0)
		{
			for(int i = 0; i < results.Length; i++)
			{
				string it = results[i];	
				if (it.Contains(out_trade_no))
				{
					resultDic.Add(out_trade_no, GetResultInComma(it, out_trade_no));
				}
				else if(it.Contains(subject))
				{
					resultDic.Add(subject, GetResultInComma(it, subject));
				}
				else if (it.Contains(body))
				{
					resultDic.Add(body, GetResultInComma(it, body));
				}
				else if (it.Contains(total_fee))
				{
					resultDic.Add(total_fee, GetResultInComma(it, total_fee));
				}
			}
		}
	}
	
	void purchaseAgain()
	{
		switch(type)
		{
		case 3400000: //3.4M
		case 300000:  //300K
		case 120000: //120K
		case 6800000: //6.8M
		case 60000:   //60K
		case 700000:  //700K
		case 250000: //250K
			Shop.Singleton.BuyChip((int)type, price, appleStr, payWay);
			break;
		case (int)RoomType.Egypt:
		case (int)RoomType.Hawaii:
		case (int)RoomType.Japan:
		case (int)RoomType.West:
		case (int)RoomType.China:
			if (BuySceneFinished != null)
				BuySceneFinished(true, price, appleStr, payWay);
			break;
		case 41:
		case 42:
		case 43:
		case 44:
		case 45:
			Shop.Singleton.BuyProp(type % 10, ItemType.Jade, price, appleStr, payWay);
			break;
		case 51:
		case 52:
		case 53:
		case 54:
		case 55:
			Shop.Singleton.BuyProp(type % 10, ItemType.Lineage, price, appleStr, payWay);
			break;
		case 301:
		case 302:
		case 303:
		case 304:
		case 305:
		case 306:
		case 307:
		case 308:
			Shop.Singleton.BuyProp(type % 100, ItemType.Avator, price, appleStr, payWay);
			break;
		}
	}
	
	void purcharseFinished(string result)
	{
		UtilityHelper.CloseMaskingTable();
		
		Debug.Log("the 91result is: " + result);
		resultDic.Clear();
		type = 0;
		price = 0;
		appleStr = string.Empty;
		string[] results = result.Split('|');
	    payWay = PayWay.UnKown;
		
		if (MyVersion.CurrentPlatform == DevicePlatform.Normal && Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if (results == null || results.Length < 2)
			{
				Debug.Log("purcharse failed!");
				return;
			}
			
			type = Convert.ToInt32(results[0]);
			price = Convert.ToInt32(results[1]);
			appleStr = Convert.ToString(results[2]);
			payWay = PayWay.IAP;
			Debug.Log("type is : " + type);
			Debug.Log("price is " + price);
			Debug.Log("appleStr is:" + appleStr);
		}
		else
		{
			if (results == null)
			{
				Debug.Log("purcharse failed!");
				return;
			}
			payWay = PayWay.Alipay;
			ParseResultString(result);
			string title = string.Empty;
			if (resultDic.Count > 0 && resultDic[subject] != null)
			{
				title = resultDic[subject];
			}
			else
			{
				Debug.Log("purcharse failed!");
				return;
			}
			
			setMyType(title);
			
			if (resultDic[total_fee] != null)
			{
				price = Convert.ToInt32(resultDic[total_fee]);
			}
			else
			{
				Debug.Log("purcharse failed!");
				return;
			}
			
			if (resultDic[out_trade_no] != null)
			{
				appleStr = resultDic[out_trade_no];
			}
			else
			{
				Debug.Log("purcharse failed!");
				return;
			}
		}
		
		if (payWay == PayWay.IAP)
		{
			UtilityHelper.CloseMaskingTable();
			MaskingTable table = gameObject.AddComponent<MaskingTable>();
			table.SetNeedPopupDialog(false);
			table.SetCallback(gameObject,"purchaseAgain");
		}
		
		MyBuyItemActionToServer();
	}
	
	void purcharse91Finished(string result)
	{
		// will refresh the userinfo
		string myUserId = User.Singleton.UserData.UserId;
		PhotonClient.Singleton.QueryUserById(myUserId);
		
		UtilityHelper.CloseMaskingTable();
		
		Debug.Log("the result is 91: " + result);
		resultDic.Clear();
		type = 0;
		price = 0;
		appleStr = string.Empty;
		string[] results = result.Split('|');
	    payWay = PayWay.UnKown;
		
		if (MyVersion.CurrentPlatform == DevicePlatform.Normal && Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return;
		}
		else
		{
			if (results == null)
			{
				Debug.Log("purcharse failed!");
				return;
			}
			payWay = PayWay.NineOnePay;
			ParseResultString(result);
			string title = string.Empty;
			if (resultDic.Count > 0 && resultDic[subject] != null)
			{
				title = resultDic[subject];
			}
			else
			{
				Debug.Log("purcharse failed!");
				return;
			}
			
			setMyType(title);
			
			if (resultDic[total_fee] != null)
			{
				price = Convert.ToInt32(resultDic[total_fee]);
			}
			else
			{
				Debug.Log("purcharse failed!");
				return;
			}

			if (resultDic[out_trade_no] != null)
			{
				appleStr = resultDic[out_trade_no];
			}
			else
			{
				Debug.Log("purcharse failed!");
				return;
			}
		}

		MyBuyItemActionToServer();
	}
	
	void purcharseYeepayFinished(string result){
		// will refresh the userinfo
		string myUserId = User.Singleton.UserData.UserId;
		PhotonClient.Singleton.QueryUserById(myUserId);
		
		UtilityHelper.CloseMaskingTable();
		
		Debug.Log("the Yeepay result is: " + result);
		resultDic.Clear();
		type = 0;
		price = 0;
		appleStr = string.Empty;
		string[] results = result.Split('|');
	    payWay = PayWay.UnKown;
		
		if (MyVersion.CurrentPlatform == DevicePlatform.Normal && Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return;
		}
		else
		{
			if (results == null)
			{
				Debug.Log("purcharse failed!");
				return;
			}
			payWay = PayWay.Alipay; // should to change 
			ParseResultString(result);
			string title = string.Empty;
			if (resultDic.Count > 0 && resultDic[subject] != null)
			{
				title = resultDic[subject];
			}
			else
			{
				Debug.Log("purcharse failed!");
				return;
			}
			
			Debug.Log("the Yeepay setMyType is: ");
			setMyType(title);
			
			Debug.Log("the Yeepay price is: ");
			if (resultDic[total_fee] != null)
			{
				price = Convert.ToInt32(resultDic[total_fee]);
				//price = 1;
			}
			else
			{
				Debug.Log("purcharse failed!");
				return;
			}
			
			Debug.Log("the Yeepay out_trade_no is: ");
			if (resultDic[out_trade_no] != null)
			{
				appleStr = resultDic[out_trade_no];
			}
			else
			{
				Debug.Log("purcharse failed!");
				return;
			}
		}
		Debug.Log("the Yeepay lee test type: " + type);
		MyBuyItemActionToServer();
	}
	
	void setMyType(string title){
		if (title == LocalizeHelper.Translate("SCENE_CLASSIC"))
		{
			type = (int)RoomType.China;
		}
		else if (title == LocalizeHelper.Translate("SCENE_EGYPT"))
		{
			type = (int)RoomType.Egypt;
		}
		else if (title == LocalizeHelper.Translate("SCENE_HAWAII"))
		{
			type = (int)RoomType.Hawaii;
		}
		else if (title == LocalizeHelper.Translate("SCENE_JAPAN"))
		{
			type = (int)RoomType.Japan;
		}
		else if (title == LocalizeHelper.Translate("SCENE_WEST"))
		{
			type = (int)RoomType.West;
		}
		else if (title == LocalizeHelper.Translate("BUY_CHIPS_INDEX_60K"))
		{
			type = 60000;
		}
		else if (title == LocalizeHelper.Translate("BUY_CHIPS_INDEX_300K"))
		{
			type = 300000;
		}
		else if (title == LocalizeHelper.Translate("BUY_CHIPS_INDEX_700K"))
		{
			type = 700000;
		}
		else if (title == LocalizeHelper.Translate("BUY_CHIPS_INDEX_3.4M"))
		{
			type = 3400000;
		}
		else if (title == LocalizeHelper.Translate("BUY_CHIPS_INDEX_6.8M"))
		{
			type = 6800000;
		}
		else if (title == LocalizeHelper.Translate("BUY_CHIPS_INDEX_120K"))
		{
			type = 120000;
		}
		else if (title == LocalizeHelper.Translate("BUY_CHIPS_INDEX_250K"))
		{
			type = 250000;
		}
		else if (title == LocalizeHelper.Translate("PROP_4_1"))
		{
			type = 41;
		}
		else if (title == LocalizeHelper.Translate("PROP_4_2"))
		{
			type = 42;
		}
		else if (title == LocalizeHelper.Translate("PROP_4_3"))
		{
			type = 43;
		}
		else if (title == LocalizeHelper.Translate("PROP_4_4"))
		{
			type = 44;
		}
		else if (title == LocalizeHelper.Translate("PROP_4_5"))
		{
			type = 45;
		}
		else if (title == LocalizeHelper.Translate("PROP_5_1"))
		{
			type = 51;
		}
		else if (title == LocalizeHelper.Translate("PROP_5_2"))
		{
			type = 52;
		}
		else if (title == LocalizeHelper.Translate("PROP_5_3"))
		{
			type = 53;
		}
		else if (title == LocalizeHelper.Translate("PROP_5_4"))
		{
			type = 54;
		}
		else if (title == LocalizeHelper.Translate("PROP_5_5"))
		{
			type = 55;
		}
		else if (title == LocalizeHelper.Translate("NICKNAME_OIL"))
		{
			type = 301;
		}
		else if (title == LocalizeHelper.Translate("NICKNAME_BLACK"))
		{
			type = 302;
		}
		else if (title == LocalizeHelper.Translate("NICKNAME_PIRATE"))
		{
			type = 303;
		}
		else if (title == LocalizeHelper.Translate("NICKNAME_PRINCE"))
		{
			type = 304;
		}
		else if (title == LocalizeHelper.Translate("NICKNAME_QIANJIN"))
		{
			type = 305;
		}
		else if (title == LocalizeHelper.Translate("NICKNAME_WEALTHYWIFE"))
		{
			type = 306;
		}
		else if (title == LocalizeHelper.Translate("NICKNAME_SONG_WOMEN"))
		{
			type = 307;
		}
		else if (title == LocalizeHelper.Translate("NICKNAME_LOLI"))
		{
			type = 308;
		}	
	}
	
	void MyBuyItemActionToServer(){
		switch(type)
		{
		case 3400000: //3.4M
		case 300000:  //300K
		case 120000: //120K
		case 6800000: //6.8M
		case 60000:   //60K
		case 700000:  //700K
		case 250000: //250K
			Shop.Singleton.BuyChip((int)type, price, appleStr, payWay);
			break;
		case (int)RoomType.Egypt:
		case (int)RoomType.Hawaii:
		case (int)RoomType.Japan:
		case (int)RoomType.West:
		case (int)RoomType.China:
			
			if (BuySceneFinished != null)
				BuySceneFinished(true, price, appleStr, payWay);
			break;
		case 41:
		case 42:
		case 43:
		case 44:
		case 45:
			Shop.Singleton.BuyProp(type % 10, ItemType.Jade, price, appleStr, payWay);
			break;
		case 51:
		case 52:
		case 53:
		case 54:
		case 55:
			Shop.Singleton.BuyProp(type % 10, ItemType.Lineage, price, appleStr, payWay);
			break;
		case 301:
		case 302:
		case 303:
		case 304:
		case 305:
		case 306:
		case 307:
		case 308:
			Shop.Singleton.BuyProp(type % 100, ItemType.Avator, price, appleStr, payWay);
			break;
		}		
	}	
}
