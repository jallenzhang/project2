using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using DataPersist;
using AssemblyCSharp.Helper;
using LilyHeart;

public class BuyChipsTable : MonoBehaviour {
	
	private GameObject parentItem;
	private GameObject item;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
//	void disabledAllBtns()
//	{
//		BoxCollider[] boxcoolides = parentItem.GetComponentsInChildren<BoxCollider>();//parentItem
//		foreach(BoxCollider one in boxcoolides)
//		{
//			one.enabled=false;
//		}
//	}
	
	void PopUpTips()
	{
		//disabledAllBtns();
		GameObject prefab=Resources.Load("prefab/tips2") as GameObject;
		item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		
		item.transform.parent=transform.FindChild("Button_1");
		item.transform.localPosition=new Vector3(-260,300,-21);
		item.transform.localScale =new Vector3(1,1,1);
	}
	
	string GetBuyChipsTitle(int index)
	{
		string result = string.Empty;
		switch(index)
		{
		case 60000:
			result = LocalizeHelper.Translate("BUY_CHIPS_INDEX_60K");
			break;
		case 300000:
			result = LocalizeHelper.Translate("BUY_CHIPS_INDEX_300K");
			break;
		case 700000:
			result = LocalizeHelper.Translate("BUY_CHIPS_INDEX_700K");
			break;
		case 3400000:
			result = LocalizeHelper.Translate("BUY_CHIPS_INDEX_3.4M");
			break;
		case 6800000:
			result = LocalizeHelper.Translate("BUY_CHIPS_INDEX_6.8M");
			break;
		case 120000:
			result = LocalizeHelper.Translate("BUY_CHIPS_INDEX_120K");
			break;
		case 250000:
			result = LocalizeHelper.Translate("BUY_CHIPS_INDEX_250K");
			break;
		}
		
		return result;
	}
	
	void startBuyChips(int index, int price)
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer && MyVersion.CurrentPlatform == DevicePlatform.Normal)
		{
#if UNITY_IPHONE
			User.bBuyThingsInIos = true;
			MaskingTable table = gameObject.transform.parent.gameObject.AddComponent<MaskingTable>();
			table.SetNeedPopupDialog(false);
			EtceteraBinding.etceteraPurchaseWithIndex(index);
#endif
		}
		else if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
		{
#if UNITY_IPHONE
			if(GlobalManager.Singleton.ApplicationType == AppType.NinetyOne)
				EtceteraBinding.etceteraPurchaseWithUnityCall91Pay(GetBuyChipsTitle(index), price, User.Singleton.UserData.UserId, UtilityHelper.GetChannelId());
			else
				EtceteraBinding.etceteraPurchaseWithUnityCallAlixPay(GetBuyChipsTitle(index), price, User.Singleton.UserData.UserId, UtilityHelper.GetChannelId());
#endif

#if UNITY_ANDROID
			PaySwitchScript.OnShowPaySwitchView(gameObject.name, GetBuyChipsTitle(index), "Toufe", price.ToString());
			// EtceteraAndroid.StartBuy(GetBuyChipsTitle(index), "Toufe", price.ToString());
#endif
		}
	}
	void OnRegister(string values)
	{
		Debug.Log("OnRegister"+values);
		PlatformHelper.OnUpgrade(values);
	}
	void btnBuyChips(GameObject btn)
	{
		//remove Guest limit
//		if (User.Singleton.UserData.UserType == UserType.Guest)
//		{
//			((Player)User.Singleton).CurrentInfos.Enqueue(new GuestBuyChipsLimitedDialog());
//			parentItem = gameObject.transform.parent.gameObject;//here get front button game object
//			PopUpTips();
//		}
		if (User.Singleton.UserData.UserType == UserType.Guest&&GlobalManager.Singleton.ApplicationType == AppType.NinetyOne)
		{
			GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new NinetyOneGuestBuyTipDialog(gameObject.name,"OnRegister",true));
			PopUpTips();
		}
		else
		{
			switch(btn.name)
			{
			case "Button_1":
				startBuyChips(6800000, 648);
				break;
			case "Button_2":
				startBuyChips(3400000, 328);
				break;
			case "Button_3":
				startBuyChips(700000, 68);
				break;
			case "Button_4":
				startBuyChips(300000, 30);
				break;
			case "Button_5":
				startBuyChips(250000, 25);
				break;
			case "Button_6":
				startBuyChips(120000, 12);
				break;
			case "Button_7":
				startBuyChips(60000, 6);
				break;
			default:break;
			}
		}
	}
	
	void DestroyItem()
	{
		Destroy(gameObject);
	}
}
