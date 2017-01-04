using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using DataPersist;
using System;
using AssemblyCSharp.Helper;
using LilyHeart;

public class AvatorGridScript : MonoBehaviour {
	
	private GameObject currentAvator = null;
	public  GameObject nickNameObj;
	public string mail;
	public string password;
	
	// Use this for initialization
	void Start () {
		PhotonClient.BuyItemResponseEvent += UpdateAvators;
		UpdateAvators(true, ItemType.Room);
	}
	
	IEnumerator NetworkUpdate()
	{
		PhotonClient.Singleton.Update();
		yield return null;
	}
	
	// Update is called once per frame
	void Update () {
		StartCoroutine(NetworkUpdate());
	}
	
	string GenerateItemName(Props prop)
	{
		return string.Concat(prop.ItemType.ToString(), "_", prop.ItemId.ToString());
	}
	
	long GetCurrentAvatorPrice()
	{
		if (currentAvator == null)
		{
			return -1;
		}
		
		AvatorScript avator = currentAvator.GetComponent<AvatorScript>();
		if (avator != null)
			return (long)avator.price;
		
		return -1;
	}
	
	int GetCurrentAvatorItemId()
	{
		if (currentAvator == null)
			return -1;
		
		AvatorScript avator = currentAvator.GetComponent<AvatorScript>();
		if (avator != null)
			return (int)avator.avatorIdInPurchase;
		
		return -1;
	}
	
	byte GetCurrentAvatorType()
	{
		if (currentAvator != null)
		{
			AvatorScript avator = currentAvator.GetComponent<AvatorScript>();
			if (avator != null)
				return (byte)avator.avatorType;
			else
				return 0;
		}
		
		return 0;
	}
	
	int GetCurrentAvatorPayWay()
	{
		if (currentAvator != null)
		{
			AvatorScript avator = currentAvator.GetComponent<AvatorScript>();
			if (avator != null)
				return (int)avator.feeType;
			else
				return -1;
		}
		
		return -1;
	}
	
	string GetCurrentAvatorName()
	{
		if (currentAvator != null)
		{
			AvatorScript avator = currentAvator.GetComponent<AvatorScript>();
			if (avator != null)
				return avator.avatorName;
			else
				return string.Empty;
		}
		
		return string.Empty;
	}
	
	void OnUse()
	{
		OnAccept(true);
	}
	
	void OnUpgrade()
	{
		if (currentAvator == null)
		{
			GlobalManager.Log("currentAvator is null");
			UtilityHelper.PopUpErrorDialog(LocalizeHelper.Translate("LOGIN_AVATOR_CHOOSE_ERROR"));
			return;
		}
		
		if (User.Singleton != null)
		{
			byte avator = GetCurrentAvatorType();
			GlobalManager.Log(currentAvator.name);
			if(nickNameObj!=null)
			{
				UILabel nick = nickNameObj.GetComponent<UILabel>();
				if(nick!=null)
				{
					string nickName = nick.text;
					if (nickName == LocalizeHelper.Translate("PLEASE_INPUT_YOUR_NICKNAME"))
					{
						UtilityHelper.PopUpErrorDialog(LocalizeHelper.Translate("LOGIN_NICKNAME_LENGTH_ERROR"));
						return;
					}
					if(nickName.Length == 0||string.IsNullOrEmpty(nickName))
					{
						UtilityHelper.PopUpErrorDialog(LocalizeHelper.Translate("LOGIN_NICKNAME_LENGTH_ERROR"));
						return;
					}
					//nick.text = UtilityHelper.checkNickName(nickName);
					
					GlobalManager.Log("avator is: " + avator);
					if (avator > 0)
					{
						PhotonClient.RegisterOrUpgradeEvent += OnUpgradeFinish;
						User.Singleton.GuestUpgrade(nick.text, mail, password.getMD5(), DeviceTokenHelper.myDeviceToken, avator);
					}
				}
			}
		}
	}
	
	void OnAccept(bool bRegister)
	{
		if (currentAvator == null)
		{
			GlobalManager.Log("currentAvator is null");
			UtilityHelper.PopUpErrorDialog(LocalizeHelper.Translate("LOGIN_AVATOR_CHOOSE_ERROR"));
			return;
		}
		UILabel nick = null;
		if(nickNameObj!=null)
		{
		    nick = nickNameObj.GetComponent<UILabel>();
			if(nick!=null)
			{
				string nickName = nick.text;
				if (nickName == LocalizeHelper.Translate("PLEASE_INPUT_YOUR_NICKNAME"))
				{
					UtilityHelper.PopUpErrorDialog(LocalizeHelper.Translate("LOGIN_NICKNAME_LENGTH_ERROR"));
					return;
				}
				if(nickName.Length == 0||string.IsNullOrEmpty(nickName))
				{
					UtilityHelper.PopUpErrorDialog(LocalizeHelper.Translate("LOGIN_NICKNAME_LENGTH_ERROR"));
					return;
				}
				//nick.text = UtilityHelper.checkNickName(nickName);
			}
		}
		if (User.Singleton != null)
		{
			byte avator = GetCurrentAvatorType();
			if (bRegister)
			{
				GlobalManager.Log(currentAvator.name);
				GlobalManager.Log("avator is: " + avator);
				if (avator > 0&&nick!=null)
				{
					PhotonClient.RegisterOrUpgradeEvent += onRegisterResponse;
					if(GlobalManager.Singleton.InitGuestSignIn)
					{
						Debug.Log("GlobalManager.Singleton.InitGuestSignIn");
						User.Singleton.Register(nick.text, mail, password.getMD5(),  DeviceTokenHelper.myDeviceToken, avator,UserType.Guest);
					}
					else
					{
						User.Singleton.Register(nick.text, mail, password.getMD5(),  DeviceTokenHelper.myDeviceToken, avator);
					}
				}
			}
			else
			{
				if (avator > 0)
				{
					User.Singleton.Save(avator, User.Singleton.UserData.Password, User.Singleton.UserData.LivingRoomType);
					User.Singleton.UserInfoChanged();
					UtilityHelper.RoomDataChanged();
					fadePanel panel = gameObject.GetComponent<fadePanel>();
					panel.fadeOut(null);
				}
			}
		}
	}
	void OnUpgradeFinish(bool success)
	{
		PhotonClient.RegisterOrUpgradeEvent -= OnUpgradeFinish;
		if (success)
		{
			GlobalManager.Singleton.ParamPassword=string.Empty;
			fadePanel panel = gameObject.GetComponent<fadePanel>();
			panel.fadeOut(null);
		}
		else
		{
#if UNITY_IPHONE
			Debug.Log(LocalizeHelper.Translate("DIALOG_TITLE_ERROR") + " " + PhotonClient.Singleton.ErrorMessage);
			//EtceteraBinding.showAlertWithTitleMessageAndButton(LocalizeHelper.Translate("DIALOG_TITLE_ERROR"), PhotonClient.Singleton.ErrorMessage, LocalizeHelper.Translate("DIALOG_BUTTON_OK"));
			
#endif
#if UNITY_ANDROID
			//EtceteraAndroid.showAlert(LocalizeHelper.Translate("DIALOG_TITLE_ERROR"), PhotonClient.Singleton.ErrorMessage, LocalizeHelper.Translate("DIALOG_BUTTON_OK"));
			//EtceteraAndroid.excuteJS("alert('User Exsist')");
#endif
			GlobalManager.Singleton.Notifications.Enqueue(new The3rdFirstLoginNotification());
		}
	}

	void onRegisterResponse (bool success)
	{
		PhotonClient.RegisterOrUpgradeEvent -= onRegisterResponse;
		if (success)
		{
			gameObject.AddComponent<ShowLoadingTable>();
		}
		else
		{
#if UNITY_IPHONE
			Debug.Log(LocalizeHelper.Translate("DIALOG_TITLE_ERROR") + " " + PhotonClient.Singleton.ErrorMessage);
			//EtceteraBinding.showAlertWithTitleMessageAndButton(LocalizeHelper.Translate("DIALOG_TITLE_ERROR"), PhotonClient.Singleton.ErrorMessage, LocalizeHelper.Translate("DIALOG_BUTTON_OK"));
			
#endif
#if UNITY_ANDROID
			//EtceteraAndroid.showAlert(LocalizeHelper.Translate("DIALOG_TITLE_ERROR"), PhotonClient.Singleton.ErrorMessage, LocalizeHelper.Translate("DIALOG_BUTTON_OK"));
			//EtceteraAndroid.excuteJS("alert('User Exsist')");
#endif
		}
	}
	
	
	void SetCurrentItem(GameObject obj)
	{
		if (obj != null)
		{
			currentAvator = obj;
		}
	}
	
	void OnBuy()
	{
		if (currentAvator != null)
		{
			int type = GetCurrentAvatorItemId();
			long price = GetCurrentAvatorPrice();
			string avatorName= GetCurrentAvatorName();
			
			if (type != -1 && price != -1)
			{
				int payWay = GetCurrentAvatorPayWay();
				switch(payWay)
				{
				case 0://free
				case 4:
					OnAccept(false);
					return;
				case 1://chips
					User.Singleton.MessageOperating = true;
					GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new BuyItemConfirmDialog(price, avatorName, gameObject, "OnBuyWithChips", "Avatar"));
					PopUpTips();
					//OnBuyWithChips();
					return;
				case 2://money
					BuyItemHelper.StartBuy(type, price);
					return;
				default:
					break;
				}
				
			}
			
			Debug.LogWarning("Buy failed!");
		}
	}
	
	void PopUpTips()
	{
		GameObject prefab=Resources.Load("prefab/tips2") as GameObject;
		GameObject item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		
		item.transform.parent=transform;
		item.transform.localPosition=new Vector3(0,200,-21);
		item.transform.localScale =new Vector3(1,1,1);
	}
	
	void OnBuyWithChips()
	{
		if (currentAvator != null)
		{
			int type = GetCurrentAvatorType();
			long price = GetCurrentAvatorPrice();
			
			if (type != 0)
			{
				Debug.Log("type: " + type);
				if (price <= User.Singleton.UserData.Chips)
				{
					Shop.Singleton.BuyPropWithChip((int)ItemType.Avator, type, price);
				}
				else
				{
					User.Singleton.MessageOperating = true;
					GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new ChipsNotEnoughDialog(LocalizeHelper.Translate("BUY_ITEM_CONFIRM_DIALOG_TITLE"),null,string.Empty));
					PopUpTips();
				}
			}
			else
			{
				Debug.LogWarning("OnBuyWithChips error");
			}
		}
	}
	
//	void UpdateAvator(Props prop)
//	{
//		string itemName = GenerateItemName(prop);
//		Transform trs = gameObject.transform.FindChild(itemName);
//		if (trs != null)
//		{
//			Transform[] alltrans = trs.GetComponentsInChildren<Transform>();
//			foreach(Transform tr in alltrans)
//			{
//				//todo update days using prop.duration
//			}
//		}
//		else
//		{
//			Debug.Log("Can't find the child " + itemName);
//		}
//	}
	
	
	void UpdateAvators(bool success, ItemType itemType)
	{
		Debug.Log("Success: " + success);
		if (success && User.Singleton != null && User.Singleton.Avators != null)
		{
			TF_ButtonGroup avatorGroup = gameObject.GetComponent<TF_ButtonGroup>();
			if (avatorGroup != null && avatorGroup.buttons != null)
			{
				foreach(GameObject obj in avatorGroup.buttons)
				{
					AvatorScript avScript = obj.GetComponent<AvatorScript>();
					if (avScript != null && User.Singleton.Avators.Exists(r => r.ItemId == (int)avScript.avatorType))
					{
						if ((int)avScript.avatorType == (int)User.Singleton.UserData.Avator)
						{
							avScript.feeType = AvatorScript.AvatorFeeType.Using;
						}
						else
						{
							avScript.feeType = AvatorScript.AvatorFeeType.ToUse;
						}
					}
					
					if (avScript != null)
						avScript.UpdateAvator();
				}
			}
			
			if (currentAvator != null)
			{
				OnAccept(false);
			}
		}
		else if (!success)
		{
			//TODO: buy item failed
			User.Singleton.MessageOperating = true;
			GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new ChipsNotEnoughDialog(LocalizeHelper.Translate("BUY_ITEM_CONFIRM_DIALOG_TITLE"),null,string.Empty));
			PopUpTips();
		}	
	}
	
	void OnDestroy()
	{
		PhotonClient.BuyItemResponseEvent -= UpdateAvators;
		PhotonClient.RegisterOrUpgradeEvent -= onRegisterResponse;
	}
}
