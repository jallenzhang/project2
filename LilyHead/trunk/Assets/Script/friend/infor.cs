using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using DataPersist;
using System.Text;
using System.Collections.Generic;
using LilyHeart;

public class infor : MonoBehaviour {
	
	public GameObject personInfor;
	public GameObject senderchips;
	// Use this for initialization
	GameObject item=null;
	GameObject item2=null;
	public GameObject IconDragon;
	public GameObject IconHat;
	public GameObject IconVIP;
	public GameObject AvatarPic;
	public GameObject ChooseRoleBtn;
	
	public List<Vector3> IconDragonPositions = new List<Vector3>();
	public List<Vector3> IconHatPositions = new List<Vector3>();
	public List<Vector3> IconVIPPositions = new List<Vector3>();
	public List<Vector3> AvatarPicPositions = new List<Vector3>();
	
	public GameObject NameObject = null;
	public GameObject ChipsLableObject = null;
	public GameObject GamePlayedObject = null;
	public GameObject LevelObject = null;
	public GameObject RatioObject = null;
	public GameObject WinGamesObject = null;
	public GameObject BiggestBetObject = null;
	public GameObject AvatorSpriteObject = null;
	public GameObject ExpObject = null;
	public GameObject TitleObject = null;
	public GameObject PPBForegroundObject = null;
	
	public List <GameObject> CardSpriteObject;
	public List<Vector3> CardSpritePositions = new List<Vector3>();
	public GameObject SlicedSpritePITPic_5Object = null;
	public Vector3 SlicedSpritePITPic_5Position;
	
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
	void showPersonalInofr()
	{
		if(item==null)
		{
		   item=Instantiate(personInfor) as GameObject;
 		   item.name="item 0";
  		   item.transform.parent=transform;
		   item.transform.localPosition=new Vector3(0,0,0);
 		   item.transform.localScale=new Vector3(1f,1f,1f);
		}
		else
		{
			Destroy(item);
			item=null;
		}
	}
	void showSenderChips()
	{
		if(item2==null)
		{
		   item2=Instantiate(senderchips) as GameObject;
 		   item2.name="item 0";
  		   item2.transform.parent=transform;
		   item2.transform.localPosition=new Vector3(0,0,0);
 		   item2.transform.localScale=new Vector3(1f,1f,1f);
		}
		else
		{
			Destroy(item2);
			item2=null;
		}
	}
	UIAtlas getAtlas(string tile)
	{
		string path=string.Format("Game Lobby/{0}Atlas",tile);
 		GameObject loadaltalsObject=Resources.Load(path) as GameObject;
		return loadaltalsObject.GetComponent<UIAtlas>();
	}
	private void InitThreeButtons(UserData userdata)
	{
		bool bIsOpenKangXi = false;
		bool bIsLineage = false;
		bool bIsVip = false;
		
		bIsOpenKangXi = userdata.Jade;
		bIsLineage = userdata.LineAge;
		if(userdata.VIP >0) bIsVip = true;
	    
		DoUpdateBtnBackgroup(IconDragon,"TableBtn_dragon",bIsOpenKangXi);
		DoUpdateBtnBackgroup(IconHat,"TableBtn_hat",bIsLineage);
		DoUpdateBtnBackgroup(IconVIP,"TableBtn_Vip",bIsVip);
	}
	private  void  DoUpdateBtnBackgroup(GameObject Btn, string backgroupName,bool isOpen)
	{
		if(Btn!=null)
		{
			UISprite backgroup =  Btn.GetComponent<UISprite>();
			if(backgroup!=null&&!string.IsNullOrEmpty(backgroupName))
			{
				if(backgroup.atlas==null)
					backgroup.atlas = getAtlas("GameLobbv");
				if(!isOpen)
					backgroup.spriteName = backgroupName + "_dark";
				else
					backgroup.spriteName = backgroupName;
			}
		}
	}
	private string FormatString(long lvalue)
	{
		if (lvalue < 100)
			return lvalue.ToString();
		
		return string.Format("{0:0,00}",lvalue);
	}
	
	private UIFilledSprite fuckedFilledSprite = null;
	private float currentFillAmount = 0.0f;
	
	void Start () {
		User.UserDataChangedEvent += UpdataInfos;
		UpdataInfos();
	}
	
	void UpdateStatusButtons()
	{
		if (IconDragonPositions.Count > 0 
			&& IconHatPositions.Count > 0 
			&& IconVIPPositions.Count > 0
			&& AvatarPicPositions.Count > 0
			&& ChooseRoleBtn != null 
			&& IconDragon != null
			&& IconHat != null
			&& IconVIP != null
			&& AvatarPic != null)
		{
			if (User.Singleton.CurrentPlayInfo == 1)
			{
				IconDragon.transform.localPosition = IconDragonPositions[0];
				IconHat.transform.localPosition = IconHatPositions[0];
				IconVIP.transform.localPosition = IconVIPPositions[0];
				AvatarPic.transform.localPosition = AvatarPicPositions[0];
				ChooseRoleBtn.SetActiveRecursively(true);
			}
			else
			{
				IconDragon.transform.localPosition = IconDragonPositions[1];
				IconHat.transform.localPosition = IconHatPositions[1];
				IconVIP.transform.localPosition = IconVIPPositions[1];
				AvatarPic.transform.localPosition = AvatarPicPositions[1];
				ChooseRoleBtn.SetActiveRecursively(false);
			}
		}
	}
	void UpdataInfos()
	{
		UserData userData = null;
		if(User.Singleton.CurrentPlayInfo == 2)
			userData = Room.Singleton.RoomData.Owner;
		else
			userData = User.Singleton.UserData;
		
		InitThreeButtons(userData);
		UpdateStatusButtons();
		
		int level = 0;
		long expNeeded = 0;
		
		string[] cards = null;
		if (!string.IsNullOrEmpty(userData.BestHand))
		{
			Debug.Log("@@@@@@@@ " + userData.BestHand + "@@@@@");
			cards = userData.BestHand.ToLower().Split(' ');
			if (cards.Length != 5)
				cards = null;
		}
		StringBuilder builder;
		if(NameObject!=null)
		{   
			SetGameObjectLableText(NameObject,userData.NickName);
		}
		if(ChipsLableObject!=null)
			SetGameObjectLableText(ChipsLableObject,FormatString(userData.Chips));
		if(GamePlayedObject!=null)
			SetGameObjectLableText(GamePlayedObject,FormatString(userData.HandsPlayed));
		if(LevelObject!=null)
		{
				builder = new StringBuilder("Lv.");
				if (userData.Level == 0)
					builder.Append("1").ToString();
				else
					builder.Append(userData.Level).ToString();
			SetGameObjectLableText(LevelObject,builder.ToString());
		}
		if(RatioObject!=null)
		{
			builder = new StringBuilder((userData.WinRatio * 100).ToString());
			builder.Append("%").ToString();
			SetGameObjectLableText(RatioObject,builder.ToString());
		}
		if(WinGamesObject!=null)
		{
			SetGameObjectLableText(WinGamesObject,FormatString(userData.HandsWon));
		}
		if(BiggestBetObject!=null)
		{
			string str = string.Empty;
			if (userData.BiggestWin > 1000000)
				str=string.Format("{0:N1}M",(userData.BiggestWin/1000000.0f));
			else if(userData.BiggestWin>=1000)
			    str=string.Format("{0:N1}K",(userData.BiggestWin/1000.0f));
			else
				str=string.Format("{0}",(userData.BiggestWin));
			SetGameObjectLableText(BiggestBetObject,str);
		}
		if(AvatorSpriteObject!=null)
		{
			UISprite sprite = AvatorSpriteObject.GetComponent<UISprite>();
			sprite.spriteName = ((AvatorType)userData.Avator).ToString();
		}
		if(ExpObject!=null)
		{
			level = userData.Level == 0 ? 1 : userData.Level;
			expNeeded = (long)((level * level) * (level + 4)/3);
			builder = new StringBuilder();
			builder.Append(userData.LevelExp.ToString());
			builder.Append("/");
			builder.Append(expNeeded.ToString());
			SetGameObjectLableText(ExpObject,builder.ToString());
		}
		if(TitleObject!=null)
		{
			UISlicedSprite slicedSprite = TitleObject.GetComponent<UISlicedSprite>();
			HonorType type = HonorHelper.GetHonorRecuise(userData, HonorType.Citizen);
			slicedSprite.spriteName = HonorHelper.GetHonorPicString(type, userData.Avator);
		}
		if(PPBForegroundObject!=null)
		{
			level = userData.Level == 0 ? 1 : userData.Level;
			expNeeded = (long)((level * level) * (level + 4)/3);
				
			UIFilledSprite fillSprite = PPBForegroundObject.GetComponent<UIFilledSprite>();
			fillSprite.fillAmount = (float)((float)userData.LevelExp/(float)expNeeded);
			fuckedFilledSprite = fillSprite;
			currentFillAmount = fillSprite.fillAmount;
		}
		if(CardSpriteObject!=null&&CardSpriteObject.Count>0)
		{
			GameObject cardSpriteObject;
			Vector3  cardSpritePosition;
			UISlicedSprite slicedSprite;
			if(cards!=null)
			{
				for(int i = 0;i<CardSpriteObject.Count;i++)
				{
					cardSpriteObject =  CardSpriteObject[i];
					cardSpritePosition = CardSpritePositions[i];
					if(cardSpriteObject!=null&&cardSpritePosition!=Vector3.zero)
					{
						slicedSprite = cardSpriteObject.GetComponent<UISlicedSprite>();
						if(cards!=null&&i<cards.Length&&!string.IsNullOrEmpty(cards[i]))
						{
							slicedSprite.spriteName = cards[i];
						}
						cardSpriteObject.transform.localPosition = cardSpritePosition;
					}
				}
			}
		}
		if(SlicedSpritePITPic_5Object!=null)
		{
			if(cards == null)
				SlicedSpritePITPic_5Object.transform.localPosition = SlicedSpritePITPic_5Position;
		}
	}
//	void UpdataInfos()
//	{
//		UserData userData = null;
//		if(User.Singleton.CurrentPlayInfo == 2)
//			userData = Room.Singleton.RoomData.Owner;
//		else
//			userData = User.Singleton.UserData;
//		
//		InitThreeButtons(userData);
//		UpdateStatusButtons();
//		
//		int level = 0;
//		long expNeeded = 0;
//		
//		string[] cards = null;
//		if (!string.IsNullOrEmpty(userData.BestHand))
//		{
//			Debug.Log("@@@@@@@@ " + userData.BestHand + "@@@@@");
//			cards = userData.BestHand.ToLower().Split(' ');
//			if (cards.Length != 5)
//				cards = null;
//		}
//		
//		Transform trn=personInfor.transform;
//		personInfor.transform.localPosition=new Vector3(trn.localPosition.x,2,transform.localPosition.z);
//		
//		Transform[] alltrans=personInfor.GetComponentsInChildren<Transform>();
//		foreach(Transform trs in alltrans)
//		{
//			UILabel label =trs.gameObject.GetComponent<UILabel>(); 
//			UISlicedSprite slicedSprite = trs.gameObject.GetComponent<UISlicedSprite>();
//			StringBuilder builder;
//			switch(trs.gameObject.name)
//			{
//			case "Name":
//				label.text = userData.NickName;
//				break;
//			case "Chips_lable":
//				label.text = FormatString(userData.Chips);
//				break;
//			case "GamePlayed":
//				label.text = FormatString(userData.HandsPlayed);
//				break;
//			case "Level":
//				builder = new StringBuilder("Lv.");
//				if (userData.Level == 0)
//					label.text = builder.Append("1").ToString();
//				else
//					label.text = builder.Append(userData.Level.ToString()).ToString();
//				break;
//			case "Ratio":
//				builder = new StringBuilder((userData.WinRatio * 100).ToString());
//				label.text = builder.Append("%").ToString();
//				break;
//			case "WinGames":
//				label.text = FormatString(userData.HandsWon);
//				break;
//			case "BiggestBet":
//				string str = string.Empty;
//				if (userData.BiggestWin > 1000000)
//					str=string.Format("{0:N1}M",(userData.BiggestWin/1000000.0f));
//				else if(userData.BiggestWin>=1000)
//			    	str=string.Format("{0:N1}K",(userData.BiggestWin/1000.0f));
//				else
//					str=string.Format("{0}",(userData.BiggestWin));
//				label.text = str;
//				break;
//			case "AvatorSprite":
//				UISprite sprite = trs.gameObject.GetComponent<UISprite>();
//				
//				sprite.spriteName = ((AvatorType)userData.Avator).ToString();
//				break;
//			case "Exp":
//				level = userData.Level == 0 ? 1 : userData.Level;
//				expNeeded = (long)((level * level) * (level + 4)/3);
//				builder = new StringBuilder();
//				builder.Append(userData.LevelExp.ToString());
//				builder.Append("/");
//				builder.Append(expNeeded.ToString());
//				label.text = builder.ToString();
//				break;
//			case "title":
//				HonorType type = HonorHelper.GetHonorRecuise(userData, HonorType.Citizen);
//				slicedSprite.spriteName = HonorHelper.GetHonorPicString(type, userData.Avator);
//				break;
//			case "PPBForeground":
//			{
//				level = userData.Level == 0 ? 1 : userData.Level;
//				expNeeded = (long)((level * level) * (level + 4)/3);
//				
//				UIFilledSprite fillSprite = trs.gameObject.GetComponent<UIFilledSprite>();
//				fillSprite.fillAmount = (float)((float)userData.LevelExp/(float)expNeeded);
//				fuckedFilledSprite = fillSprite;
//				currentFillAmount = fillSprite.fillAmount;
//				break;
//			}
//			case "Card1Sprite":
//				if (cards != null)
//				{
//					slicedSprite.spriteName = cards[0];
//					trs.gameObject.transform.localPosition = new Vector3(-366, -137, -1);
//				}
//				break;
//			case "Card2Sprite":
//				if (cards != null)
//				{
//					slicedSprite.spriteName = cards[1];
//					trs.gameObject.transform.localPosition = new Vector3(-313, -137, -1);
//				}
//				break;
//			case "Card3Sprite":
//				if (cards != null)
//				{
//					slicedSprite.spriteName = cards[2];
//					trs.gameObject.transform.localPosition = new Vector3(-256, -137, -1);
//				}
//				break;
//			case "Card4Sprite":
//				if (cards != null)
//				{
//					slicedSprite.spriteName = cards[3];
//					trs.gameObject.transform.localPosition = new Vector3(-197, -137, -1);
//				}
//				break;
//			case "Card5Sprite":
//				if (cards != null)
//				{
//					slicedSprite.spriteName = cards[4];
//					trs.gameObject.transform.localPosition = new Vector3(-137, -137, -1);
//				}
//				break;
//			case "SlicedSprite (PITPic_5)":
//				if (cards == null)
//				{
//					trs.gameObject.transform.localPosition = new Vector3(-256.8155f, -130.935f, 0);
//				}
//				break;
//			default:
//				break;
//			}
// 		}
//	}
	private void SetGameObjectLableText(GameObject gameObject,string strText)
	{
		if(gameObject!=null&&!string.IsNullOrEmpty(strText))
		{
			UILabel label =  gameObject.GetComponent<UILabel>();
			if(label!=null)
				label.text = strText;
		}
	}
	// Update is called once per frame
	void Update () {
		if (fuckedFilledSprite != null)
			fuckedFilledSprite.fillAmount = currentFillAmount;
	}
	//show user's personal information
	void showInfor()
	{
		UserData userdata = User.Singleton.UserData;
		Transform trn=personInfor.transform;
		personInfor.transform.localPosition=new Vector3(trn.localPosition.x,2,transform.localPosition.z);
		if(NameObject!=null)
		{
			SetGameObjectLableText(NameObject,userdata.NickName);
		}
		if(RatioObject!=null)
		{
			SetGameObjectLableText(RatioObject,userdata.WinRatio.ToString());
		}
	}
	void OnDestroy()
	{
		User.UserDataChangedEvent -= UpdataInfos;
	}
}
