using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using DataPersist;
using System;
using System.Collections.Generic;
using LilyHeart;

public class PropScript : MonoBehaviour {
	
	public GameObject yuxi;
	public GameObject xuetong;
	
	public GameObject yuxi1;
	public GameObject yuxi2;
	public GameObject xuetong1;
	public GameObject xuetong2;
	public GameObject yuxi_xufei1;
	public GameObject yuxi_xufei2;
	public GameObject xuetong_xufei1;
	public GameObject xuetong_xufei2;
	
	public Vector3 yuxi1_position;
	public Vector3 yuxi2_position;
	public Vector3 xuetong1_position;
	public Vector3 xuetong2_position;
	public Vector3 yuxi_xufei1_position;
	public Vector3 yuxi_xufei2_position;
	public Vector3 xuetong_xufei1_position;
	public Vector3 xuetong_xufei2_position;
	
	
	private bool bIsOpenYuXi = false;//YuXi
	private bool bIsOpenLineage = false;//XueTong
	
	// Use this for initialization
	void Start () {
		//PhotonClient.BuyPropFinishedEvent += UpdateProp;
		PhotonClient.BuyPropFinishedEvent += ResetBtnStatus;
		ResetBtnStatus(null);
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
	
	void ResetBtnStatus(Props prop)
	{
		bIsOpenYuXi = User.Singleton.UserData.Jade;
		bIsOpenLineage = User.Singleton.UserData.LineAge;
		
		yuxi1.SetActiveRecursively(!bIsOpenYuXi);
		yuxi2.SetActiveRecursively(!bIsOpenYuXi);
		xuetong1.SetActiveRecursively(!bIsOpenLineage);
		xuetong2.SetActiveRecursively(!bIsOpenLineage);
		yuxi_xufei1.SetActiveRecursively(bIsOpenYuXi);
		yuxi_xufei2.SetActiveRecursively(bIsOpenYuXi);
		xuetong_xufei1.SetActiveRecursively(bIsOpenLineage);
		xuetong_xufei2.SetActiveRecursively(bIsOpenLineage);
		
		if(bIsOpenYuXi)
		{
			yuxi_xufei1.transform.localPosition = yuxi_xufei1_position;
			yuxi_xufei2.transform.localPosition = yuxi_xufei2_position;
		}
		else
		{
			yuxi1.transform.localPosition = yuxi1_position;
			yuxi2.transform.localPosition = yuxi2_position;
		}
		
		if(bIsOpenLineage)
		{
			xuetong_xufei1.transform.localPosition = xuetong_xufei1_position;
			xuetong_xufei2.transform.localPosition = xuetong_xufei2_position;
		}
		else
		{
			xuetong1.transform.localPosition = xuetong1_position;
			xuetong2.transform.localPosition = xuetong2_position;
		}
	}
	
	int GetPriceWithItemId(string itemId)
	{
		if (string.IsNullOrEmpty(itemId))
		{
			return -1;
		}
		
		switch(itemId)
		{
		case "4_1":
		case "4_1_1":
			return 6;
		case "4_2":
			return 12;	
		case "4_3":
			return 18;
		case "4_4":
		case "4_4_1":
			return 30;
		case "4_5":
			return 98;
		case "5_1":
		case "5_1_1":
			return 6;
		case "5_2":
			return 12;	
		case "5_3":
			return 18;
		case "5_4":
		case "5_4_1":
			return 30;
		case "5_5":
			return 98;
		default:
			return -1;
		}
	}
	
	int GetTypeWithItemId(string itemId)
	{
		if (string.IsNullOrEmpty(itemId))
		{
			return -1;
		}
		
		switch(itemId)
		{
		case "4_1":
		case "4_1_1":
			return 41;
		case "4_2":
			return 42;	
		case "4_3":
			return 43;
		case "4_4":
		case "4_4_1":
			return 44;
		case "4_5":
			return 45;
		case "5_1":
		case "5_1_1":
			return 51;
		case "5_2":
			return 52;	
		case "5_3":
			return 53;
		case "5_4":
		case "5_4_1":
			return 54;
		case "5_5":
			return 55;
		default:
			return -1;
		}
	}
	
	void OnUse(GameObject obj)
	{
		
	}
	void OnRegister(string values)
	{
		Debug.Log("OnRegister"+values);
		PlatformHelper.OnUpgrade(values);
	}
	void PopUpTips()
	{
		GameObject prefab=Resources.Load("prefab/tips2") as GameObject;
		GameObject item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		
		item.transform.parent=transform;
		item.transform.localPosition=new Vector3(0,200,-21);
		item.transform.localScale =new Vector3(1,1,1);
	}
	void OnBuy(GameObject obj)
	{

		if (User.Singleton.UserData.UserType == UserType.Guest&&GlobalManager.Singleton.ApplicationType == AppType.NinetyOne)
		{
			GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new NinetyOneGuestBuyTipDialog(gameObject.name,"OnRegister",true));
			PopUpTips();
		}
		else
		{
			int type = GetTypeWithItemId(obj.name);
			int price = GetPriceWithItemId(obj.name);
			
			Debug.Log("type " + type + "price " + price);
			if (type != -1 && price != -1)
			{
				if( Application.platform == RuntimePlatform.IPhonePlayer && MyVersion.CurrentPlatform == DevicePlatform.Normal)
				{
					MaskingTable table = gameObject.AddComponent<MaskingTable>();
					table.SetNeedPopupDialog(false);
				}
				BuyItemHelper.GameObjectName = gameObject.name;
				BuyItemHelper.StartBuy(type, price);
			}
			else
				Debug.LogWarning("Buy failed!");
		}
	}
	
	void OnBuyWithChips(GameObject obj)
	{
		string itemName = obj.transform.parent.name;
		string[] type_id = itemName.Split('_'); //todo should to ensure is parent or not
		
		if (type_id != null && type_id.Length == 2)
		{
			//Shop.Singleton.BuyPropWithChip(Convert.ToInt32(type_id[0]), Convert.ToInt32(type_id[1]));
		}
		else
		{
			Debug.LogWarning("OnBuyWithChips error");
		}
	}
	void OnDestroy()
	{
		PhotonClient.BuyPropFinishedEvent -= ResetBtnStatus;
	}
}
