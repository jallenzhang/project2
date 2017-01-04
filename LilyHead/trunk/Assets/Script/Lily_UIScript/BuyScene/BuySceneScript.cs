using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using DataPersist;
using System.Collections.Generic;
using AssemblyCSharp.Helper;
using LilyHeart;

public class BuySceneScript : MonoBehaviour {
	
	public enum BuySceneState
	{
		Buy,
		ToUse,
		Using
	}
	
	public enum SceneType
	{
		Community = 1,
		Egypt = 2,
		Hawaii = 4,
		Japan = 8,
		West = 16,
		China = 32
	}
	
	public SceneType sceneType = SceneType.Community;
	public BuySceneState sceneState;
	public GameObject btnBackground;
	public GameObject sceneButton;
	public List<Vector3> buttonPositions = new List<Vector3>();
	public List<GameObject> dynamicObjs = new List<GameObject>();
	public GameObject parentItem;
	private long howmuch = 0;
	public string sceneName;
		
	private const string TOUSE = "Btn_use";
	private const string TOBUY = "Btn_buy";
	private const string USING = "useing";
	private const string AFTER = "_press";
	
	// Use this for initialization
	void Start () {
		UpdateScene();
	}
	
	void GetPrice()
	{
		switch(sceneType)
		{
		case SceneType.China:
			howmuch = 200000;
			break;
		case SceneType.Egypt:
			howmuch = 800000;
			break;
		case SceneType.Hawaii:
			howmuch = 70000;
			break;
		case SceneType.Japan:
			howmuch = 130000;
			break;
		case SceneType.West:
			howmuch = 320000;
			break;
		}
	}
	
	public void UpdateScene()
	{
		GetSceneState();
		GetPrice();
		
		if (btnBackground == null)
			return;
		
		UISlicedSprite slicedSprite = btnBackground.GetComponent<UISlicedSprite>();
		
		switch(sceneState)
		{
		case BuySceneState.Buy:
			slicedSprite.spriteName = TOBUY;
			if (sceneButton != null)
			{
				if (buttonPositions != null && buttonPositions.Count > 0)
					sceneButton.transform.localPosition = buttonPositions[1];
				UIImageButton imageButton = sceneButton.GetComponent<UIImageButton>();
				imageButton.hoverSprite = TOBUY;
				imageButton.pressedSprite = string.Concat(TOBUY, AFTER);
				imageButton.normalSprite = TOBUY;
			}
			foreach(GameObject obj in dynamicObjs)
			{
				obj.SetActiveRecursively(true);
			}
			break;
		case BuySceneState.ToUse:
			slicedSprite.spriteName = TOUSE;
			if (sceneButton != null)
			{
				if (buttonPositions != null && buttonPositions.Count > 0)
					sceneButton.transform.localPosition = buttonPositions[0];
				UIImageButton imageButton = sceneButton.GetComponent<UIImageButton>();
				imageButton.hoverSprite = TOUSE;
				imageButton.pressedSprite = string.Concat(TOUSE, AFTER);
				imageButton.normalSprite = TOUSE;
			}
			foreach(GameObject obj in dynamicObjs)
			{
				obj.SetActiveRecursively(false);
			}
			break;
		case BuySceneState.Using:
			slicedSprite.spriteName = USING;
			
			if (sceneButton != null)
			{
				if (buttonPositions != null && buttonPositions.Count > 0)
					sceneButton.transform.localPosition = buttonPositions[0];
				UIImageButton imageButton = sceneButton.GetComponent<UIImageButton>();
				imageButton.hoverSprite = USING;
				imageButton.pressedSprite = USING;
				imageButton.normalSprite = USING;
			}
			foreach(GameObject obj in dynamicObjs)
			{
				obj.SetActiveRecursively(false);
			}
			break;
		}
		
		btnBackground.transform.localScale = new Vector3(slicedSprite.sprite.outer.width, slicedSprite.sprite.outer.height, 0);
	}
	
	void GetSceneState()
	{
		if (User.Singleton != null)
		{
			if (User.Singleton.UserData.LivingRoomType == (RoomType)sceneType)
			{
				sceneState = BuySceneState.Using;
			}
			else if (((int)User.Singleton.UserData.OwnRoomTypes & (int)sceneType) != 0)
			{
				sceneState = BuySceneState.ToUse;
			}
			else 
			{
				sceneState = BuySceneState.Buy;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		UISlicedSprite slicedSprite = btnBackground.GetComponent<UISlicedSprite>();
		btnBackground.transform.localScale = new Vector3(slicedSprite.sprite.outer.width, slicedSprite.sprite.outer.height, 0);
	}
	
	void BuySceneResponse(bool success, ItemType itemType)
	{
		PhotonClient.BuyItemResponseEvent-=BuySceneResponse;
		if (success)
		{	
			UpdateScene();
			BuySceneGridScript buySceneGrid = parentItem.GetComponent<BuySceneGridScript>();
			if (buySceneGrid != null)
				buySceneGrid.UpdateScenes();
			
			//change to the using status after buying.
			btnClick();
		}
		else
		{
			User.Singleton.MessageOperating = true;
			GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new ChipsNotEnoughDialog(LocalizeHelper.Translate("BUY_ITEM_CONFIRM_DIALOG_TITLE"),null,string.Empty));
			PopUpTips();
		}
	}
	
	void PopUpTips()
	{
		GameObject prefab=Resources.Load("prefab/tips2") as GameObject;
		GameObject item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		
		item.transform.parent=parentItem.transform.parent;
		item.transform.localPosition=new Vector3(0,200,-21);
		item.transform.localScale =new Vector3(1,1,1);
	}
	
	void OnBuyWithChips()
	{
		PhotonClient.BuyItemResponseEvent+=BuySceneResponse;
		if (User.Singleton.UserData.Chips >= howmuch)
			Shop.Singleton.BuyRoomWithChips((RoomType)sceneType, howmuch);
		else
		{
			User.Singleton.MessageOperating = true;
			GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new ChipsNotEnoughDialog(LocalizeHelper.Translate("BUY_ITEM_CONFIRM_DIALOG_TITLE"),null,string.Empty));
			PopUpTips();
		}
	}
	
	void btnClick()
	{
		switch(sceneState)
		{
			case BuySceneState.Buy:
			{
				User.Singleton.MessageOperating = true;
				GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new BuyItemConfirmDialog(howmuch, sceneName, gameObject, "OnBuyWithChips", "Scene"));
				PopUpTips();
				break;
			}
			case BuySceneState.ToUse:
			{	
				if (User.Singleton.Friends == null || !User.Singleton.Friends.Exists(f => f.UserId == Room.Singleton.RoomData.Owner.UserId))
					PhotonClient.RoomTypeChanged+=ChangeRoomBackGround;
				
				User.Singleton.Save(User.Singleton.UserData.Avator, User.Singleton.UserData.Password, (RoomType)sceneType);
				UpdateScene();
				BuySceneGridScript buySceneGrid = parentItem.GetComponent<BuySceneGridScript>();
				if (buySceneGrid != null)
					buySceneGrid.UpdateScenes();
				break;
			}
		}
	}
	
	public void ChangeRoomBackGround()
	{
		ChangeBackGroundSence cbgs = gameObject.GetComponent<ChangeBackGroundSence>();
		cbgs.changeRoomBg(sceneType.ToString());
		BuySceneGridScript buySceneGrid = parentItem.GetComponent<BuySceneGridScript>();
		if (buySceneGrid != null)
			buySceneGrid.UpdateScenes();
		PhotonClient.RoomTypeChanged-=ChangeRoomBackGround;
	}
	
	void OnDestroy()
	{
		PhotonClient.RoomTypeChanged-=ChangeRoomBackGround;
		PhotonClient.BuyItemResponseEvent-=BuySceneResponse;
	}
}
