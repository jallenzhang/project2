using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;
using DataPersist;
using AssemblyCSharp.Helper;
using LilyHeart;

public class BuySenceGrid : MonoBehaviour {
public enum Arrangement
	{
		Horizontal,
		Vertical,
	}

	public Arrangement arrangement = Arrangement.Horizontal;
	public int maxPerLine = 0;
	public float cellWidth = 200f;
	public float cellHeight = 200f;
	public bool repositionNow = false;
	public bool sorted = false;
	public bool hideInactive = true;
	
	
	public GameObject item0;
	public GameObject spritebg;
	 
	GameObject addbtn;
	
	private Vector3 orignPosition;
	private Vector3 orignScale;
	
	private  Transform origntransform;
	private bool scaled=false;
	
	public GameObject panel;
	public GameObject parentItem;
	
	private UISlicedSprite currentSlicedSprite;
	private BoxCollider currentBox;
	private GameObject buyBtn;
	// Use this for initialization
	
    void Start ()
	{
		UpdateItems();
		Reposition();
	}
	public void addItems()
	{
 		for(int i=0;i<10;i++)
		{
 		   GameObject item=Instantiate(item0) as GameObject;
		   item.name="item 0";
  		   item.transform.parent=transform;
 		   item.transform.localScale=new Vector3(1f,1f,1f);
		   item.transform.localPosition=new Vector3(0,0,0);
   		}
 		Reposition ();
	}
	
	void UpdateItem(GameObject item, int type)
	{
		Transform[] alltrans=item.GetComponentsInChildren<Transform>();
		
		Debug.Log("User.Singleton.UserData.LivingRoomType: " + User.Singleton.UserData.LivingRoomType);
		
		foreach(Transform trs in alltrans)
		{
			UISlicedSprite slicedSprite = (UISlicedSprite)trs.gameObject.GetComponent<UISlicedSprite>();
			switch(trs.gameObject.name)
			{
			case "buyBackground":
				slicedSprite.tag = string.Format("buySceneBackground{0}", type);
				if (User.Singleton.UserData.LivingRoomType == (RoomType)type)
				{
					slicedSprite.spriteName = "BSTHasbeenused";
					slicedSprite.transform.localScale = new Vector3(143, 42, 1);
					currentSlicedSprite = slicedSprite;
				}
				else if (((int)User.Singleton.UserData.OwnRoomTypes & type) != 0)
				{
					Debug.Log("OwnerRoomType is: " + User.Singleton.UserData.OwnRoomTypes.ToString());
					slicedSprite.spriteName = "BSTGreenBtn";
				}
				break;
			case "btnbuy":
				BoxCollider box = (BoxCollider)trs.gameObject.GetComponent<BoxCollider>();
				if ((int)User.Singleton.UserData.LivingRoomType == 0)
					User.Singleton.UserData.LivingRoomType = RoomType.Common;
				
				if (User.Singleton.UserData.LivingRoomType == (RoomType)type)
				{
					box.enabled = false;
					currentBox = box;
				}
				else
				{
					box.enabled = true;
				}
				break;
			}
		}
	}
	 
	void UpdateItems()
	{
		Transform[] alltrans=gameObject.GetComponentsInChildren<Transform>();
		foreach(Transform trs in alltrans)
		{
			switch(trs.gameObject.name)
			{
			case "item0":
				UpdateItem(trs.gameObject, (int)RoomType.Common);
				break;
			case "item1":
				UpdateItem(trs.gameObject, (int)RoomType.China);
				break;
			case "item2":
				UpdateItem(trs.gameObject, (int)RoomType.Egypt);
				break;
			case "item3":
				UpdateItem(trs.gameObject, (int)RoomType.Hawaii);
				break;
			case "item4":
				UpdateItem(trs.gameObject, (int)RoomType.Japan);
				break;
			case "item5":
				UpdateItem(trs.gameObject, (int)RoomType.West);
				break;
			}
		}
	}
	
	void Update ()
	{
		if (repositionNow)
		{
			repositionNow = false;
			Reposition();
		}
		
		StartCoroutine(NetworkUpdate());
	}
	
	IEnumerator NetworkUpdate()
	{
		PhotonClient.Singleton.Update();
		yield return null;
	}
	
	private RoomType GetRoomTypeWithTag(string tag)
	{
		RoomType type = RoomType.Common;
		switch(tag)
		{
		case "buySceneBackground1":
			type = RoomType.Common;
			break;
		case "buySceneBackground2":
			type = RoomType.Egypt;
			break;
		case "buySceneBackground4":
			type = RoomType.Hawaii;
			break;
		case "buySceneBackground8":
			type = RoomType.Japan;
			break;
		case "buySceneBackground16":
			type = RoomType.West;
			break;
		case "buySceneBackground32":
			type = RoomType.China;
			break;
		}
		
		return type;
	}
	
	void StartBuy(int type, int price)
	{
		if( Application.platform == RuntimePlatform.IPhonePlayer && MyVersion.CurrentPlatform == DevicePlatform.Normal)
		{
#if UNITY_IPHONE
			User.bBuyThingsInIos = true;
			MaskingTable table = gameObject.AddComponent<MaskingTable>();
			table.SetNeedPopupDialog(false);
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
			}
#if UNITY_IPHONE
			if(GlobalManager.Singleton.ApplicationType == AppType.NinetyOne)
				EtceteraBinding.etceteraPurchaseWithUnityCall91Pay(title, price, User.Singleton.UserData.UserId, UtilityHelper.GetChannelId());
			else
				EtceteraBinding.etceteraPurchaseWithUnityCallAlixPay(title, price, User.Singleton.UserData.UserId, UtilityHelper.GetChannelId());
#endif
			
#if UNITY_ANDROID
			EtceteraAndroid.StartBuy(title, "Toufe", price.ToString());
#endif
		}
	}
	
	void BuySceneFinished(bool successed){
		
		BuySceneFinished(successed,0,"", PayWay.UnKown);
	}
	
	void BuySceneWithChips()
	{
		if (buyBtn != null)
		{
			Transform[] alltrans=buyBtn.GetComponentsInChildren<Transform>();
		
			BoxCollider box = (BoxCollider)buyBtn.GetComponent<BoxCollider>();
			box.enabled = false;
			currentBox.enabled = true;
			currentBox = box;
			
			foreach(Transform trs in alltrans)
			{
				UISlicedSprite slicedSprite = (UISlicedSprite)trs.gameObject.GetComponent<UISlicedSprite>();
				switch(trs.gameObject.name)
				{
				case "buyBackground":
					if (slicedSprite.spriteName.Contains("BSTRedBtn"))
					{
					 	//TODO: Shop.Singleton.BuyRoomWithChips(GetRoomTypeWithTag(slicedSprite.tag));
						User.Singleton.UserData.OwnRoomTypes |= GetRoomTypeWithTag(slicedSprite.tag);
						
						Debug.Log("slicedSprite's tag is: " + slicedSprite.tag);
						
						slicedSprite.spriteName = "BSTGreenBtn";
					}
					else if (slicedSprite.spriteName == "BSTGreenBtn")
					{
						currentSlicedSprite.spriteName = "BSTGreenBtn";
						slicedSprite.spriteName = "BSTHasbeenused";
						slicedSprite.transform.localScale = new Vector3(143, 42, 1);
						currentSlicedSprite.transform.localScale = new Vector3(160, 72, 1);
						currentSlicedSprite = slicedSprite;
						//in my room
						if (User.Singleton.Friends == null || !User.Singleton.Friends.Exists(f => f.UserId == Room.Singleton.RoomData.Owner.UserId))
							PhotonClient.RoomTypeChanged+=ChangeRoomBackGround;
						
						User.Singleton.Save(User.Singleton.UserData.Avator, User.Singleton.UserData.Password, GetRoomTypeWithTag(currentSlicedSprite.tag));
					}
					else
					{
						//TODO: pop up dialog, say this screen you have used!
					}
					break;
					
				}
			}
		}
	}
	
	void BuySceneFinished(bool successed, int money, string appleStr, PayWay payWay)
	{
		if (buyBtn != null)
		{
			Transform[] alltrans=buyBtn.GetComponentsInChildren<Transform>();
		
			BoxCollider box = (BoxCollider)buyBtn.GetComponent<BoxCollider>();
			box.enabled = false;
			currentBox.enabled = true;
			currentBox = box;
			
			foreach(Transform trs in alltrans)
			{
				UISlicedSprite slicedSprite = (UISlicedSprite)trs.gameObject.GetComponent<UISlicedSprite>();
				switch(trs.gameObject.name)
				{
				case "buyBackground":
					if (slicedSprite.spriteName.Contains("BSTRedBtn"))
					{
						if (successed)
						{
							BuyItem.BuySceneFinished -= BuySceneFinished;
						 	Shop.Singleton.BuyRoom(GetRoomTypeWithTag(slicedSprite.tag), money, appleStr, payWay);
							User.Singleton.UserData.OwnRoomTypes |= GetRoomTypeWithTag(slicedSprite.tag);
							
							Debug.Log("slicedSprite's tag is: " + slicedSprite.tag);
							
							slicedSprite.spriteName = "BSTGreenBtn";
						}
						else
						{
							//IAP
							BuyItem.BuySceneFinished += BuySceneFinished;
							
							switch(buyBtn.transform.parent.name)
							{
							case "item1":
								StartBuy((int)RoomType.China, 18);
								break;
							case "item2":
								StartBuy((int)RoomType.Egypt, 68);
								break;
							case "item3":
								StartBuy((int)RoomType.Hawaii, 6);
								break;
							case "item4":
								StartBuy((int)RoomType.Japan, 12);
								break;
							case "item5":
								StartBuy((int)RoomType.West, 30);
								break;
							}
							
						}
					}
					else if (slicedSprite.spriteName == "BSTGreenBtn")
					{
						currentSlicedSprite.spriteName = "BSTGreenBtn";
						slicedSprite.spriteName = "BSTHasbeenused";
						slicedSprite.transform.localScale = new Vector3(143, 42, 1);
						currentSlicedSprite.transform.localScale = new Vector3(160, 72, 1);
						currentSlicedSprite = slicedSprite;
						//in my room
						if (User.Singleton.Friends == null || !User.Singleton.Friends.Exists(f => f.UserId == Room.Singleton.RoomData.Owner.UserId))
							PhotonClient.RoomTypeChanged+=ChangeRoomBackGround;
						
						User.Singleton.Save(User.Singleton.UserData.Avator, User.Singleton.UserData.Password, GetRoomTypeWithTag(currentSlicedSprite.tag));
					}
					else
					{
						//TODO: pop up dialog, say this screen you have used!
					}
					break;
					
				}
			}
		}
		
	}
	
	public void ChangeRoomBackGround()
	{
		ChangeBackGroundSence cbgs = gameObject.GetComponent<ChangeBackGroundSence>();
		cbgs.changeRoomBg(buyBtn.transform.parent.name);
		PhotonClient.RoomTypeChanged-=ChangeRoomBackGround;
	}
	
	void OnBuy(GameObject button)
	{
		buyBtn = button;
		BuySceneFinished(false);
	}
	
	void OnBuyWithChips(GameObject button)
	{
		buyBtn = button;
		BuySceneWithChips();
	}
	
	static public int SortByName (Transform a, Transform b) { return string.Compare(a.name, b.name); }

	public  void Reposition ()
	{
		Transform myTrans = transform;

		int x = 0;
		int y = 0;

		if (sorted)
		{
			List<Transform> list = new List<Transform>();

			for (int i = 0; i < myTrans.childCount; ++i) list.Add(myTrans.GetChild(i));
			list.Sort(SortByName);

			foreach (Transform t in list)
			{
				if (!t.gameObject.active && hideInactive) continue;

				t.localPosition = (arrangement == Arrangement.Horizontal) ?
					new Vector3(cellWidth * x, -cellHeight * y, 0f) :
					new Vector3(cellWidth * y, -cellHeight * x, 0f);

				if (++x >= maxPerLine && maxPerLine > 0)
				{
					x = 0;
					++y;
				}
			}
		}
		else
		{
			for (int i = 0; i < myTrans.childCount; ++i)
			{
				Transform t = myTrans.GetChild(i);

				if (!t.gameObject.active && hideInactive) continue;

				t.localPosition = (arrangement == Arrangement.Horizontal) ?
					new Vector3(cellWidth * x, -cellHeight * y, 0f) :
					new Vector3(cellWidth * y, -cellHeight * x, 0f);

				if (++x >= maxPerLine && maxPerLine > 0)
				{
					x = 0;
					++y;
				}
			}
		}
	}
	
	 
	/// <summary>
	/// Disableds all btns.
	/// </summary>
	void TransparentAllBtns(Transform me)
	{
		 Transform[] trs=transform.GetComponentsInChildren<Transform>();
		 foreach(Transform tr in trs)
		 {
			Debug.Log(tr.name);
			  if(tr!=me)
			  {
				
		        UIWidget widget=tr.GetComponent<UIWidget>();
				if(widget)
				{
					Color mColor=widget.color;
			        mColor.a=0;
					widget.color=mColor;
 		           // TweenColor.Begin(tr.gameObject, 0.01f, mColor);
				}
				
				 
				
 			  }
//			if(tr.name == "Bg")
//			{
//				tr.gameObject.SetActiveRecursively(false);
//			}
			BoxCollider colder= tr.GetComponent<BoxCollider>();
			if(colder!=null)
					colder.enabled=false;
		}
 	}
	
	
	void DisAbleAllBtns(bool iswork)
	{
		Transform[] trs=transform.GetComponentsInChildren<Transform>();
		foreach(Transform tr in trs)
		{
			BoxCollider box=tr.GetComponent<BoxCollider>();
			if(box!=null)
			{
				box.enabled=iswork;
			}
		}
	}
	
	/// <summary>
	/// Enables the allbtns.
	/// </summary>
	void NotTransparentAllbtns(Transform me)
	{
 		Transform[] trs=transform.GetComponentsInChildren<Transform>();
		 foreach(Transform tr in trs)
		 {
 		        UIWidget widget=tr.GetComponent<UIWidget>();
				if(widget)
				{
					Color mColor=widget.color;
			        mColor.a=1;
 		            TweenColor.Begin(tr.gameObject, 0.6f, mColor);
				}
//			if(tr.name == "Bg")
//			{
//				tr.gameObject.active=true;
//				tr.gameObject.SetActiveRecursively(true);
//			}
			
  		}
		 
	 
 	}
	 
	/// <summary>
	/// Ons the bigcomplete.
	/// </summary>
	/// <param name='tr'>
	/// Tr.
	/// </param>
	void onBigcomplete(Transform tr)
	{
		string spritename=tr.GetComponent<UISprite>().spriteName;
  		
		GameObject prefab=Resources.Load("prefab/MainSprite") as GameObject;
		GameObject	item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
			
		item.transform.parent=transform.parent.parent;// btn.transform.parent;
		item.transform.localPosition =new Vector3(2,0,-12);
		item.transform.localScale=new Vector3(AutoSizeScript.Width+1,AutoSizeScript.Height,1);
		
		if(spritename !=item.GetComponent<UISprite>().spriteName)
		{
 			GameObject preatlas=Resources.Load("BuySenceTable/"+spritename+"Atlas") as GameObject;
  			item.GetComponent<UISprite>().atlas=preatlas.GetComponent<UIAtlas>();
		}
 
		UIButtonMessage buMessage=item.GetComponent<UIButtonMessage>();
		buMessage.functionName="scalePictureToOrgin";
		buMessage.target=gameObject;
		 
	}
	 
	
	
	/// <summary>
	/// Scales the picture to screen.
	/// </summary>
	/// <param name='btn'>
	/// Button.
	/// </param>
	void scalePictureToScreen(GameObject btn)
	{
		if(scaled==false)
			scaled=true;
		else
			return;
		DisAbleAllBtns(false);
		
  		Transform[] trs=btn.GetComponentsInChildren<Transform>();
		foreach(Transform tr in trs)
		{
			if(tr.gameObject.name=="MainSprite" && origntransform ==null)
			{
			    origntransform =tr;
				orignPosition= new Vector3(tr.transform.localPosition.x,tr.transform.localPosition.y,tr.transform.localPosition.z);
 				orignScale= new Vector3(tr.transform.localScale.x,tr.transform.localScale.y,tr.transform.localScale.z);
				
				UIPanel uipanel=panel.GetComponent<UIPanel>();
				uipanel.clipping=UIDrawCall.Clipping.None;
 				
				TransparentAllBtns(tr);
 				iTween.RotateTo(tr.gameObject,iTween.Hash("rotation", new Vector3(0,0,6),"time",0.3f,"easetype","easeInOutQuad"));
				iTween.ScaleTo(tr.gameObject,iTween.Hash("scale",new Vector3(AutoSizeScript.Width,AutoSizeScript.Height,1),"time",0.3f,"easetype","easeInOutQuad","oncomplete","onBigcomplete","oncompletetarget",gameObject,"oncompleteparams",tr));
				iTween.MoveTo(tr.gameObject,iTween.Hash("position", new Vector3(0,0,0),"time",0.3f,"easetype","easeInOutQuad"));

			  	MusicManager.Singleton.BgAudio.Pause();
			    MusicManager.Singleton.PlayForeMusic(btn.name);
 			}
			
		}
		
	}
	/// <summary>
	/// Ons the smallcomplete.
	/// </summary>
	void onSmallcomplete(GameObject tr)
	{
		 
		Transform[] trs=transform.GetComponentsInChildren<Transform>();
		foreach(Transform atr in trs)
		{
			BoxCollider box=atr.GetComponent<BoxCollider>();
 		    if(box!=null)
			{
				box.enabled=true;
			}
		 	
		}
		
		UIPanel uipanel=panel.GetComponent<UIPanel>();
		uipanel.clipping=UIDrawCall.Clipping.SoftClip;
		uipanel.clipRange=new Vector4(0,100,936,460);
		uipanel.clipSoftness=new Vector2(40,40);
	
		DisAbleAllBtns(true);
	}
	
	void enableAllbtns(Transform panel)
	{
		BoxCollider[] boxcoolides =panel.GetComponentsInChildren<BoxCollider>();
		foreach(BoxCollider one in boxcoolides)
		{
			one.enabled=true;
		}
	}
    void fadeOut()
	{
		Transform panel= GameObject.Find("UI Root (2D)_FrontButton").transform.FindChild("Camera/Anchor/Panel");
		enableAllbtns(panel);
		Destroy(parentItem);
	}
	
	/// <summary>
	/// Scales the picture to orgin.
	/// </summary>
	/// <param name='btn'>
	/// Button.
	/// </param>
	void scalePictureToOrgin(GameObject btn)
	{
		
 	 
		Destroy(btn);
		 
		 if(scaled==true)
			scaled=false;
		 else
			return;
 		
	   iTween.RotateTo(origntransform.gameObject,iTween.Hash("rotation", new Vector3(0,0,0),"time",0.3f,"easetype","easeInOutQuad"));
	   iTween.ScaleTo(origntransform.gameObject,iTween.Hash("scale",orignScale,"time",0.3f,"easetype","easeInOutQuad","oncomplete","onSmallcomplete","oncompletetarget",gameObject,"oncompleteparams",origntransform.gameObject));
	   iTween.MoveTo(origntransform.gameObject,iTween.Hash("position", orignPosition,"time",0.3f,"easetype","easeInOutQuad","islocal",true));
		
	   NotTransparentAllbtns(origntransform);
		
	  	origntransform=null;
		MusicManager.Singleton.ForeAudio.Stop();
		MusicManager.Singleton.BgAudio.Play();
 	
	}
	
	void OnDestroy()
	{
		if(MusicManager.Singleton.ForeAudio!=null)
		{
			MusicManager.Singleton.ForeAudio.Stop();
		}
		MusicManager.Singleton.BgAudio.Play();
	}
	
}
