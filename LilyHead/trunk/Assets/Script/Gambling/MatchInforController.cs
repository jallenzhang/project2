using UnityEngine;
using System.Collections;
using System;
using AssemblyCSharp;
using DataPersist;
using DataPersist.CardGame;
using System.Collections.Generic;
using EasyMotion2D;
using System.Timers;
using AssemblyCSharp.Helper;
using System.Text.RegularExpressions;
using System.Linq;
using LilyHeart;

public class MatchStartTipDialog : DialogInfo {
	public MatchStartTipDialog(string fre)
	{
		this.Title = LocalizeHelper.Translate("MATCH_TIP_TITLE");
		this.Description = string.Format(LocalizeHelper.Translate("MATCH_TIP_DESCRIPTION"),fre);
		this.Buttons = 0;
	}
	
	public override void Process()
	{
 		 
	}
}
public class BigBlindAmntChangeDialog : DialogInfo {
	public BigBlindAmntChangeDialog(int BigBlindamnt)
	{
		this.Title = LocalizeHelper.Translate("BLINDAMNT");
		this.Description =string.Format(LocalizeHelper.Translate("BLINDAMNTANDSMALLAMNT"), Util.getLableMoneyK_MModel(BigBlindamnt)+"/"+Util.getLableMoneyK_MModel(BigBlindamnt/2)) ;
		this.Buttons = 0;
	}
	
	public override void Process()
	{
 		 
	}
}
public class MatchInforController : MonoBehaviour {

		AsyncOperation async = null;
	
	public static bool hasJoined=false;
	public static MatchInforController Singleton {get;private set;}
	public GameObject Button_Jia;
	public GameObject SliderChip;
	public GameObject BlackMask;
	public GameObject BigHandCard;
	
	public GameObject PublicCard;
	
	public GameObject totalchipbg;
   	
   	public GameObject topPanel;
	
	public GameObject wintitlebg_Label; 
	
	public GameObject blackMask;
	
	public bool HasMatchStarted=false;
		
	public AudioSource AudioSource {get;set;}
   	private int preWinindex=-1;
 	 
	static int numOfCheckInTable=0;
	
	private int numOfWonPots=1;
  	
	bool hasJoinGamed=false;
	
	
	
  	
	bool shouldPopUpAchiveDialog = false;
	bool shouldPopUpUpgradeDialog = false;
	bool upgradeDialogAlreadyPopUp = false;
	bool achiveDialogAlreadyPopUp = false;
	
 //	bool NeedToShowTheViewWaitingForNext=false;
	
	//bool hasReSetGameState=false;
	//private ChaBarData barChaBarData{set;get;}
   	
    public bool LoseWinAndStandup=false;
	public GameObject Button_Speak;
	public GameObject Button_Reward;
	//remember for Game statistics info
	public int NWonGameNum {get;set;}
	public int NPlayGameNum {get;set;}
	public long lGetInGameChip {get;set;}
	public long lNowChip {get;set;}
	public long lWinInGameChip {get;set;}
	//end - remember
	
	
	public static bool BreakBiggestWonPot=false;
	
	//public byte[] RankList{get;set;}
  	
	public int RankPlace=5;
  	
 	public MatchInforController(){
		NWonGameNum = 0;
		NPlayGameNum = 0;
		lGetInGameChip = 0;
		lNowChip = 0;
		lWinInGameChip = 0;
		PMAchive.ResetAchivement();		
	}
 	
 	 
	
	void ReSetGameRolesState()
	{
		Debug.Log("ReSetGameRolesState");
 		//hasReSetGameState=true;
		
		DestroyAllPublicCards();
    }
	
	public void NormalWinnerCards(int index)
	{
 		//Util.BeTranSparentGameObject(getCardObject(index,true),true);
 		//Util.BeTranSparentGameObject(getCardObject(index,false),true);
 		for(int i=2;i<7;i++)
		{
			Transform card=transform.FindChild("PublicCard"+(i-1));
			if(card!=null)
			{
 			 	Util.BeTranSparentGameObject(card.gameObject,true);
			}
		}
		preWinindex=-1;
	}
	
	GameObject getCardObject(int index,bool isleft)
	{
		
		Transform ob=null;
		if(index!=1)
		{
			Debug.LogWarning(index);
			if(isleft)
			{
				ob=transform.FindChild("RoleInfo_"+index).FindChild("lefthandcard");
			}
			else
			{
				ob=transform.FindChild("RoleInfo_"+index).FindChild("righthandcard");

			}
		}
		else
		{
			if(isleft)
			{
			  ob=transform.FindChild("RoleInfo_"+index).FindChild("cardAnim/f/BigCardFace1");
			} 
			else
			{
			   ob=transform.FindChild("RoleInfo_"+index).FindChild("cardAnim/f/BigCardFace2");
			} 
		}
		if(ob)
			return ob.gameObject;
		else
			return null;
	  	  
	}
	
	void BuyItemResponseEvent(bool success, ItemType itemType)
	{
		User.Singleton.MessageOperating = true;
		GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new BuyItemResponseDialog(success));
		PopUpTips();
		
		if (itemType == ItemType.Chip)
		{
			AudioSource audioSource = gameObject.GetComponent<AudioSource>();
			if (audioSource == null)
			{
				audioSource = gameObject.AddComponent<AudioSource>();
			}
			SoundHelper.PlaySound("Music/Other/DailySendChip",audioSource);
		}
	}
	
	
	
    public void ShowWinnerCards(bool[] cardshows,int index)
	{
		if(preWinindex!=-1)
			NormalWinnerCards(preWinindex);
		
		preWinindex=index;
		 
		if(cardshows.Length==7)
		{
//		   if(cardshows[0]==false)
//			{
//  			 	Util.BeTranSparentGameObject(getCardObject(index,true),false);
//  			}
//			if(cardshows[1]==false)
//			{
// 				Util.BeTranSparentGameObject(getCardObject(index,false),false);
//  			}
 			for(int i=2;i<7;i++)
			{
				if(cardshows[i]==false)
				{
					Transform card= transform.FindChild("PublicCard"+(i-1));
					if(card!=null)
					{
 						Util.BeTranSparentGameObject(card.gameObject,false);
					}
 				}
			}
 		}
		 
	}
	
	void DidFriendListFinished(bool result)
	{
		RemoveLoadingView();
		DisAbleAllButtons(true);
		PhotonClient.FriendListEvent -= DidFriendListFinished;
		if(Room.Singleton.RoomData.Owner.UserId==User.Singleton.UserData.UserId || (Room.Singleton.PokerGame.TableInfo != null && ! Room.Singleton.PokerGame.TableInfo.OnlyFriend))
		{
			
			if(User.Singleton.Friends!=null)
			{
				Transform FriendList2=topPanel.transform.FindChild("FriendList2");
				if(FriendList2==null)
				{
	 				GameObject prefab=Resources.Load("prefab/cardFriend/FriendList2") as GameObject;
					GameObject item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
					
			 		item.transform.parent=topPanel.transform;
					item.transform.name="FriendList2";
					item.transform.localPosition=new Vector3(0,0,-102);
					item.transform.localScale =new Vector3(1,1,1);
				}
			}
			else
			{  
				GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new NoFriendDialog());
				PopUpTips();
			}
		}
	}

 
	void RemoveLoadingView()
	{
		Transform Gamblingblackmask=topPanel.transform.FindChild("Gamblingblackmask");
		if(Gamblingblackmask!=null)
		{
			Destroy(Gamblingblackmask.gameObject);
		}
	}
	
	void ShowLoadingView()
	{
		GameObject prefab=Resources.Load("prefab/Gambling/Gamblingblackmask") as GameObject;
		GameObject item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		item.transform.parent=topPanel.transform;
		item.transform.localScale =new Vector3(1,1,1);
		item.name="Gamblingblackmask";
	}
	 
 
	void InviteFriends()
	{
 
		if(User.Singleton.UserData.NoSeat !=-1 )/**&& User.Singleton.UserData.UserType != UserType.Guest)**/
		{
 
		//remove UserType.Guest Invite Friends Limit
//		if(User.Singleton.UserData.UserType!=UserType.Guest)
//		{
 
			if(Room.Singleton.RoomData.Owner.UserId==User.Singleton.UserData.UserId || (Room.Singleton.PokerGame.TableInfo != null && ! Room.Singleton.PokerGame.TableInfo.OnlyFriend))
			{
				DisAbleAllButtons(false);
				ShowLoadingView();
				PhotonClient.FriendListEvent += DidFriendListFinished;
				PhotonClient.Singleton.FriendList();
 			}
 		}
  	}
 
	
 	IEnumerator NetworkUpdate()
	{
		PhotonClient.Singleton.Update();
		yield return null;
	}
	
	// Update is called once per frame
	void Update () {
 		StartCoroutine(NetworkUpdate());
  	 	SetProgress();
		 
		if(GlobalManager.Singleton.Messages.Count>0 && !User.Singleton.MessageOperating)
		{
			Debug.Log("Message  UIII");
			User.Singleton.CurrentMessage = GlobalManager.Singleton.Messages.Peek();
			
			if (User.Singleton.CurrentMessage.GetType() ==  typeof(RequestFriendMessage))
			{
				User.Singleton.MessageOperating = true;
				GlobalManager.Singleton.Messages.Dequeue();
				PopupMessage();
			}
			else if (User.Singleton.CurrentMessage.GetType() ==  typeof(AddFriendMessage))
			{
				User.Singleton.MessageOperating = true;
				GlobalManager.Singleton.Messages.Dequeue();
				Debug.Log("AddFriendMessage " + ((PlayerMessage)User.Singleton.CurrentMessage).NickName);
				GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new FriendAddedDialog(((PlayerMessage)User.Singleton.CurrentMessage).NickName));
				PopUpAddFirends();
			}
			else if (User.Singleton.CurrentMessage.GetType() ==  typeof(SendChipMessage))
			{
				User.Singleton.MessageOperating = true;
				GlobalManager.Singleton.Messages.Dequeue();
		    	//Debug.LogError("SendChipMessage "+((PlayerMessage)User.Singleton.CurrentMessage).Title);
				GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new SendChipDialog(((PlayerMessage)User.Singleton.CurrentMessage).NickName, ((PlayerMessage)User.Singleton.CurrentMessage).Title));
				PopupChipGift();
			}
			else if (User.Singleton.CurrentMessage.GetType() == typeof(AchievementMessage))
            {
                // User.Singleton.MessageOperating = true;
                // Debug.LogWarning("Achievement Messageeeeeeeeeeee");
				GlobalManager.Singleton.Messages.Dequeue();
                PopUpAchivement();
            }
			else{
				
				Debug.Log("Other message");
			}
		}
		
		if(GlobalManager.Singleton.Notifications.Count>0)
		{
			Notification notification=GlobalManager.Singleton.Notifications.Peek();
			if((notification.Target&TargetType.Game)==TargetType.Game)
			{
				if(notification is CheckEmailNotification)
				{
					CheckEmailNotification checkEmailNotification=GlobalManager.Singleton.Notifications.Dequeue() as CheckEmailNotification;
					if(checkEmailNotification.IsSuccess)
					{
						gameObject.name="Panel";
						WebBindingHelper.CloseWebView();
						if(User.Singleton.UserData.Avator<=0)
							PopUpAvatorDialog();
						else
						{
							WebBindingHelper.CloseWebView();
							PhotonClient.RegisterOrUpgradeEvent += UtilityHelper.OnUpgradeFinish;
							User.Singleton.GuestUpgrade(User.Singleton.UserData.NickName, GlobalManager.Singleton.ParamEmail, GlobalManager.Singleton.ParamPassword.getMD5(), DeviceTokenHelper.myDeviceToken, User.Singleton.UserData.Avator);
						}
					}
					else
					{
						WebBindingHelper.ExcuteJS("hideloading();");
						//UtilityHelper.ShowAlertDialog(LocalizeHelper.Translate("DIALOG_TITLE_ERROR"), PhotonClient.Singleton.ErrorMessage, LocalizeHelper.Translate("DIALOG_BUTTON_OK"));
//						#if UNITY_IPHONE
//							EtceteraBinding.showAlertWithTitleMessageAndButton(LocalizeHelper.Translate("DIALOG_TITLE_ERROR"), PhotonClient.Singleton.ErrorMessage, LocalizeHelper.Translate("DIALOG_BUTTON_OK"));
//						#endif
//						#if UNITY_ANDROID
//							EtceteraAndroid.showAlert(LocalizeHelper.Translate("DIALOG_TITLE_ERROR"), PhotonClient.Singleton.ErrorMessage, LocalizeHelper.Translate("DIALOG_BUTTON_OK"));
//						#endif
					}
				}
			}
		}
		
		if(GlobalManager.Singleton.Notifications.Count>0)
		{
			Notification notification=GlobalManager.Singleton.Notifications.Peek();
			if((notification.Target&TargetType.Match)==TargetType.Match)
			{
				if(notification is MatchBigBlindChangedNotification)
				{
					MatchBigBlindChangedNotification matchNotification=notification as MatchBigBlindChangedNotification;
					DoMatchBigBlindAmntChange(matchNotification.Amnt); 
 					GlobalManager.Singleton.Notifications.Dequeue();
				}
				else if(notification is MatchRankListChangedNotification)
				{
					MatchRankListChangedNotification matchNotification=notification as MatchRankListChangedNotification;
					//DoMatchBigBlindAmntChange(matchNotification.RankList); 
					//Debug.LogError(matchNotification.RankList);
//					for(int i=0;i<matchNotification.RankList.Length;i++)
//					{
//						Debug.Log(matchNotification.RankList[i]);
//					}
					if(User.Singleton.UserData.NoSeat!=-1)
						RankPlace=matchNotification.RankList[User.Singleton.UserData.NoSeat];
					
 					GlobalManager.Singleton.Notifications.Dequeue();
				}
				else
				{
					GlobalManager.Singleton.Notifications.Dequeue();

				}
			}
			else
			{
				GlobalManager.Singleton.Notifications.Dequeue();
			}
		}
	 
	}
	
	void PopUpAvatorDialog()
	{
		WebBindingHelper.CloseWebView();
		GameObject prefab=Resources.Load("prefab/chooseAvatar2") as GameObject;
		GameObject item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		
		item.layer=topPanel.layer;
		for(int i=0;i<item.transform.GetChildCount();i++)
		{
			Transform child=item.transform.GetChild(i);
			child.gameObject.layer=topPanel.layer;
		}
		item.transform.parent=topPanel.transform;
		AvatorGridScript avatorGridScript = item.GetComponent<AvatorGridScript>();
		
		if (avatorGridScript != null)
		{
			avatorGridScript.mail = GlobalManager.Singleton.ParamEmail;
			avatorGridScript.password = GlobalManager.Singleton.ParamPassword;
		}
			
		item.transform.localPosition=new Vector3(0,0,-100);
		item.transform.localScale =new Vector3(1,1,1); 
	}
	
	void PopupChipGift()
	{
		//DisAbleAllButtons(false); 
 		SoundHelper.PlaySound("Music/Other/DailySendChip",AudioSource,0);

		GameObject prefab=Resources.Load("prefab/Gambling/DailyGiftTable 1") as GameObject;
		GameObject item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		
		item.transform.parent=topPanel.transform;
		item.transform.localPosition=new Vector3(0,10,-20);
		item.transform.localScale =new Vector3(1,1,1);
	}
	void PopupMessage()
	{
		//DisAbleAllButtons(false); 
		GameObject prefab=Resources.Load("prefab/Gambling/PromptMessageTable") as GameObject;
		GameObject item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		
		item.transform.parent=topPanel.transform;
		item.transform.localPosition=new Vector3(0,0,-20);
		item.transform.localScale =new Vector3(1,1,1);
		
	}
	GameObject PopUpAddFirends()
	{
		//DisAbleAllButtons(false); 
		GameObject prefab=Resources.Load("prefab/Gambling/tips2") as GameObject;
		GameObject item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		
		item.transform.parent=topPanel.transform;
		item.transform.localPosition=new Vector3(0,200,-20);
		item.transform.localScale =new Vector3(1,1,1);
		
		return item;
	}
 
 
	
	void ShowTheViewWaitingForNext()
	{
			GameObject prefab=Resources.Load("prefab/WaitingForNext") as GameObject;
			GameObject item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
			
			item.transform.parent=topPanel.transform;
			item.name="ShowTheViewWaitingForNext";
			item.transform.localPosition=new Vector3(0,52,0);
			item.transform.localScale =new Vector3(1,1,1);
	}
   
	public void PopUpTips()
	{
		
		Transform tips2=topPanel.transform.FindChild("tips2");
		if(tips2==null)
		{
 			GameObject prefab=Resources.Load("prefab/Gambling/tips2") as GameObject;
			GameObject item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
			
			item.transform.parent=topPanel.transform;
			item.name="tips2";
			item.transform.localPosition=new Vector3(0,200,-16);
			item.transform.localScale =new Vector3(1,1,1);
		}
		
	}
	void PopUpTips2()
	{
 		GameObject prefab=Resources.Load("prefab/Gambling/tips2") as GameObject;
		GameObject item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		
		item.transform.parent=topPanel.transform;
		item.name="tips2";
		item.transform.localPosition=new Vector3(0,200,-6);
		item.transform.localScale =new Vector3(1,1,1);
 		
	}
	GameObject PopUpTips3()
	{
 		GameObject prefab=Resources.Load("prefab/WaitingForNext") as GameObject;
		GameObject item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		if(item.transform.FindChild("Button_close")!=null)
			Destroy(item.transform.FindChild("Button_close").gameObject);
		
		item.transform.parent=topPanel.transform;
		item.name="tips2";
		item.transform.localPosition=new Vector3(0,52,-6);
		item.transform.localScale =new Vector3(1,1,1);
 		return item;
	}
	void PopKickFodTwice()
	{
		GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new KickFoldTeiceDialog());
		GameObject prefab=Resources.Load("prefab/Gambling/tips2") as GameObject;
		GameObject item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		
		item.transform.parent=topPanel.transform;
		item.name="tips2";
		item.transform.localPosition=new Vector3(0,200,-11);
		item.transform.localScale =new Vector3(1,1,1);
	} 
	
	IEnumerator daleyerdestoryItem(float tim,GameObject item)
	{
		yield return new WaitForSeconds(tim);
		
		Destroy(item);
	}
	
	IEnumerator destoryItem(float tim,float fadeouttime,GameObject item)
	{
		yield return new WaitForSeconds(tim);
		
		Util.FadeWidtsOutWithTime(item,fadeouttime);
		StartCoroutine(daleyerdestoryItem(fadeouttime,item));
	}
	
	void DoMatchBigBlindAmntChange(int bigblindamnt)
	{
		GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new BigBlindAmntChangeDialog(bigblindamnt));
		GameObject tip= PopUpTips3();	 
		//tip.GetComponent<BoxCollider>().enabled=false;
		StartCoroutine(destoryItem(3,0.25f,tip));

	}
	
	void PopMatchStartTips()
	{
		GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new MatchStartTipDialog(string.Format("{0}$+{1}$",Champion.barChaBarData.fJoinSpend,Champion.barChaBarData.fServiceSpend)));
		GameObject tip= PopUpTips3();	 
	//	tip.GetComponent<BoxCollider>().enabled=false;
		StartCoroutine(destoryItem(3,0.25f,tip));		 
	}
	void PopUpDate()
	{
		if(achiveDialogAlreadyPopUp){
			shouldPopUpUpgradeDialog = true;
			return;
		}
		
		upgradeDialogAlreadyPopUp = true;
 		GameObject prefab=Resources.Load("prefab/Upgrade") as GameObject;
		GameObject item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		
		LevelUpDialog script = item.GetComponent<LevelUpDialog>();
		script.popDialogBehavior += ChoosePopUpAchivementDialog;

		item.transform.parent=topPanel.transform;
		item.transform.localPosition=new Vector3(-307.3499f,0,-1);
		item.transform.localScale =new Vector3(1,1,1);
	}
	 
	
	public void PopUpAchivement() {		
		if(upgradeDialogAlreadyPopUp) 
		{
			shouldPopUpAchiveDialog = true;
			return;
		}		
		
		achiveDialogAlreadyPopUp = true;
        GameObject prefab = Resources.Load("prefab/Achivementbar") as GameObject;
        GameObject item = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;

        PMAchive chaScript = item.GetComponent<PMAchive>();
        chaScript.item = item;
		chaScript.popDialogBehavior += ChoosePopUpUpgradeDialog;
		chaScript.localPosition = new Vector3(0, -10, -200);
		
        item.transform.parent = topPanel.transform;	
    }
	
	public void ChoosePopUpAchivementDialog(){
		// this will called by upgrade
		upgradeDialogAlreadyPopUp = false;
		if(shouldPopUpAchiveDialog){			
			shouldPopUpAchiveDialog = false;	
			
			PopUpAchivement();
		}		
	}
	
	public void ChoosePopUpUpgradeDialog(){		
		// this will called by achive
		achiveDialogAlreadyPopUp = false;
		if(shouldPopUpUpgradeDialog){			
			shouldPopUpUpgradeDialog = false;			
			
			PopUpDate();
		}
	}
      
	  
 	
	void DoPckerPlayerLeave(ActorInfor ac,bool isKick,PlayerLeaveType leavetype)
	{
 		if(isKick)
		{
			if(leavetype == PlayerLeaveType.KickFoldTwice &&  ac.NoSeat ==User.Singleton.UserData.NoSeat)
			{
			    PopKickFodTwice();
			}
			else if(leavetype == PlayerLeaveType.KickByPlayer &&  ac.NoSeat ==User.Singleton.UserData.NoSeat)
			{
			    DidLeaveGameFinished();
			}
		    else 
			{
				if(ac.NoSeat ==User.Singleton.UserData.NoSeat)
				{
					DidLeaveGameFinished();
				}
			}
		}
  		
 	}
	
	void DisCloseTipView()
	{
 		DisAbleAllButtons(true);
	}
	
	public void DisAbleAllButtons(bool iswork)
	{
		if(BlackMask!=null)
		{
 	    	BlackMask.SetActiveRecursively(!iswork);
			//transform.FindChild("AllBtns").GetComponent<BtnsAction>().DisAllBtns(!iswork);
			Transform allbtns=transform.FindChild("AllBtns");
			if(allbtns!=null)
			{
				allbtns.GetComponent<BtnsAction>().DisAllBtns(!iswork);
			}
		}
 	}
	
	void DoPockerGameStart()
	{
		HasMatchStarted=true;
 		if (Room.Singleton.PokerGame.TableInfo == null)
		{
			StartAdditionalBehaviour();
			return;
		}
	    numOfWonPots=1;
		ReSetGameRolesState();
      }
 
 
    
	void DoPockerPlayerHoleCardsChangedEvent(List<ActorInfor> players)
	{
	//	Debug.LogError("DoPockerPlayerHoleCardsChangedEvent");
		if(players.Count>0)
		{
 	  		float totalTime=0;
	 		for(int i=0 ;i<players.Count*2;i++)
			{
				ActorInfor ac=players[i%(players.Count)];
				Debug.Log(ac.name);
				if(ac!=null)
				{
		 			GameObject handcard=Instantiate(BigHandCard) as GameObject;
					handcard.transform.parent=transform.FindChild(ac.RoleName);
					handcard.transform.localScale =   new Vector3(1,1,1);
		 			if(i<players.Count)
					{
						handcard.name="lefthandcard";
						handcard.transform.localPosition = new Vector3(0,130,0);
					}
					else
					{
						handcard.name="righthandcard";
						handcard.transform.localPosition = new Vector3(0,130,-1);
				
					}
					HandCard handcardsprite=handcard.GetComponent<HandCard>();
					handcardsprite.index=ac.gamblingNo;
					handcardsprite.isleft=(i<players.Count);
					handcardsprite.Noseat=ac.NoSeat;
				    handcardsprite.waitForMoveTime=totalTime;
					if(User.Singleton.UserData.NoSeat==ac.NoSeat)
					{
						handcardsprite.SmallCard=false;
						if(i<players.Count)
						{
							handcardsprite.waitForRotateTime=0.15*(players.Count>1?(players.Count-1):1);
						}
					}
					else
					{
						handcardsprite.SmallCard=true;
					} 
					
		  			totalTime+=0.15f; 
				}
			}
		}
	}
 
	 
   
	void DidRegisterOrUpgradeEvent(bool iswork)
	{
		PhotonClient.RegisterOrUpgradeEvent-=DidRegisterOrUpgradeEvent;
 	}
 	
	void DoActorControllerGameSitDown(TypeState gamestate)
	{
 		RemovePromptChipsBtn();
 		TableInfo info=Room.Singleton.PokerGame.TableInfo;
 	 
		if(gamestate == TypeState.Playing  )
		{
			PlayerInfo iteminfo=info.GetPlayer(User.Singleton.UserData.NoSeat);
			if(iteminfo!=null)
			{
				if(!(iteminfo.IsPlaying==true || iteminfo.IsAllIn==true))
				{ 
					ShowTheViewWaitingForNext();
				}
			}
		}
	 
		Util.BeGrayGameObject(Button_Speak,true);

 	}
	void RegisterEventAction()
	{
		
		TableState.DopockerBetTurnBegan+=DopockerBetTurnBegan;
		TableState.DoPockerGameStandUp+=DoPockerGameStandUp;
		TableState.DoActorControllerGameSitDown+=DoActorControllerGameSitDown;
		TableState.DopockerBetTurnEnd+=DopockerBetTurnEnd;
 		TableState.DoPckerPlayerLeave+=DoPckerPlayerLeave;
		TableState.DoPockerGameStart+=DoPockerGameStart;
		TableState.DoPockerGameEnd+=DoPockerGameEnd;
		TableState.DoPockerPlayerHoleCardsChangedEvent+=DoPockerPlayerHoleCardsChangedEvent;
		TableState.DoPockerSendGiftEvent+=DoPockerSendGiftEvent;
 		
		PhotonClient.BroadcastMessageInTableEvent+=DidBroadcastMessageInTableEvent;
 		
  		PhotonClient.PlayerWonPotImproveEvent +=DidPlayerWonPotImproveEvent;
		PhotonClient.LevelUpEvent+=DidLevelUpEvent;
  		PhotonClient.RegisterOrUpgradeEvent+=DidRegisterOrUpgradeEvent;
		PhotonClient.SameAccountLoginEvent+=ReceiveSameAccountLoginEvent;
  		PhotonClient.SyncGameDataTableInfoEvent+=DidSyncGameDataTableInfoEvent;
		
		UtilityHelper.MaskTableAdditionalBehaviourEvent += StartAdditionalBehaviour;
		PhotonClient.LeaveGameFinished+=DidLeaveGameFinished;
		PhotonClient.GotoBackgroundSceneEvent += GotoRoom;
		PhotonClient.MaskingTablePopUPEvent +=PopupMaskingTable;
		PhotonClient.BuyItemResponseEvent += BuyItemResponseEvent;

		PhotonClient.DoNotSuccessKickPlayerEven+=DoNotSuccessKickPlayerEven;
		
		//PhotonClient.MatchBigBlindAmntChange+=DoMatchBigBlindAmntChange;
 	}
	
	void PopupMaskingTable()
	{
  	 	gameObject.AddComponent<MaskingTable>();		
	}
	
	void StartAdditionalBehaviour()
	{
		PhotonClient.Singleton.SyncGameDataTableInfo();
	}
	
	public void ReceiveSameAccountLoginEvent()
	{
		User.Singleton.MessageOperating = true;
		GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new SameAccountLoginDialog());
		PopUpTips();
	}
 	 
	void showBackView()
	{
		if(topPanel.transform.FindChild("GamblingInterface_BackList_4(Clone)")!=null)
		{
			DestroyImmediate(topPanel.transform.FindChild("GamblingInterface_BackList_4(Clone)").gameObject);
		}
		
		GameObject prefab=Resources.Load("prefab/Gambling/GamblingInterface_BackList_4") as GameObject;
		GameObject item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
  		item.transform.parent=topPanel.transform;
		
		item.transform.localPosition=new Vector3(-12,30,-10);
		item.transform.localScale =new Vector3(1,1,1);
	}
	void DoPockerGameStandUp(int Noseat)
	{
		
		//Debug.LogError(User.Singleton.UserData.NoSeat);
     	//NeedToShowTheViewWaitingForNext=false;
 		Util.BeGrayGameObject(Button_Speak,false);
 		showBackView();
		
		
//		if(LoseWinAndStandup==true)
//		{
//			ShowBuyInchip(transform.FindChild("RoleInfo_1/Button_Role").gameObject);
//			LoseWinAndStandup=false;
//		}
		
  	}
	void DidLevelUpEvent(int level)
	{
		  GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new LevelUpInfo(level));
		  PopUpDate();
	}
	
	void DoNotSuccessKickPlayerEven(ErrorCode err)
	{
 		switch(err)
		{
			case ErrorCode.KickErrorLevel:		
//				Debug.Log("fuck his level > your");
		 		GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new KickErrorLevel());
 				break; 
			case ErrorCode.KickErrorLimit:	
//				Debug.Log("num > you vip level");
 				GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new KickErrorLimit());		 
         		break;
			case ErrorCode.KickErrorOwner:	
//				Debug.Log("can not kick the owner of room");
				 GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new KickErrorOwner());
				break;
			case ErrorCode.KickErrorPlaying:	
//					Debug.Log("can not kick the playing user");
 				GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new KickErrorPlaying());	
				break;
		}
 	    PopUpTips();
	}
	void OutRegisterEventAction()
	{
		
		TableState.Singleton.CheckInTableInfoState();
		TableState.DopockerBetTurnBegan-=DopockerBetTurnBegan;
		TableState.DoPockerGameStandUp-=DoPockerGameStandUp;
		TableState.DoActorControllerGameSitDown-=DoActorControllerGameSitDown;
	    TableState.DopockerBetTurnEnd-=DopockerBetTurnEnd;
 		TableState.DoPckerPlayerLeave-=DoPckerPlayerLeave;
		TableState.DoPockerGameStart-=DoPockerGameStart;
		TableState.DoPockerGameEnd-=DoPockerGameEnd;
		TableState.DoPockerPlayerHoleCardsChangedEvent-=DoPockerPlayerHoleCardsChangedEvent;
		TableState.DoPockerSendGiftEvent-=DoPockerSendGiftEvent;

  		PhotonClient.BroadcastMessageInTableEvent-=DidBroadcastMessageInTableEvent;
		PhotonClient.BuyItemResponseEvent -= BuyItemResponseEvent;
	
		PhotonClient.LevelUpEvent-=DidLevelUpEvent;
		PhotonClient.PlayerWonPotImproveEvent -=DidPlayerWonPotImproveEvent;

 		PhotonClient.GotoBackgroundSceneEvent -= GotoRoom;
		PhotonClient.RegisterOrUpgradeEvent-=DidRegisterOrUpgradeEvent;
		PhotonClient.SameAccountLoginEvent-=ReceiveSameAccountLoginEvent;
 		PhotonClient.JoinGameEventFinished-=DidJoinGameEventFinished;
 		PhotonClient.SyncGameDataTableInfoEvent-=DidSyncGameDataTableInfoEvent;
		PhotonClient.LeaveGameFinished-=DidLeaveGameFinished;
		UtilityHelper.MaskTableAdditionalBehaviourEvent -= StartAdditionalBehaviour;
		
		PhotonClient.MaskingTablePopUPEvent -=PopupMaskingTable;
	    PhotonClient.DoNotSuccessKickPlayerEven-=DoNotSuccessKickPlayerEven;
			//	PhotonClient.MatchBigBlindAmntChange-=DoMatchBigBlindAmntChange;

	}
	
	void DidBroadcastMessageInTableEvent(int nosear,string message)
	{	
		if(nosear<0)
			return;
		
	 
		TableInfo info=Room.Singleton.PokerGame.TableInfo;
		if(info!=null)
		{
			PlayerInfo iteminfo=info.GetPlayer(nosear);
			if(iteminfo!=null)
			{
				SpeakTip.mes=SpeakTip.mes+"\n[38f8ff]"+iteminfo.Name+"[-]:"+message;
				//publishMessage();	
			}
		}
 		
	}
	
	public void DidLeaveGameFinished()
	{
//		if(Room.Singleton.RoomData.GameType==GameType.User)
//		{
//			if(User.Singleton.Friends!=null&&Room.Singleton.RoomData.Owner!=null&&User.Singleton.Friends.Exists(f => f.UserId == Room.Singleton.RoomData.Owner.UserId))
//			{
//				GotoRoom();
//			}
//			if(User.Singleton.Friends!=null&&Room.Singleton.RoomData.Owner!=null&&!User.Singleton.Friends.Exists(f => f.UserId == Room.Singleton.RoomData.Owner.UserId))
//				User.Singleton.JoinRoom(User.Singleton.UserData.UserId);
//		}
//		else
//		{
//			PhotonClient.GotoBackgroundSceneEvent -= GotoRoom;
//			PhotonClient.GotoBackgroundSceneEvent += GotoRoom;
//			User.Singleton.JoinRoom(User.Singleton.UserData.UserId);
//		}
		Debug.LogWarning("DidLeaveGameFinished");
		
 		User.Singleton.JoinRoom(User.Singleton.UserData.UserId);
	}
	
	 
	
	void ClearTableCards()
	{
		//Debug.Log("ClearTableCards");
		TableInfo infor=Room.Singleton.PokerGame.TableInfo;
		if(infor==null || infor.Players.Count<=1)
		{
 			DestroyAllPublicCards();
 		}
	 
	}
	void loseGameAndMoneyNotEn()
	{
		//PopUpPromptChipsBtn();
		  LoseWinAndStandup=true;
	 	  Transform roleinfo=transform.FindChild("RoleInfo_1");
		  if(roleinfo!=null)
		  {// Destroy(roleinfo.gameObject);
				StartCoroutine(destoryItem(0.1f,0.25f,roleinfo.gameObject));
		  }
		  Util.BeGrayGameObject(Button_Speak,false);
	 	  showBackView();
 
	}
	void DidPlayerWonPotImproveEvent(int potId,int[] winner,long[] winamnt,int[] attachedplayer)
	{
		numOfWonPots++;
	}
	void SetGameCheckInfor()
	{
		TableInfo info=Room.Singleton.PokerGame.TableInfo;
		if(info!=null && User.Singleton.UserData.NoSeat!=-1)
		{
			PlayerInfo playerInfo=info.GetPlayer(User.Singleton.UserData.NoSeat);
			if(playerInfo!=null)
			{
 				NPlayGameNum = playerInfo.PlayedInTable;
				if(NPlayGameNum>999)
					NPlayGameNum = 999;
				NWonGameNum = playerInfo.WinsInTable;
				if(NWonGameNum>999)
					NWonGameNum = 999;
				lGetInGameChip = playerInfo.MoneyInitAmnt;
				lNowChip = playerInfo.MoneySafeAmnt;
				lWinInGameChip = lNowChip - lGetInGameChip;
			}
		}
	}
	void DoPockerGameEnd(long taxAnimt,List<ActorInfor> players)
	{
		if(hasJoinGamed==true)
		{
	 		Debug.LogWarning("DidEndGameEvent");
	 		TableInfo info=Room.Singleton.PokerGame.TableInfo;
 			if(info!=null)
			{
 				PlayerInfo infoitem=info.GetPlayer(User.Singleton.UserData.NoSeat);
				if(infoitem!=null)
				{
//					if(Room.Singleton.PokerGame.PlayerActionNames[infoitem.NoSeat]==PhotonClient.ACTION_WIN) 
//				    {
//						//Remove 30000 Chips Limit for UserType.Guest
//// 	 	  	 			if ( User.Singleton.UserData.UserType == UserType.Guest
////							&& User.Singleton.UserData.Chips > 30000)
////						{
//// 								((Player)User.Singleton).CurrentInfos.Enqueue(new GuestChipLimitedDialog());
////								PopUpTips();
////							    
////						}
//				    }
//					else
//					{
						
						if(infoitem.MoneySafeAmnt == 0)
						{
 							Invoke("loseGameAndMoneyNotEn",2.25f*numOfWonPots);
						}
						else
						{
							bool flag=false;
							foreach(PlayerInfo aninfo in info.Players)
							{
								if(aninfo.NoSeat!=infoitem.NoSeat && aninfo.MoneySafeAmnt>0)
								{
								//	Debug.LogWarning("----------aninfo "+aninfo.MoneySafeAmnt +" "+aninfo.Name);
									flag=true;
									break;
								}
							}
							if(!flag)
							{
 							     Invoke("WinGameAndMoneyNotEn",2.25f*numOfWonPots);
							} 
						}
					//}
					
					SetGameCheckInfor();
				}
  			}
 			numOfWonPots=1;
 	 	//	Invoke("ClearTableCards",5f);
 			//hasReSetGameState=false;
  		}
 
	    
    }
	void WinGameAndMoneyNotEn()
	{
	   LoseWinAndStandup=true;

 		for(int i=2;i<=5;i++)
		{
			Transform roleinfo=transform.FindChild("RoleInfo_"+i);
			if(roleinfo!=null)
				Destroy(roleinfo.gameObject);
		}
		
		Util.BeGrayGameObject(Button_Speak,false);
 		showBackView();
	}
	void RemovePromptChipsBtn()
	{
		Transform PromptChipsBtn=topPanel.transform.FindChild("PromptChipsColne");
		if(PromptChipsBtn!=null)
		{
			Destroy(PromptChipsBtn.gameObject);
		}
	}
	
	void PopUpPromptChipsBtn()
	{
		 
		GameObject PromptChips=Resources.Load("prefab/Gambling/PromptChipsBtn") as GameObject;
		GameObject PromptChipsColne=Instantiate(PromptChips,new Vector3(0,0,0),Quaternion.identity) as GameObject; 
		PromptChipsColne.transform.parent=topPanel.transform;
		PromptChipsColne.transform.localPosition=new Vector3(345,221,-20);
	    PromptChipsColne.transform.localScale=new Vector3(1,1,1);
		PromptChipsColne.name="PromptChipsColne";
 	}
	void DestroyAllPublicCards()
	{
		Debug.Log("DestroyAllPublicCards");
		Transform[] trs=transform.GetComponentsInChildren<Transform>();
		foreach(Transform tr in trs)
		{
			if(tr!=null)
			{
				if(tr.name=="PublicCard1"|| tr.name=="PublicCard2" || tr.name=="PublicCard3" || tr.name=="PublicCard4" || tr.name=="PublicCard5")
				{
	 				DestroyImmediate(tr.gameObject);
				}
			}
		}
	}
	 
	void showPublicCard(int index,bool anim)
	{
	    float paddingTime=0;
 		for(int i=1;i<=index;i++)
		{
		    Transform cloneCard=transform.FindChild("PublicCard"+i);
 	      	if(cloneCard==null)
 		  	{
				GameObject PublicCardColne=Instantiate(PublicCard) as GameObject;
				PublicCardColne.name="PublicCard"+i;
				PublicCardColne.transform.parent=transform;
 		    	PublicCardColne.transform.localScale=new Vector3(1,1,1);
				//Debug.Log("paddingTime:"+paddingTime);
			 	PublicLcense lcense=PublicCardColne.GetComponent<PublicLcense>();
				lcense.index=i;
				lcense.delayTime=(anim?paddingTime:-1);
				paddingTime+=0.6f;
			 }
		 }
  		
	}
 
	void DopockerBetTurnBegan()
	{
		if(hasJoinGamed==true)
		{
	 		TableInfo info=Room.Singleton.PokerGame.TableInfo;
		//	Debug.LogError("DidBetTurnStartEvent" +info.Round);
			
//			if(hasReSetGameState==false)
//			{
//				ReSetGameRolesState();
//	 		}
	 		if(info.Round == TypeRound.Flop)
			{
	 			showPublicCard(3,true);
	 		}
			else if(info.Round==TypeRound.Turn)
			{
	 	 		showPublicCard(4,true);
	    	}
			else if(info.Round==TypeRound.River)
			{				
				showPublicCard(5,true);
			}  
		}
	    
 
  	} 
	 
	void DopockerBetTurnEnd()
	{
 
  		TableInfo info=Room.Singleton.PokerGame.TableInfo;
		if(info!=null)
		{
			foreach(PlayerInfo item in info.Players)
			{
				if(item.MoneyBetAmnt>0)
				{
					Invoke("sendchipsPlayAuto",0.5f);
					break;
				}
			}
		}
 	}
	
	void showCard()
	{
		TableInfo info=Room.Singleton.PokerGame.TableInfo;
		if(info.Cards!=null)
		{
//			Debug.LogWarning(info.Cards[0].ToString());
//			Debug.LogWarning(info.Cards[1].ToString());
//			Debug.LogWarning(info.Cards[2].ToString());
//			Debug.LogWarning(info.Cards[3].ToString());
//			Debug.LogWarning(info.Cards[4].ToString());

				if(info.Cards.Count()>=3)
				{
					if(info.Cards[2].ToString()!="--")
					{
						
						showPublicCard(3,false);
					}
				}
				if(info.Cards.Count()>=4)
				{
					if(info.Cards[3].ToString()!="--")
					{
						 showPublicCard(4,false);
					}
				}
				if(info.Cards.Count()>=5)
				{
					if(info.Cards[4].ToString()!="--")
					{
						 showPublicCard(5,false);
					}
				}
		 }
	}
 
	void DidJoinGameEventFinished(ErrorCode error,TypeState gamestate)
	{
 		Debug.LogWarning(":(((((((((((((((((((((((((((");
		UtilityHelper.CloseMaskingTable();
		if(error==ErrorCode.Ok)
		{
 	 		
			TableInfo  info = Room.Singleton.PokerGame.TableInfo;
	 		if(info!=null)
			 {
				 Debug.Log("Table infor");
				 Debug.Log("########### the player number is: " + info.Players.Count.ToString());
	  		    if(hasJoinGamed==false)
				{
					PopMatchStartTips();
				}
				
				hasJoinGamed=true;
  			 
				DestroyAllPublicCards();
 				Util.BeGrayGameObject(Button_Speak,true);
 			 	Invoke("showCard",0.2f);
 				
 	   		}
			else
				Debug.Log("Table infor null");
		}
		else
		{
			if(error==ErrorCode.TableFull)
			{
 				DisAbleAllButtons(false);
				GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new TableFullErrorDialog());
 				PopUpAddFirends();
 
			}
			else
			{
 				DisAbleAllButtons(false);
				GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new GameIdNotExistsDialog());
 				PopUpAddFirends();
 
			}
		}
   	}
	 
	void DidSyncGameDataTableInfoEvent(bool iswork)
	{
		if(iswork)
		{
			Debug.LogWarning("DidSyncGameDataTableInfoEvent");
			DisAbleAllButtons(true);
  		}
 	}
 
	
	void Start () {
		MusicManager.Singleton.PlayBgMusic();
		ShowSignal();
  		Debug.LogWarning("numOfCheckInTable :"+numOfCheckInTable++);
   	}
	
	void ShowSignal()
	{
		GameObject prefab=Resources.Load("prefab/wugen") as GameObject;
		GameObject item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
 		item.transform.parent=transform;
		item.transform.localPosition=new Vector3(-343,160,-1);
		item.transform.localScale =new Vector3(1,1,1); 
	}
 	void CreateHandCards(bool left,string cardvalue)
	{ 
		
	//    		GameObject prefab=Resources.Load("prefab/Gambling/BigHandCard") as GameObject;
	//  			GameObject handcard=Instantiate(prefab) as GameObject;
	//			handcard.transform.parent=transform;
	//	  		handcard.transform.localScale  =  new Vector3(1,1,1);
	// 			
	//			if(left)
	//			{
	//				handcard.name="lefthandcard";
	//				handcard.transform.localPosition = new Vector3(0,130,0);
	//			}
	//			else
	//			{
	//				handcard.name="righthandcard";
	//				handcard.transform.localPosition = new Vector3(0,130,-1);
	//			} 
	//			HandCard handcardsprite=handcard.GetComponent<HandCard>();
	//  		    handcardsprite.setDetailInfor(index,left,NoSeat,false,cardvalue);
	//		    
	//			if(NoSeat ==User.Singleton.UserData.NoSeat && index==1 && User.Singleton.UserData.NoSeat!=-1)
	//			{
	//				handcardsprite.SmallCard=false;
	//			}
	 
 	}
		
	void Awake()
	{   
		hasJoined=true;
		MatchInforController.Singleton=this;
 		StartCoroutine(MusicManager.Singleton.BgFadeIn(1f));
		SpeakTip.mes="";
		
		if(GiftGrid.GiftDictionary.Count==0)
				GiftGrid.LoadGift();
		
		if(GlobalManager.Singleton.version==KindOfVersion.Basic)
			blackMask.SetActiveRecursively(false);
		
		//OnInitState();
		RegisterEventAction();
		gameObject.AddComponent<MaskingTable>();
  		PhotonClient.JoinGameEventFinished+=DidJoinGameEventFinished;
 		//if()//JoinGamblingSettingInfor.Singleton.isFastStart==true)
		//{
		   TableState.Singleton.CheckInTableInfoState(TableState.GameTableType.GameTableType_Match);
		   Debug.Log(Champion.barChaBarData.nLevelIndex); 
		   PhotonClient.Singleton.JoinCareerGame(Champion.barChaBarData.nLevelIndex); 
			//Debug.Log("User.Singleton.QuickStart");
			//User.Singleton.QuickStart();
		    
		//}
//		else
//		{
// 			if(JoinGamblingSettingInfor.Singleton.haveDone==true)
//			{
//			     User.Singleton.JoinGame();
//			}
//			else
//			{
//				User.Singleton.JoinGame((int)JoinGamblingSettingInfor.Singleton.MaxBlindAmnit,9,JoinGamblingSettingInfor.Singleton.thinkingTime,JoinGamblingSettingInfor.Singleton.onlyFirend);
// 			}
//		}
 	//	Debug.Log("Start");
		
 		//JoinGamblingSettingInfor.Singleton.ReSetSettingInfor();
		AudioSource=gameObject.AddComponent<AudioSource>();
 	}
 
 
	void sendchipsPlayAuto()
	{
		SoundHelper.PlaySound("Music/Other/sendchips",AudioSource,0);
	}
 	
	public void DestroytheBody(GameObject ob)
	{
		Destroy(ob);
	}
 
	void showSpeackTips()
	{
		if(User.Singleton.UserData.NoSeat>=0)
		{
			SoundHelper.PlaySound("Music/Other/viewIn",AudioSource,0);
			DisAbleAllButtons(false); 
			UtilityHelper.PreConditionFinished(true);
  		}
	}
	 
	
	public void showBuyChips()
	{
		//UserType.Guest-Buy
		//Remove UserType.Guest Buy Chips Limit
//		if (User.Singleton.UserData.UserType == UserType.Guest)
//		{
//			((Player)User.Singleton).CurrentInfos.Enqueue(new GuestBuyChipsLimitedDialog());
// 			PopUpTips2();
//		}
 
//		else
//		{
 		if (User.Singleton.UserData.UserType == UserType.Guest&&GlobalManager.Singleton.ApplicationType == AppType.NinetyOne)
		{
			GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new NinetyOneGuestBuyTipDialog(gameObject.name,"OnRegister",false));
			PopUpTips2();
		}
		else
		{
 
 			RemovePromptChipsBtn();
			DisAbleAllButtons(false);
			SoundHelper.PlaySound("Music/Other/viewIn",AudioSource,0);
 			
			GameObject prefabobject=Resources.Load("prefab/Gambling/BuyChipGambling") as GameObject;
			GameObject SpeakGambling=Instantiate(prefabobject,new Vector3(0,0,0),Quaternion.identity) as GameObject;
			SpeakGambling.transform.parent=topPanel.transform;
			 
			SpeakGambling.transform.localPosition=new Vector3(0,0,-10);
			SpeakGambling.transform.localScale=new Vector3(1,1,1);
			
 			UIButtonMessage mes= SpeakGambling.transform.FindChild("Button_close").GetComponent<UIButtonMessage>();
			mes.functionName="BtnClose";
			mes.target=gameObject;
 		}
	}
	 
	
	void RoleSitDown(GameObject Btn)
	{
 
     	TableInfo info=Room.Singleton.PokerGame.TableInfo;

		BuyChip buchip=Btn.transform.parent.GetComponent<BuyChip>();
		if(buchip.currentValue>=info.BigBlindAmnt*20)
		{ 
  			for(int i=1;i<=9;i++)
			{
                if(Btn.transform.name=="RoleInfo_"+i)
				{
 				//	Debug.LogWarning((TableState.Singleton.CurrentNoSeat+(i-1))%9+" : "+buchip.currentValue);
					if(info.GetPlayer((TableState.Singleton.CurrentNoSeat+(i-1))%9)==null)
					{
						User.Singleton.Sit((byte)((TableState.Singleton.CurrentNoSeat+(i-1))%9),buchip.currentValue);
 	 					BtnClose(Btn); 
 						break;
					}
   				}
			}
		}
		else
		{
			GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new BuyIndDialog());
			PopUpTips();
 		}
 	}
	
	void ShowBuyInchip(GameObject btn)
	{
 		TableInfo info=Room.Singleton.PokerGame.TableInfo;
		if(info!=null)
		{
 	  		if(info.BigBlindAmnt*20>User.Singleton.UserData.Chips)
	  		{
				GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new BuyIndDialog());
				PopUpTips();
				return;
		 	}
			
	 		DisAbleAllButtons(false);
	 		GameObject prefab=Resources.Load("prefab/Gambling/buyinChip") as GameObject;
			GameObject	item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
	 		item.transform.parent=topPanel.transform;
			item.transform.name=btn.transform.parent.name;
		}
  
	}
 
	void RoleStandUp()
	{
		TableState.Singleton.Standup();
  	}
	void OnDestroy()
	{
 		CancelInvoke();
		if(GlobalScript.ScriptSingleton.CurrentInfos!=null)
			GlobalScript.ScriptSingleton.CurrentInfos.Clear();
		Debug.Log("OnDestroy");
		OutRegisterEventAction();
 		User.Singleton.MessageOperating = false;
		MatchInforController.Singleton=null;
	}
	public void leaveGame()
	{
	    //OutRegisterEventAction();
		
		//if(User.Singleton.UserData.NoSeat==-1)
		//{
				User.Singleton.LeaveGame();
			
// 		}
// 		else
// 		{
// 			DidLeaveGameFinished();
// 		}
 	}
	
	private IEnumerator internalLoadLevelAsync(AsyncOperation async)
	{
		while(!async.isDone){
        	LoadingPercentHelper.Progress = async.progress;
			Debug.Log("aaaaaaaaa " + LoadingPercentHelper.Progress);
			yield return async.progress;
		}

		LoadingPercentHelper.Progress = 0;
    }
	
	public void GotoRoom()
	{
		//Debug.LogWarning("---GotoRoom"); 
		PhotonClient.GotoBackgroundSceneEvent -= GotoRoom;
		if (GlobalManager.Singleton.version == KindOfVersion.Ultimate)
			async = Application.LoadLevelAsync("BackGround");
		else if (GlobalManager.Singleton.version == KindOfVersion.Basic)
		{
			Debug.Log("00000000000000000000000000000");
			async = Application.LoadLevelAsync("BackGround_simple");
		}
			
		OutRegisterEventAction();
	}
	
	void SetProgress()
	{
		if (async != null)
		{
			Debug.Log("Progress is: " + async.progress);
			LoadingPercentHelper.Progress = async.progress;
		}
	}
	
	public void BtnBackToRoom()
	{
      //  Debug.LogWarning ("!!!!!!! BtnBackToRoom");
		ShowLoadingTable table =topPanel.transform.parent.gameObject.AddComponent<ShowLoadingTable>();
		table.SetCallback(gameObject, "leaveGame");
		leaveGame();
 
	}
	 
	void BtnBackAction()
	{
 		DisAbleAllButtons(false);
		if(User.Singleton.UserData.UserType==UserType.Guest)
			UtilityHelper.ChooseFirstOneFinished(false);
		else
			UtilityHelper.ChooseFirstOneFinished(true);
  	}
	public void QuickRegister()
	{
		PlatformHelper.Upgrade(gameObject.name,"OnRegister");
//		switch(GlobalManager.Singleton.ApplicationType)
//		{
//		case AppType.Normal:
//			gameObject.name="fuckPanel";
// 			WebBindingHelper.ShowRegisterWebView(gameObject.name,"OnRegister");
//			break;
//		case AppType.NinetyOne:
//			NdComHelper.GuestRegist(gameObject.name,"OnRegister");
//			break;
//		}
	}
	
	void OnRegister(string values)
	{
		Debug.Log(values);
		
//		string[] param;
//		
//		switch(GlobalManager.Singleton.ApplicationType)
//		{
//		case AppType.Normal:
//			param=values.Split(',');
//			GlobalManager.Singleton.ParamEmail=param[0];
//			GlobalManager.Singleton.ParamPassword=param[1];
//			User.Singleton.CheckEmail(GlobalManager.Singleton.ParamEmail);
//			break;
//		case AppType.NinetyOne:
//			User.Singleton.GuestUpgrade(User.Singleton.UserData.NickName, 
//				values, 
//				string.Empty, 
//				DeviceTokenHelper.myDeviceToken, 
//				User.Singleton.UserData.Avator);
//			GlobalManager.Singleton.ParamEmail=string.Empty;
//			GlobalManager.Singleton.ParamPassword=string.Empty;
//			break;
//		}
		
		PlatformHelper.OnUpgrade(values);
	}
	
 	public void ShowGiftView(GameObject btn)
	{
 
		if(User.Singleton.UserData.NoSeat==-1)
			return;
		
 		UtilityHelper.PreConditionFinished(true);
  	}
	
	void GiftMoveCompleted(GameObject gift)
	{
		Debug.Log(gift.name);
		Transform giftbg=gift.transform.FindChild("Background");
		UISprite giftbgsprite=giftbg.GetComponent<UISprite>();
		
		Transform role=transform.FindChild(gift.name);
		Transform background=role.FindChild("Button_gift/Background");
		UISprite backgroundsprite=background.GetComponent<UISprite>();
 		backgroundsprite.spriteName=giftbgsprite.spriteName;
		background.transform.localScale=new Vector3(50,44,1);
		
		Destroy(gift);
 	}
	void DoPockerSendGiftEvent(ActorInfor sendactor,ActorInfor receiveactor,int ID,List<ActorInfor> players)
	{
 		
		TableInfo info=Room.Singleton.PokerGame.TableInfo;
		if(info!=null)
		{
			PlayerInfo iteminfo=info.GetPlayer(sendactor.NoSeat);
			if(iteminfo!=null)
			{
				Dictionary<int,Gift>dic=GiftGrid.GiftDictionary;
		  		Gift gift=dic[ID];
		 		if(receiveactor==null)
				{
					BuyGiftsToAll(gift.icon_name,sendactor,players);
				}
				else
				{
					BuyGiftToOneFinished(receiveactor,gift.icon_name,sendactor);
				}
				
 				Transform Role=transform.FindChild(sendactor.RoleName);
 				Util.SetLabelValue(Role,"Label_money",Util.getLableMoneyK_MModel(iteminfo.MoneySafeAmnt));
				
			}
		}

	}
	
	void BuyGiftToOneFinished(ActorInfor receiveactor,string giftname,ActorInfor sendactor)
	{
 
		
 		Transform giftbtn=transform.FindChild(receiveactor.RoleName+"/Button_gift");
		giftbtn.GetComponent<GiftTime>().setCaculateGiftTime(giftname);
		Transform giftbtnbg= giftbtn.FindChild("Background");
		UISprite  giftsprite=giftbtnbg.GetComponent<UISprite>();
		
		if(receiveactor.RoleName == "RoleInfo_1") 	
		{
		    giftsprite.spriteName=giftname;
 		    giftbtnbg.transform.localScale=new Vector3(giftsprite.sprite.outer.width/2,giftsprite.sprite.outer.height/2,1);
		}
 		else 	
		{
			
			
		   Transform myButton_gift=transform.FindChild(sendactor.RoleName+"/Button_gift");
		   GameObject giftclone=Instantiate(myButton_gift.gameObject,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		   giftclone.transform.parent=myButton_gift.parent;
  			
			giftclone.transform.localPosition=myButton_gift.transform.localPosition;
			giftclone.transform.localScale=myButton_gift.transform.localScale;
			giftclone.name=receiveactor.RoleName;
 			Transform giftclonebg=giftclone.transform.FindChild("Background");
 			UISprite  giftclonesprite=giftclonebg.GetComponent<UISprite>();
			
 			giftclonesprite.spriteName=giftname;
		 	giftclonebg.transform.localScale=new Vector3(50,44,1);

 			iTween.MoveTo(giftclone,iTween.Hash("position",new Vector3( giftbtn.transform.localPosition.x, giftbtn.transform.localPosition.y,0),"time",0.6f,"looptype","none","easetype","easeInOutQuad","islocal",true,"oncomplete","GiftMoveCompleted","oncompletetarget",gameObject,"oncompleteparams",giftclone));
		}
		
	}
	
	
	
	void BuyGiftsToAll(string iconName,ActorInfor sendnoseatActr,List<ActorInfor> players)
	{
 	   foreach(ActorInfor actor in players)
		{
  			
			Transform role=transform.FindChild(actor.RoleName);
			Transform roleBtn=role.FindChild("Button_gift");
			
			
			
			Transform myButton_gift=transform.FindChild(sendnoseatActr.RoleName+"/Button_gift");
			
			GameObject giftclone=Instantiate(myButton_gift.gameObject,new Vector3(0,0,0),Quaternion.identity) as GameObject;
			
			giftclone.transform.parent=myButton_gift.parent;
			giftclone.transform.localPosition=myButton_gift.transform.localPosition;
			giftclone.transform.localScale=myButton_gift.transform.localScale;
			giftclone.name=role.name;
 			Transform giftclonebg=giftclone.transform.FindChild("Background");
 			UISprite  giftclonesprite=giftclonebg.GetComponent<UISprite>();
			
 			giftclonesprite.spriteName=iconName;
		 	giftclonebg.transform.localScale=new Vector3(50,44,1);
			roleBtn.GetComponent<GiftTime>().setCaculateGiftTime(iconName);
  			iTween.MoveTo(giftclone,iTween.Hash("position",new Vector3( roleBtn.localPosition.x, roleBtn.localPosition.y,0),"time",0.6f,"looptype","none","easetype","easeInOutQuad","islocal",true,"oncomplete","GiftMoveCompleted","oncompletetarget",gameObject,"oncompleteparams",giftclone));

		}
	}
	
	 
   	 
	
	void showFirendInforPref(PlayerInfo playinfo,string RoleName)
	{
		
 		GameObject prefab=Resources.Load("prefab/Gambling/check_friendinfo") as GameObject;
		GameObject item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
 		 
		item.transform.parent=topPanel.transform;
		item.transform.localPosition=new Vector3(0,0,-100);
		item.transform.localScale =new Vector3(1,1,1);
		item.name=RoleName;
   	    item.GetComponent<FriendPlayerInfo>().OnInitDetailInformation(playinfo);
		
 	}
	void CardFriendNotHere()
	{
		GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new CardFriendNotHereDialog());
		PopUpTips2();		 
	}
	
	void addMakeFriend(GameObject btn)
	{
 		TableInfo infor=Room.Singleton.PokerGame.TableInfo;
		if(infor!=null)
		{
			int Noseat=btn.transform.parent.GetComponent<PockerPlayer>().NoSeat;
			string playerName=btn.transform.parent.GetComponent<PockerPlayer>().PlayerName;
			Debug.LogWarning("playerName "+playerName);
			
			if(Noseat!=-1)
			{
 		  		PlayerInfo playitem=infor.GetPlayer(Noseat);
				if(playitem!=null)
				{
					Debug.LogWarning(playitem.Name);
					if(playitem.Name==playerName)
					{
						DisAbleAllButtons(false);
		  				showFirendInforPref(playitem,btn.transform.parent.name);
					}
					else
					{
						CardFriendNotHere();
					}
		 		}
				else
				{
					CardFriendNotHere();
				}
			}
 		}
  	}
	
	 
	
    void BtnClose(GameObject btn)
	{
 		
		SoundHelper.PlaySound("Music/Other/viewClose",AudioSource,0);
		DisAbleAllButtons(true);
		//Debug.Log(btn.name);
		 Destroy(btn.transform.parent.gameObject);
 	 }
	
	
	
//	void OnApplicationQuit()
//	{
//		GlobalManager.Singleton.EndTiming();
//	}
	
	void ActivedToIdle()
	{
		User.Singleton.ActivedToIdle();
	}

	void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			Debug.Log("OnApplicationPause Suspend");
			UtilityHelper.CloseMaskingTable();
			User.Singleton.Suspend();
			
			StartCoroutine(NetworkUpdate());
		}
		else
		{
			Debug.Log("OnApplicationPause Actived");
			
			User.Singleton.ActivedToIdle();
  			
			//DisAbleAllButtons(false);
			MaskingTable table = gameObject.AddComponent<MaskingTable>();
			table.SetCallback(gameObject, "ActivedToIdle");
		 	table.bDisableAllbtns = false;
			
			StartCoroutine(NetworkUpdate());
		}
	}
  
}
