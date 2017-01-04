using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DataPersist;
using AssemblyCSharp;
using AssemblyCSharp.Helper;
using System;
using LilyHeart;
//using AssemblyCSharp;
//using DataPersist;


public class UINewGrid : MonoBehaviour {
	
	public enum Arrangement
	{
		Horizontal,
		Vertical,
	}
	
	public enum AvatorType:byte
	{
		Guest,
		DaHeng = 1,
		Songer,
		Captain,
		European,
		Qianjing,
		Dalaopo,
		Yitaitai,
		Luoli,
		AGe,
		General,//10
		Princess,
		Queen
	}

	public Arrangement arrangement = Arrangement.Horizontal;
	public int maxPerLine = 0;
	public float cellWidth = 200f;
	public float cellHeight = 200f;
	
	public bool sorted = false;
	public bool hideInactive = true;
	
	public GameObject searchPanel;
	
	public GameObject Panel;
	public GameObject SearchKeyword;
	public GameObject NoResult;
	public GameObject LabelBG;
	public GameObject LabelFG;
	public GameObject NoFriend;
	public GameObject parentItem;
	public Vector3 defaultPosition = Vector3.zero;
	public Vector3 initPosition = new Vector3(366, 256, -1);
	public GameObject BtnSearch;
	
	private string searchKey = string.Empty;
	private bool repositionNow = false;
	
	public bool UseRriendCell2=false;
	GameObject addbtn;
	private bool bIsSearch = false;
	private Vector3 FirstItemPosition;
	public int nFriendCount = -1;
	// Use this for initialization
    void Start ()
	{
	 	List<UserData> friends = Player.Singleton.Friends;
		if(friends!=null&&friends.Count>0)
			nFriendCount = friends.Count;
	    StartCoroutine(addItems(friends, false));
		Reposition();
		transform.localPosition = initPosition;
		PhotonClient.Singleton.bSendUserID = string.Empty;
		PhotonClient.Singleton.lSendchip = 0;
		PhotonClient.SendChipMessageFinishEvent += UpdateAfterSendChip;
	}
	private void UpdateAfterSendChip()
	{
		UpdateFriendChipInFriendList();
	}
	private void UpdateFriendChipInFriendList()
	{
		List<UserData> friends = Player.Singleton.Friends;
		string strFriendUserID = PhotonClient.Singleton.bSendUserID;
		long sendChips = PhotonClient.Singleton.lSendchip;
		UserData friend;
		
		if(string.IsNullOrEmpty(strFriendUserID)||sendChips==0) return;
		
		Transform transform = gameObject.transform;
		
		Transform [] allTrans = transform.GetComponentsInChildren<Transform>();
		foreach(Transform trs in allTrans)
		{
			if(trs.name == "item_"+strFriendUserID)
			{
				Transform [] allTranss = trs.GetComponentsInChildren<Transform>();
				foreach(Transform trss in allTranss)
				{
					if(trss.name == "Label_chips")
					{
						UILabel label = (UILabel)trss.GetComponent<UILabel>();
						if(friends!=null&&friends.Count>0)
						{
						    friend = friends.Find(f=>f.UserId == strFriendUserID);
							if(friend!=null)
							{
								friend.Chips += sendChips;
								if(label!=null)
									label.text = friend.Chips >= 100 ? string.Format("{0:0,00}",friend.Chips) : friend.Chips.ToString();
							}
						}
						break;
					}
				}
				break;
			}
		}
	}
	//Add by john wu 2012/08/14
	private void AddDefaultAvatorAfterSearch(List<UserData> friends)
	{
		if(friends!=null)
		{
			foreach(UserData friend in friends)
			{
				Debug.Log(friend.Avator.ToString());
				if(friend.Avator == 0)
					friend.Avator = (byte)PlayerAvator.DaHeng;
			}
		}
	}
	//End-Add
	private string GetAvatorString(UserData friend)
	{
		string result = string.Empty;
		
		result = ((AvatorType)friend.Avator).ToString();
		
		return result;
	}
	
	private string GetHonorByUserData(UserData data)
	{
		string result = string.Empty;
		
		if (data == null)
			return result;
		
		HonorType honor = HonorHelper.GetHonorRecuise(data, HonorType.Citizen);
		
		switch(honor)
		{
		case HonorType.Citizen:
			result = honor.ToString();
			break;
		case HonorType.Knight:
			result = honor.ToString();
			break;
		case HonorType.Baron:
			result = honor.ToString();
			break;
		case HonorType.Vicomte:
			result = "Viscount";
			break;
		case HonorType.Comte:
			result = "Count";
			break;
		case HonorType.Marquis:
			result = honor.ToString();
			break;
		case HonorType.Duke:
			result = honor.ToString();
			break;
		case HonorType.Archduke:
			result = "GrandDuke";
			break;
		case HonorType.Infante:
			result = "Crownprince";
			break;
		case HonorType.Prince:
			result = honor.ToString();
			break;
		case HonorType.King:
			if ((int)data.Avator <= (int)PlayerAvator.Guest)
				result = honor.ToString();
			else
				result = "Queen";
			break;
		default:
			break;
		}
		
		return result;
	}
	
	private string GetUserStatus(UserData data)
	{
		string result = string.Empty;
		
		switch(data.Status)
		{
		case UserStatus.Idle:
			result = "LEDgreen";
			break;
		case UserStatus.Offline:
			result = "LEDgray";
			break;
		case UserStatus.Playing:
			result = "LEDred";
			break;
		default:
			result = "LEDblue";
			break;
		}
			
		return result;
	}
	
	public void addItemgs()
	{
		for(int i=0;i<1;i++)
		{
 			GameObject prefab=Resources.Load(UseRriendCell2?"prefab/cardFriend/FriendCell2":"prefab/cardFriend/FriendCell") as GameObject;
			GameObject item = Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
			
 			item.transform.parent=transform;
 		    item.transform.localScale=new Vector3(1f,1f,1f);
		    item.transform.localPosition=new Vector3(0,0,0);
		}
	}
	
	IEnumerator addItems(List<UserData> friends, bool search)
	{
		DestroyAllItems();
		
		if (friends == null || friends.Count == 0)
		{
			if (search)
			{
				NoResult.transform.localPosition = new Vector3(4.306857f, -59.40472f, 0);
				
				UILabel bgLabel = LabelBG.GetComponent<UILabel>();
				bgLabel.text = string.Format(LocalizeHelper.Translate("CANNOT_FIND_FRIEND_BG_LABEL"), searchKey); 
				UILabel fgLabel = LabelFG.GetComponent<UILabel>();
				fgLabel.text = string.Format(LocalizeHelper.Translate("CANNOT_FIND_FRIEND_FG_LABEL"), searchKey);

				LabelBG.transform.localPosition = new Vector3(-333.6852f, 97.25842f, -3);
				LabelFG.transform.localPosition = new Vector3(-333.6852f, 95.25842f, -3);
			}
			else
				NoFriend.transform.localPosition = new Vector3(-2.173347f, -50.57951f, 0);
			yield break;
		}
		
		List<UserData> result=new List<UserData>(friends);
		if(sorted)
		{
			result.Sort((p1,p2)=>string.Compare(p1.NickName,p2.NickName));
		}
		
		int count=1;
		int x = 0;
		int y = 0;
		foreach(UserData friend in result)
		{
			if(count%20==0)
			{
				yield return new WaitForEndOfFrame();
			}
			GameObject prefab;
			if (GlobalManager.Singleton.version == KindOfVersion.Ultimate)
				prefab=Resources.Load(search ? "prefab/cardFriend/FriendCell3" : UseRriendCell2 ?"prefab/cardFriend/FriendCell2":"prefab/cardFriend/FriendCell") as GameObject;
			else
				prefab=Resources.Load(search ? "prefab/cardFriend/FriendCell3" : UseRriendCell2 ?"prefab/cardFriend/FriendCell2":"prefab/cardFriend/FriendCell_simple") as GameObject;
			GameObject item = Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
			
			Transform[] alltrans=item.GetComponentsInChildren<Transform>();
			
			foreach(Transform trs in alltrans)
			{
				UILabel label = (UILabel)trs.gameObject.GetComponent<UILabel>();
				UISlicedSprite slicedSprite = (UISlicedSprite)trs.gameObject.GetComponent<UISlicedSprite>();
				switch(trs.gameObject.name)
				{
				case "Label_name":
					label.text = friend.NickName;
					break;
				case "Label_chips":
					label.text = friend.Chips >= 100 ? string.Format("{0:0,00}",friend.Chips) : friend.Chips.ToString();
					break;
				case "ChipsSprite":
					int level = HonorHelper.GetChipLevel(friend.Chips, 1);
					slicedSprite.spriteName = HonorHelper.GetChipString(level);
					break;
				case "AvatorSprite":
						slicedSprite.spriteName = GetAvatorString(friend);
					break;
				case "StatusSprite":
					slicedSprite.spriteName = GetUserStatus(friend);
					break;
				case "HonorSprite":
					slicedSprite.spriteName = GetHonorByUserData(friend);
					break;
				}
			}
			
			cardFirend cfriend = (cardFirend)item.GetComponent<cardFirend>();
			cfriend.friend = friend;
			
			item.name="item_" + friend.UserId;
			item.transform.parent=transform;
 		    item.transform.localScale=new Vector3(1f,1f,1f);
		    item.transform.localPosition=new Vector3(0,0,0);
			
			UIDragObject iDragobject=item.GetComponent<UIDragObject>();
			if(iDragobject!=null)
				iDragobject.target=transform;
			
			UIMyDragObject dragobject=item.GetComponent<UIMyDragObject>();
			dragobject.target=transform;
			if(friends.Count>5)
			{
				dragobject.momentumAmount = Mathf.Abs(dragobject.momentumAmount);
				dragobject.NeedDampen = true;
			}
			else
			{
				if(dragobject.momentumAmount > 0f)
					dragobject.momentumAmount = -dragobject.momentumAmount;
				dragobject.NeedDampen = false;
			}

			if (!item.active && hideInactive) continue;
			
			item.transform.localPosition = (arrangement == Arrangement.Horizontal) ?
				new Vector3(cellWidth * x, -cellHeight * y, 0f) :
				new Vector3(cellWidth * y, -cellHeight * x, 0f);
			Debug.Log("localPosition"+item.transform.localPosition.ToString());
			
			if(search)
			{
				item.transform.localPosition = (arrangement == Arrangement.Horizontal) ?
				new Vector3(cellWidth * x, -cellHeight * (y) - 50f, 0f) :
				new Vector3(cellWidth * y, -cellHeight * (x) - 50f, 0f);
				if(FirstItemPosition == Vector3.zero)
					FirstItemPosition = item.transform.localPosition;
			}
			
			if (++x >= maxPerLine && maxPerLine > 0)
			{
				x = 0;
				++y;
			}
			count++;
		}
		
		SpringPosition sp = GetComponent<SpringPosition>();
		if (sp != null)
			GameObject.DestroyImmediate(sp);
		
		transform.localPosition = defaultPosition;
		
	}
	void OnDestroy()
	{
		PhotonClient.SendChipMessageFinishEvent -= UpdateAfterSendChip;
		StopAllCoroutines();
	}
	void CloseDialog(GameObject btn)
	{
		if (btn)
		{
			BoxCollider box = btn.GetComponent<BoxCollider>();
			box.enabled = false;
			Destroy(btn);
		}
		
		fadePanel panel = parentItem.GetComponent<fadePanel>();
		panel.fadeOut(null);
	}
	
	void btnSenderChips()
	{
//		string friendUserID = PhotonClient.Singleton.bSendUserID;
//		long sendChip = PhotonClient.Singleton.lSendchip;
//		if(!string.IsNullOrEmpty(friendUserID)&&sendChip!=0)
//		{
//			GameObject prefab=Resources.Load("prefab/cardFriend/FriendCell") as GameObject;
//			
//			Transform[] alltrans=prefab.GetComponentsInChildren<Transform>();
//			
//			foreach(Transform trs in alltrans)
//			{
//				UILabel label = (UILabel)trs.gameObject.GetComponent<UILabel>();
//			}
//		}
	}
	
	/// <summary>
	/// Add Card friend  
	/// </summary>
	/// 
	void btnAddCardFriend(GameObject btnAddfriend)
	{
		DestroyAllItems();
		addbtn=btnAddfriend;
		addbtn.transform.localPosition =new Vector3(379.66530f,464.8802f,-1f);
		
		
		Panel.transform.localPosition=new Vector3(0,-46.23429f,0);
		UIPanel uipanle=Panel.GetComponent<UIPanel>();
 		uipanle.clipRange=new Vector4(0,-60,937,440);
			
		Reposition();
		  
		searchPanel.transform.localPosition =new Vector3(1001.637f,-0.007027387f,-1);
		
		searchPanel.transform.localScale =new Vector3(1,1,1);
		
		NoFriend.transform.localPosition = new Vector3(1000, 0, 0);
		//searchPanel.active=true; 
 	}
//	void disabledAllBtns()
//	{
//		BoxCollider[] boxcoolides = transform.parent.parent.gameObject.GetComponentsInChildren<BoxCollider>();//parentItem
//		foreach(BoxCollider one in boxcoolides)
//		{
//			one.enabled=false;
//		}
//	}
	void PopUpTips()
	{
		//disabledAllBtns();
		GameObject prefab=Resources.Load("prefab/tips2") as GameObject;
		GameObject item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		
		item.transform.parent=transform.parent.parent.parent.parent;
		item.transform.localPosition=new Vector3(0,200,-20);
		item.transform.localScale =new Vector3(1,1,1);
	}
	
	void btnStartSearchFriend()
	{
		UILabel label = (UILabel)SearchKeyword.GetComponent<UILabel>();
		string keyword = label.text;
		
		Debug.Log("keyword is: " + keyword);
		
		if (keyword == null || keyword == string.Empty)
		{
			GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new SearchFailedDialog());
			PopUpTips();
			return;
		}
		
		searchKey = keyword;
		
		DestroyAllItems();
		if (SearchKeyword != null)
		{
			NoResult.transform.localPosition = new Vector3(1000,0, 0);
			LabelBG.transform.localPosition = new Vector3(1000, 200, 0);
			LabelFG.transform.localPosition = new Vector3(1000, 400, 0);
			NoFriend.transform.localPosition = new Vector3(1000, 0, 0);
			
			PhotonClient.SearchUserFinished += SearchUserDidFinished;
			MaskingTable table = transform.parent.parent.parent.parent.gameObject.AddComponent<MaskingTable>();
			table.SetCallback(transform.parent.parent.parent.parent.gameObject, "retrySearch");
			
			Debug.Log("keyword: " + keyword);
			Player.Singleton.SearchUser(keyword);
			
			if(BtnSearch!=null)
			{
				BtnSearch.collider.enabled=false;
			}
		}
	}
	
	void retrySearch()
	{
		Player.Singleton.SearchUser(searchKey);
	}
	
	void SearchUserDidFinished()
	{
		Debug.Log("SearchUserDidFinished");
		PhotonClient.SearchUserFinished -= SearchUserDidFinished;
		AddDefaultAvatorAfterSearch(GlobalManager.Singleton.SearchUsers);
		UtilityHelper.CloseMaskingTable();
		//if (GlobalManager.Singleton.SearchUsers != null && GlobalManager.Singleton.SearchUsers.Count > 0)
		{	
			FirstItemPosition = Vector3.zero;
			StartCoroutine(addItems(GlobalManager.Singleton.SearchUsers, true));
			Debug.Log("SearchUserDidFinished"+transform.localPosition.ToString());
			bIsSearch = true;
		}
		
	    if(BtnSearch!=null)
		{
			BtnSearch.collider.enabled=true;
		}
	}
	private void DestroyAllItems()
	{	
		Transform myTrans = transform;
		List<Transform> list = new List<Transform>();
		
		for (int i = 0; i < myTrans.childCount; ++i) 
			list.Add(myTrans.GetChild(i));
		
		for (int i = (myTrans.childCount - 1); i > -1; i--)
		{
			Destroy(list[i].gameObject);
		}
		
		Reposition();
		gameObject.transform.localPosition = defaultPosition; 
	}
	
	/// <summary>
	/// Out of Search  .
	/// </summary>/
	/// 
	void  SeachFriendEsc()
	{
		Panel.transform.localPosition=new Vector3(0,0,0);
		UIPanel uipanle=Panel.GetComponent<UIPanel>();
 		uipanle.clipRange=new Vector4(0,-60,937,525);
		addbtn.transform.localPosition =new Vector3(379.66530f,264.8802f,-1f);
		searchPanel.transform.localPosition =new Vector3(2086.09f,-0.007027387f,-1);
		NoResult.transform.localPosition = new Vector3(1000,0, 0);
		LabelBG.transform.localPosition = new Vector3(1000, 200, 0);
		LabelFG.transform.localPosition = new Vector3(1000, 400, 0);
		NoFriend.transform.localPosition = new Vector3(1000, 0, 0);
		
		StartCoroutine(addItems(User.Singleton.Friends, false));
		
		bIsSearch = false;
		
		//Reposition ();
		//searchPanel.active=false;
	}
	
	IEnumerator NetworkUpdate()
	{
		PhotonClient.Singleton.Update();
		yield return null;
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
	static public int SortByName (Transform a, Transform b) { return string.Compare(a.name, b.name); }

	public  void Reposition ()
	{
		Transform myTrans = transform;
		int x = 0;
		int y = 0;
		for (int i = 0; i < myTrans.childCount; i++)
		{
			Transform t = myTrans.GetChild(i);
			if (!t.gameObject.active && hideInactive)
				continue;
			if(t.name.IndexOf("item_")==-1)
				continue;
			t.localPosition = (arrangement == Arrangement.Horizontal) ?
				new Vector3(cellWidth * x, -cellHeight * y, 0f) :
				new Vector3(cellWidth * y, -cellHeight * x, 0f);
			
			if(nFriendCount<20&&nFriendCount>0)
				t.localPosition = new Vector3 (t.localPosition.x,t.localPosition.y-cellHeight,t.localPosition.z);
			
			if (++x >= maxPerLine && maxPerLine > 0)
			{
				x = 0;
				++y;
			}
		}
		SpringPosition sp = GetComponent<SpringPosition>();
		if (sp != null)
			GameObject.DestroyImmediate(sp);
		
		transform.localPosition = defaultPosition;
	}
}
