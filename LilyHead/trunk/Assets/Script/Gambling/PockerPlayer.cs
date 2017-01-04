using UnityEngine;
using System.Collections;
using DataPersist;
using AssemblyCSharp;
using EasyMotion2D;
using AssemblyCSharp.Helper;
using System.Collections.Generic;
using System;
using LilyHeart;

public class PockerPlayer : MonoBehaviour {
	
	public int index=-1;
	public int NoSeat=-1;
	public string PlayerName=string.Empty;
 
	public GameObject HanHuaTip;
	public GameObject SpriteD;
	public GameObject Vip;
	public GameObject Button_RoleBg;
	public GameObject Button;
	public GameObject GIFBlackBasePlateMask;
	
	public GameObject Gift_Button;
	public GameObject Gift_ButtonBg;
	
	public GameObject Name;
	public GameObject Label_Money;
	
	public GameObject makeEffectPrefab;
	public GameObject Title;
	
	public string GiftIcon="GIFGiftBtn";
	
	
	
	private Transform makeEffectPanel;
	private Transform topPanel;
	private AudioSource as2;
 
	//bool IndicatorBarHasShowed=false;
	
	Color white=new Color(1,1,1,1);
	Color gold=new Color(1,0.84f,0,1);
	
	public AudioSource AudioSource {get;set;}
 	
 	// Use this for initialization
	void Start () {
	
	 
		makeEffectPanel=GameObject.Find("MakeEffect/Camera/Anchor/Panel").transform;
		topPanel=GameObject.Find("topPanel/Camera/Anchor/Panel").transform;
 		
		RegisterEventAction();
		
		initEntiyRole();
		AudioSource=gameObject.AddComponent<AudioSource>();
		as2 = gameObject.AddComponent<AudioSource>();
	}
	void OnDestroy()
	{
		//Debug.LogWarning("OnDes  "+index);
		CancelInvoke();
		UnRegisterEventAction();
	}
 	// Update is called once per frame
	void Update () {
	
	}
	
	void setMainBtnBg(PlayerInfo iteminfor ,ActorInfor ac)
	{
  		UISprite sprite=Button_RoleBg.GetComponent<UISprite>();
   		if(iteminfor.Avator==(byte)PlayerAvator.DaHeng)
		{
		     sprite.spriteName="Oil tycoon";
		}
		else if(iteminfor.Avator==(byte)PlayerAvator.Dalaopo)
		{
 			sprite.spriteName="Wealthy first wife";
		}
		else if(iteminfor.Avator==(byte)PlayerAvator.European)
		{
			sprite.spriteName="Prince";
		}
		else if(iteminfor.Avator==(byte)PlayerAvator.Luoli)
		{
			sprite.spriteName="loli";
		}
		else if(iteminfor.Avator==(byte)PlayerAvator.Songer)
		{
			sprite.spriteName="BlackMan";
		}
		else if(iteminfor.Avator==(byte)PlayerAvator.Yitaitai)
		{
			sprite.spriteName="Song Dynasty Women";
		}
		else if(iteminfor.Avator==(byte)PlayerAvator.Qianjing)
		{
			sprite.spriteName="Missy";
 		}
		else if(iteminfor.Avator==(byte)PlayerAvator.Guest)
		{
 			sprite.spriteName="Guest";
		}
		else if(iteminfor.Avator==(byte)PlayerAvator.Captain)
		{
			sprite.spriteName="Pirate";
		}
		else if ((iteminfor.Avator == (byte)PlayerAvator.AGe) 
			|| (iteminfor.Avator == (byte)PlayerAvator.Princess)
			|| (iteminfor.Avator == (byte)PlayerAvator.General)
			|| (iteminfor.Avator == (byte)PlayerAvator.Queen))
		{
			sprite.spriteName = ((PlayerAvator)iteminfor.Avator).ToString();
		}
  		
		if(Util.isMatch())
		{
			if(ac.gamblingNo>=4 && ac.gamblingNo<=5)
			{
				 Button_RoleBg.transform.localScale=new Vector3(-sprite.sprite.outer.width,sprite.sprite.outer.height,1);
			} 
			else
			{
				Button_RoleBg.transform.localScale=new Vector3(sprite.sprite.outer.width,sprite.sprite.outer.height,1);
	
			}
		}
		else
		{
 	  		if(ac.gamblingNo>=6 && ac.gamblingNo<=9)
			{
				 Button_RoleBg.transform.localScale=new Vector3(-sprite.sprite.outer.width,sprite.sprite.outer.height,1);
			} 
			else
			{
				Button_RoleBg.transform.localScale=new Vector3(sprite.sprite.outer.width,sprite.sprite.outer.height,1);
	
			}
		}
 	 
		UIButtonMessage btnMess=Button.GetComponent<UIButtonMessage>();
  		btnMess.functionName="addMakeFriend";
   
		
	}
     
	
 	 
	void DidBetTurnEndeReSetRolesName()
	{
		TableInfo infor=Room.Singleton.PokerGame.TableInfo;
		if(infor!=null)
		{
		    PlayerInfo playinfo=infor.GetPlayer(NoSeat);
			if(playinfo!=null)
			{
	 			if(Room.Singleton.PokerGame.PlayerActionNames[playinfo.NoSeat]==PhotonClient.ACTION_FOLD)
				{
					SetRoleName(index,LocalizeHelper.Translate("YIGAIPAI")); 
				}
				else
				{
					SetRoleName(index,playinfo.Name); 
				} 
			}
  		}
		 
	}
 
	void SetPockerGameStartSetRoleInfor(PlayerInfo iteminfor,ActorInfor actorinfor)
	{
		setMainBtnBg(iteminfor,actorinfor);
		Util.SetLabelValue(Label_Money.transform,Util.getLableMoneyK_MModel(iteminfor.MoneySafeAmnt));
		GIFBlackBasePlateMask.SetActiveRecursively(true);
		
		string picstr =HonorHelper.GetHonorPicString(HonorHelper.GetHonorRecuise(iteminfor as UserData,HonorType.Citizen),iteminfor.Avator);
 		showTitleTip(actorinfor,picstr);
		
		PlayerName=iteminfor.Name;
 	 
		setGiftBg(true);

		
		TableInfo info=Room.Singleton.PokerGame.TableInfo;
		if(info!=null)
		{
			if(NoSeat == info.NoSeatDealer)
			{
				SpriteD.SetActiveRecursively(true);
			}
		}
		
		if((iteminfor as UserData).VIP>0)
			Vip.SetActiveRecursively(true); 
	}
 	void SetRoleDetailInfor(PlayerInfo iteminfor,ActorInfor actorinfor)
	{  	 
		
    	setMainBtnBg(iteminfor,actorinfor);
 		
		Transform btnGift=transform.FindChild("Button_gift");
		btnGift.gameObject.SetActiveRecursively(true);
 		
		Util.SetLabelValue(Label_Money.transform,Util.getLableMoneyK_MModel(iteminfor.MoneySafeAmnt));
 		 
		GIFBlackBasePlateMask.SetActiveRecursively(true);
  		 
		SetRoleName(actorinfor.gamblingNo,iteminfor.Name);
 		PlayerName=iteminfor.Name;
		
		string picstr =HonorHelper.GetHonorPicString(HonorHelper.GetHonorRecuise(iteminfor as UserData,HonorType.Citizen),iteminfor.Avator);
 		
		showTitleTip(actorinfor,picstr);
		
		if(iteminfor.MoneyBetAmnt>0)
		{
			setSpritechipLabel(Util.getLableMoneyK_MModel(iteminfor.MoneyBetAmnt),iteminfor.MoneyBetAmnt);
		}
		
		TableInfo info=Room.Singleton.PokerGame.TableInfo;
		if(info!=null)
		{
			if(NoSeat == info.NoSeatDealer)
			{
				SpriteD.SetActiveRecursively(true);
			}
		}
		
		if((iteminfor as UserData).VIP>0)
			Vip.SetActiveRecursively(true); 
		
		if(iteminfor.IsAllIn==true || iteminfor.IsPlaying==true)
		{
			//Debug.LogWarning(iteminfor.Cards[0].ToString() +" "+iteminfor.Cards[1].ToString());
			if(iteminfor.Cards[0].ToString()!="--" && iteminfor.Cards[1].ToString() !="--")
			{
				CreateHandCards(true,iteminfor.Cards[0].ToString()); 
				CreateHandCards(false,iteminfor.Cards[1].ToString()); 
			}
			else
			{
				CreateHandCards(true,string.Empty); 
				CreateHandCards(false,string.Empty); 
			}
		}
		else if(iteminfor.IsPlaying==false)
		{
			if(Room.Singleton.PokerGame.PlayerActionNames[iteminfor.NoSeat]==PhotonClient.ACTION_FOLD)
			{
				CreateHandCards(true,string.Empty); 
				CreateHandCards(false,string.Empty); 
				SetRoleName(index,LocalizeHelper.Translate("YIGAIPAI")); 

			}
		}
		
 
		
		TableInfo inf=Room.Singleton.PokerGame.TableInfo;
		if(inf!=null && NoSeat!=-1)
		{
			if(NoSeat==inf.m_NoSeatCurrPlayer)
			{
 				if(TableState.Singleton.isPlaying==true)
						MakeEffectRoleShow(index);
			}
		}
		
		if(index==1 && User.Singleton.UserData.NoSeat!=-1)
		{
			ShowIndicatorBarValue();
		}
 		 
		
  	}
	
	void DoPockerJoinGame(ActorInfor ac)
	{
		if(ac.gamblingNo==index)
		{
			//actorinfor=ac;
			NoSeat=ac.NoSeat;
			TableInfo info=Room.Singleton.PokerGame.TableInfo;
			if(info!=null)
			{
				PlayerInfo playerinfo=info.GetPlayer(NoSeat);
				if(playerinfo != null)
				{
					SetRoleDetailInfor(playerinfo,ac);
					SetRoleTranSparent();
					
 				}
			}
  			
		}
 
	}

	void setGiftBg(bool gift)
	{
		Gift_Button.SetActiveRecursively(gift);
 		Gift_ButtonBg.GetComponent<UISprite>().spriteName=GiftIcon;
		if(GiftIcon=="GIFGiftBtn")
			Gift_ButtonBg.transform.localScale=new Vector3(47,47,1);
		else
			Gift_ButtonBg.transform.localScale=new Vector3(50,44,1);

	}
	void initEntiyRole()
	{
 	    UIButtonMessage ButtonRole_message=Button.GetComponent<UIButtonMessage>();
 		Button.GetComponent<BoxCollider>().enabled=true;
  		
  		UISprite bgSprite=Button_RoleBg.GetComponent<UISprite>();
		
		PlayerName=string.Empty;
		if(Util.isMatch())
		{
			bgSprite.spriteName="GIFWait";
			ButtonRole_message.functionName=null;
		}
		else
		{
		
	 		if(User.Singleton.UserData.NoSeat==-1)
			{
				bgSprite.spriteName="GIFSeat";
				ButtonRole_message.functionName="ShowBuyInchip";
			}
			else
			{
				bgSprite.spriteName="GIFInviteFriend";
	 			ButtonRole_message.functionName="InviteFriends";
			
			}
		}
 		Button_RoleBg.transform.localScale=new Vector3(bgSprite.sprite.outer.width,bgSprite.sprite.outer.height,1);

	  
		Util.SetLabelValue(Label_Money.transform,string.Empty);
		GIFBlackBasePlateMask.SetActiveRecursively(false);
 		Vip.SetActiveRecursively(false);
		SpriteD.SetActiveRecursively(false);
		
 		setGiftBg(false);
	 
		Title.SetActiveRecursively(false);
	
	     SetRoleName(index,string.Empty);
		 
		
 		DestoryHandCard();
		DestorySprite();
		
		if(index==1)
			ShowIndicatorBarValue();
		
		MakeEffectRoleShow(-1);
		
		DestroyWinner();
		
		Util.DestoryChildrenGameobject(topPanel,"hanhua"+transform.name);
		
		SetRoleInforTranSparent(false);
		
 	   	 
 	}
	void DestroyWinner()
	{
		if(transform.FindChild(transform.name+"WinerTips"))
		{
			Destroy(transform.FindChild(transform.name+"WinerTips").gameObject);
		}
 
		Transform[] trs=transform.GetComponentsInChildren<Transform>();
		foreach(Transform t in trs)
		{
			if(t.name=="totalchipbgcoln"+transform.name)
			{
 				Destroy(t.gameObject);
			}
		}
		
	}
	
	void DestorySprite()
	{
		if(transform.FindChild("Chips"))
		{
			Destroy(transform.FindChild("Chips").gameObject);
		}
	}
	void DoPckerPlayerLeave(ActorInfor ac,bool isklick,PlayerLeaveType leavetype)
	{
		//Debug.LogError("Player Leave "+transform.name);
		if(ac.gamblingNo==index)
		{
			//Debug.LogError(" "+GiftIcon);
			GiftIcon="GIFGiftBtn";
			initEntiyRole();
 			NoSeat=-1;
			
			SetRoleTranSparent();
		}
		
		if(isklick && User.Singleton.UserData.NoSeat == -1)
		{
			initEntiyRole();
		}
	}
	void DoPockerPlayerJoin(ActorInfor ac)
	{
		//Debug.Log(ac.gamblingNo+" "+index);
		if(ac.gamblingNo==index)
		{
		//	Debug.LogError("ffff "+GiftIcon);
			GiftIcon="GIFGiftBtn";
			
			NoSeat=ac.NoSeat;
			setGiftBg(true);
			
			TableInfo info=Room.Singleton.PokerGame.TableInfo;
			if(info!=null)
			{
				PlayerInfo playerinfo=info.GetPlayer(NoSeat);
				if(playerinfo != null)
				{
					
					SetRoleDetailInfor(playerinfo,ac);
				    SetRoleTranSparent(); 
				}
			}
		}
	}
	void DoPockerPlayerTurnBegan(int Noset,bool isclient)
	{
		if(NoSeat == Noset)
		{
			if(!isclient)
		    	MakeEffectRoleShow(index);	
			
 			SetRoleTranSparent();
		}
		
		if(Noset==-1)
		{
			MakeEffectRoleShow(-1);
		}
	}
	
	bool GetSexy(int avatar)
	{
		bool sex = false;
		
		if (avatar <= 4)
			sex = true;
		
		if (avatar == 9 || avatar == 10)
			sex = true;
		
		return sex;
	}
 
	
	void DoPockerPlayerTurnEnd(int noseat)
	{
		//Debug.LogWarning("DoPockerPlayerTurnEnd "+noseat +" "+index);
		if(NoSeat==noseat && noseat!=-1)
		{
			//Debug.LogError(noseat);
			TableInfo info=Room.Singleton.PokerGame.TableInfo;
			if(info!=null)
			{
		 		PlayerInfo playerinfo=info.GetPlayer(noseat);
	 			if(playerinfo!=null)
				{
					
					if(playerinfo.MoneyBetAmnt>0)
					{
						setSpritechipLabel(Util.getLableMoneyK_MModel(playerinfo.MoneyBetAmnt),playerinfo.MoneyBetAmnt);
			 			Util.SetLabelValue(Label_Money.transform,Util.getLableMoneyK_MModel(playerinfo.MoneySafeAmnt));
					}
 						
 					if(Room.Singleton.PokerGame.PlayerActionNames[noseat]==PhotonClient.ACTION_CHECK)
					{
						if(noseat!=User.Singleton.UserData.NoSeat)
						{
							SoundHelper.PlaySound("Music/Other/Check",AudioSource,0);
							GameSoundHelper.PlaySound(PlayerBehavior.Pass, as2, GetSexy((int)playerinfo.Avator));
						}
 						SetRoleNameWithColor(index,LocalizeHelper.Translate("GUOPAI"),gold);
					}
					else if(Room.Singleton.PokerGame.PlayerActionNames[noseat]==PhotonClient.ACTION_CALL)
					{	
						if(noseat!=User.Singleton.UserData.NoSeat)
						{
							SoundHelper.PlaySound("Music/Other/Call",AudioSource,0);
							GameSoundHelper.PlaySound(PlayerBehavior.Follow, as2, GetSexy((int)playerinfo.Avator));
						}
 						SetRoleNameWithColor(index,LocalizeHelper.Translate("GENZHU"),gold);
 					}
					else if(Room.Singleton.PokerGame.PlayerActionNames[noseat]==PhotonClient.ACTION_RAISE)
					{
						if(noseat!=User.Singleton.UserData.NoSeat)
					  		 SoundHelper.PlaySound("Music/Other/Call",AudioSource,0);
 					   SetRoleNameWithColor(index,LocalizeHelper.Translate("JIAZHU"),gold);
 			
					}
					else if(Room.Singleton.PokerGame.PlayerActionNames[noseat]==PhotonClient.ACTION_FOLD)
					{
 						if(noseat!=User.Singleton.UserData.NoSeat)
						{
							SoundHelper.PlaySound("Music/Other/EveryoneFold",AudioSource,0);
							GameSoundHelper.PlaySound(PlayerBehavior.Fold, as2, GetSexy((int)playerinfo.Avator));
						}
						 
						SetRoleNameWithColor(index,LocalizeHelper.Translate("GAIPAI"),gold);
  					}
					else if(Room.Singleton.PokerGame.PlayerActionNames[noseat]==PhotonClient.ACTION_ALLIN)
					{
  						if(noseat!=User.Singleton.UserData.NoSeat)
						{
							SoundHelper.PlaySound("Music/Other/Allin",AudioSource,0);
							GameSoundHelper.PlaySound(PlayerBehavior.AllIn, as2, GetSexy((int)playerinfo.Avator));
						}
						 
						SetRoleNameWithColor(index,LocalizeHelper.Translate("QUANXIA"),gold);
			
					}
					
				}
			}
  			
			//Debug.LogWarning("DoPockerPlayerTurnEnd "+noseat);
			MakeEffectRoleShow(-1);	
 			SetRoleTranSparent();

		}
	}
    int GetRandomNum(int min,int max)
    {
	     System.Random random=new System.Random();
         return random.Next(min,max);
    }
	void CheckAllRoleColor(int index)
	{
 		 
		Transform Role=null;
		
 		if(Name.GetComponent<UILabel>().color==gold)
		{
			for(int i=1;i<=TableState.Singleton.TotallNumPlayer;i++)
			{
 				Role= transform.parent.FindChild("RoleInfo_"+i);
  				Role.GetComponent<PockerPlayer>().Name.GetComponent<UILabel>().color=white;
 			}
 		}
					
	 
			
		 
 	}
	void SetRoleName(int i,string str)
	{
  		UILabel labl=Name.GetComponent<UILabel>();
		labl.color=white;
 		labl.text=str ;
  	}
	void SetRoleNameWithColor(int i,string str,Color co)
	{
 
		CheckAllRoleColor(i);
   		UILabel labl=Name.GetComponent<UILabel>();
		labl.color=co;
 		labl.text=str;
 		
  	}
	void DoPockerGameStandUp(int UserNoset)
	{
	//	Debug.LogError(transform.name+"  "+NoSeat+" "+User.Singleton.UserData.NoSeat);
		if(!Util.isMatch())
		{
		    if(NoSeat==UserNoset || NoSeat==-1 )
			{
				GiftIcon="GIFGiftBtn";
			    initEntiyRole();
				NoSeat=-1;
				if(index==1)
					ShowIndicatorBarValue();
			}
		}
		else
		{
			if(NoSeat==UserNoset)
			{
				Util.FadeWidtsOutWithTime(gameObject,0.5f);
			}
		}
		
	}
	
	void DopockerBetTurnEnd()
	{
		Transform Chips=transform.FindChild("Chips");
		if(Chips!=null)
		{
			Chips.GetComponent<SpriteChip>().TheChipToWonPot();
		}
		if(NoSeat!=-1)	
			Invoke("DidBetTurnEndeReSetRolesName",0.5f);
	}
	
	void DoPockerGameEnd(long tax,List<ActorInfor> players)
	{
		MakeEffectRoleShow(-1);	
	}
	
	void SetBlindAmnt()
	{
		TableInfo info=Room.Singleton.PokerGame.TableInfo;
		if(info!=null && NoSeat!=-1)
		{
 			//Debug.Log("info.SmallBlindAmnt:"+info.SmallBlindAmnt+" info.BigBlindAmnt:"+info.BigBlindAmnt);
			//Debug.Log("info.NoSeatSmallBlind:"+info.NoSeatSmallBlind+" info.NoSeatBigBlind:"+info.NoSeatBigBlind);
			
			PlayerInfo iteminfo=info.GetPlayer(NoSeat);
			
 			if(NoSeat==info.m_NoSeatBigBlind)
			{
				SetRoleName(index,LocalizeHelper.Translate("BIGBLINDAMNT"));
//				long BigBlindamnt=info.BigBlindAmnt;
//				if(Util.isMatch())
//				{
//					if(iteminfo!=null)
//					{
//						if(BigBlindamnt>iteminfo.MoneySafeAmnt)
//							BigBlindamnt=iteminfo.MoneySafeAmnt;
// 					}
//				}
 				setSpritechipLabel(Util.getLableMoneyK_MModel(iteminfo.MoneyBetAmnt),iteminfo.MoneyBetAmnt);
	 		}
 		    if(NoSeat==info.m_NoSeatSmallBlind)
			{
				SetRoleName(index,LocalizeHelper.Translate("SAMLLBLINDAMNT"));
//				long SmallBlindAmnt=info.SmallBlindAmnt;
//				if(Util.isMatch())
//				{
//					if(iteminfo!=null)
//					{
//						if(SmallBlindAmnt>iteminfo.MoneySafeAmnt)
//							SmallBlindAmnt=iteminfo.MoneySafeAmnt;
// 					}
//				}
				setSpritechipLabel(Util.getLableMoneyK_MModel(iteminfo.MoneyBetAmnt),iteminfo.MoneyBetAmnt);

			} 
			//Debug.LogError(info.NoSeatDealer);
 			if(NoSeat == info.NoSeatDealer)
			{
				SpriteD.SetActiveRecursively(true);
			}
 		}
  	}
	
	void ReSetRoleInfor()
	{
 		SpriteD.SetActiveRecursively(false);
 		 
		DestoryHandCard();
 	 	if(NoSeat==-1)
		{	
			Util.SetLabelValue(Name.transform,string.Empty);
	    }
		else
		{
			TableInfo info=Room.Singleton.PokerGame.TableInfo;
			if(info!=null)
			{
				PlayerInfo item=info.GetPlayer(NoSeat);
				if(item!=null)
				{
					Util.SetLabelValue(Name.transform,item.Name);
 					Util.SetLabelValue(Label_Money.transform,Util.getLableMoneyK_MModel(item.MoneySafeAmnt));
 				} 
			}
			
		}
 		 
		Invoke("SetBlindAmnt",0.1f);
  		MakeEffectRoleShow(-1);
		
		if(index==1)
			ShowIndicatorBarValue();
    		
	}
	
	  
	void DoPockerGameStart()
	{
		if(NoSeat!=-1)
		{
			SetRoleTranSparent();
			DestoryHandCard();
			ReSetRoleInfor();
		}
		else
		{
		
			if(Util.isMatch())
			{
				Util.SetLabelValue(Name,string.Empty);
 			    Util.FadeWidtsOutWithTime(gameObject,0.5f);
 			}
			else
			{
				initEntiyRole();
			}
		}
		
  	}
	void DoPockerGameStateReSet()
	{
		//Debug.LogWarning("DoPockerGameStateReSet");
		NoSeat=-1;
 		initEntiyRole();
	}
	void DoPockerGameNoseatReSet()
	{
		NoSeat=-1;

	}
	void DoPockerGameSitDown(ActorInfor ac)
	{
		GiftIcon="GIFGiftBtn";
 		if(ac.gamblingNo==index)
		{
			NoSeat=ac.NoSeat;
			TableInfo info=Room.Singleton.PokerGame.TableInfo;
			if(info!=null)
			{
				PlayerInfo playerinfo=info.GetPlayer(NoSeat);
				if(playerinfo != null)
				{
				    SetRoleDetailInfor(playerinfo,ac);
				    SetRoleTranSparent(); 
				} 
			}
 		}
 		 
	}
	
	
	
	void DoPockerGameStartSetRoleInfor(ActorInfor ac)
	{
		if(ac.gamblingNo==index)
		{
			NoSeat=ac.NoSeat; 
			TableInfo info=Room.Singleton.PokerGame.TableInfo;
			if(info!=null)
			{
						//Debug.Log("DoPockerGameStartSetRoleInfor " +ac.name +"  "+index +" "+NoSeat);

				PlayerInfo iteminfo=info.GetPlayer(NoSeat);
				if(iteminfo!=null)
				{
						//	Debug.Log("iteminfo " +ac.name +"  "+index +" "+NoSeat);

					SetPockerGameStartSetRoleInfor(iteminfo,ac);
				}
			}
		}
		
		
 	}
	
	void RegisterEventAction()
	{
		TableState.DoPockerGameStartSetRoleInfor+=DoPockerGameStartSetRoleInfor;
		TableState.DoPockerJoinGame+=DoPockerJoinGame;
		TableState.DoPckerPlayerLeave+=DoPckerPlayerLeave;
		TableState.DoPockerPlayerJoin+=DoPockerPlayerJoin;
		TableState.DoPockerPlayerTurnBegan+=DoPockerPlayerTurnBegan;
		TableState.DoPockerPlayerTurnEnd+=DoPockerPlayerTurnEnd;
		TableState.DopockerBetTurnEnd+=DopockerBetTurnEnd;
		TableState.DoPockerGameEnd+=DoPockerGameEnd;
		TableState.DoPockerGameStandUp+=DoPockerGameStandUp;
		TableState.DoPockerGameStart+=DoPockerGameStart;
		
		TableState.DoPockerGameNoseatReSet+=DoPockerGameNoseatReSet;
		
		TableState.DoPockerGameSitDown+=DoPockerGameSitDown;
		TableState.DoPockerGameStateReSet+=DoPockerGameStateReSet;
 		
		TableState.DoPockerPlayerHoleCardsChangedEvent+=DoPockerPlayerHoleCardsChangedEvent;
		TableState.DoPlayersShowCardsFinished+=DoPlayersShowCardsFinished; 
		
		TableState.DopockerBetTurnBegan+=DopockerBetTurnBegan;
		
		PhotonClient.BroadcastMessageInTableEvent+=DidBroadcastMessageInTableEvent;

		
	}
	
	void DoPlayersShowCardsFinished(int Noseat,string cardvalues)
	{
		if(NoSeat==Noseat && Noseat!=-1)
		{
			string[] cards=cardvalues.Split(' ');
			 
			Transform leftTr=transform.FindChild("lefthandcard");
			if(leftTr!=null)
			{
				HandCard leftRoleName=leftTr.GetComponent<HandCard>();
				leftRoleName.GameOverRotateCard(cards[0].ToString());
			}
			Transform RightTr=transform.FindChild("righthandcard");
			if(RightTr!=null)
			{
				HandCard rightRoleName=RightTr.GetComponent<HandCard>();
				rightRoleName.GameOverRotateCard(cards[1].ToString());
			}
			
		}
		
	}
	void DoPockerPlayerHoleCardsChangedEvent( List<ActorInfor> temp)
	{
		 
	}
	void DopockerBetTurnBegan()
	{
 	
	}
	void UnRegisterEventAction()
	{
		TableState.DoPockerGameStartSetRoleInfor-=DoPockerGameStartSetRoleInfor;
		TableState.DoPockerJoinGame-=DoPockerJoinGame;
		TableState.DoPckerPlayerLeave-=DoPckerPlayerLeave;
		TableState.DoPockerPlayerJoin-=DoPockerPlayerJoin;
		TableState.DoPockerPlayerTurnBegan-=DoPockerPlayerTurnBegan;
		TableState.DoPockerPlayerTurnEnd-=DoPockerPlayerTurnEnd;
		TableState.DopockerBetTurnEnd-=DopockerBetTurnEnd;
		TableState.DoPockerGameEnd-=DoPockerGameEnd;
		TableState.DoPockerGameStandUp-=DoPockerGameStandUp;
		TableState.DoPockerGameStart-=DoPockerGameStart;
		TableState.DoPockerGameSitDown-=DoPockerGameSitDown;
		TableState.DoPockerGameStateReSet-=DoPockerGameStateReSet;
		TableState.DoPockerPlayerHoleCardsChangedEvent-=DoPockerPlayerHoleCardsChangedEvent;
		TableState.DoPlayersShowCardsFinished-=DoPlayersShowCardsFinished; 
		TableState.DopockerBetTurnBegan-=DopockerBetTurnBegan;
				TableState.DoPockerGameNoseatReSet-=DoPockerGameNoseatReSet;

		PhotonClient.BroadcastMessageInTableEvent-=DidBroadcastMessageInTableEvent;
		 


	}
	
	void PlaySound(string message, PlayerInfo itemInfo)
	{
		if (!string.IsNullOrEmpty(message) || itemInfo != null)
		{
			if (message == LocalizeHelper.Translate("GAME_SPEAK_1"))
			{
				GameSoundHelper.PlaySound(PlayerBehavior.Speak_01, as2, GetSexy((int)itemInfo.Avator));
			}
			if (message == LocalizeHelper.Translate("GAME_SPEAK_2"))
			{
				GameSoundHelper.PlaySound(PlayerBehavior.Speak_02, as2, GetSexy((int)itemInfo.Avator));
			}
			if (message == LocalizeHelper.Translate("GAME_SPEAK_3"))
			{
				GameSoundHelper.PlaySound(PlayerBehavior.Speak_03, as2, GetSexy((int)itemInfo.Avator));
			}
			if (message == LocalizeHelper.Translate("GAME_SPEAK_4"))
			{
				GameSoundHelper.PlaySound(PlayerBehavior.Speak_04, as2, GetSexy((int)itemInfo.Avator));
			}
		}
	}
	
	void DidBroadcastMessageInTableEvent(int nosear,string message)
	{	
		if(nosear<0)
			return;
		
		if(nosear==NoSeat)
		{
			TableInfo info=Room.Singleton.PokerGame.TableInfo;
			if(info!=null)
			{
				PlayerInfo iteminfo=info.GetPlayer(nosear);
				if(iteminfo!=null)
				{
  		 			ShowHanHuaTip(message);
					PlaySound(message, iteminfo);
				}
 			}
 		}
		
	}
	
	void setSpritechipLabel(string chipnum,long chip)
	{
 		if(transform.FindChild("Chips")==null)
		{
			GameObject blurchip=Resources.Load("prefab/Gambling/Chips") as GameObject;
			GameObject totalchipbgcoln=Instantiate(blurchip.gameObject,new Vector3(0,0,0),Quaternion.identity) as GameObject;
			totalchipbgcoln.transform.parent=transform;
			totalchipbgcoln.transform.localScale=new Vector3(1,1,1);
			totalchipbgcoln.transform.localPosition=new Vector3(0,0,0);
			totalchipbgcoln.name="Chips";
			totalchipbgcoln.GetComponent<SpriteChip>().OnInit(chip,null,NoSeat,index,SpriteChip.ChipType.BetTurnEndChip,0,null);
		}
		else
		{
			Transform Chips=transform.FindChild("Chips");
 			Chips.FindChild("Spritechip").GetComponent<UISprite>().spriteName=Util.GetChipSpriteByChip(chip);
 			Transform labchiptr=Chips.FindChild("spritelabchip");
			labchiptr.GetComponent<UISprite>().spriteName=Util.GetChipSpriteLabelByChip(chip);
 			Util.SetLabelValue(labchiptr,"lab",chipnum);
		}
		 
 	}
	 
	void CreateHandCards(bool left,string cardvalue)
	{ 
		
    		GameObject prefab=Resources.Load("prefab/Gambling/BigHandCard") as GameObject;
  			GameObject handcard=Instantiate(prefab) as GameObject;
			handcard.transform.parent=transform;
	  		handcard.transform.localScale  =  new Vector3(1,1,1);
 			
			if(left)
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
  		    handcardsprite.setDetailInfor(index,left,NoSeat,false,cardvalue);
		    
			if(NoSeat ==User.Singleton.UserData.NoSeat && index==1 && User.Singleton.UserData.NoSeat!=-1)
			{
				handcardsprite.SmallCard=false;
			}
	 
 	}
	void MakeEffectRoleShow(int index)
	{
 		Transform[] trs=makeEffectPanel.transform.GetComponentsInChildren<Transform>(); //.FindChild("makeEffect");
		foreach(Transform tr in trs)
		{
			if( tr.name == "makeEffect"+transform.name)
			{
				tr.GetComponent<GamblingFade>().FadeOut();
 			}
		}
 		 	
		if(index<0)
			return;
  		GameObject makeEffectRole=Instantiate(makeEffectPrefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
 		makeEffectRole.transform.parent=makeEffectPanel.transform;
		makeEffectRole.name="makeEffect"+transform.name;
 		makeEffectRole.GetComponent<MakeEffectRole>().index=index;
		
 		Transform makeeffectObject=makeEffectRole.transform.FindChild("MakeEffectScrpit");
		RectMaskEffect rectmakeeffect=makeeffectObject.GetComponent<RectMaskEffect>();
		rectmakeeffect.StartEffect(Room.Singleton.PokerGame.TableInfo.ThinkingTime);
	}
	void DestoryHandCard()
	{
 		Transform[] trs=transform.GetComponentsInChildren<Transform>();
		foreach(Transform tr in trs)
		{ 
			if(tr!=null)
			{
				if(tr.name=="lefthandcard" || tr.name=="righthandcard" || tr.name=="cardAnim")
				{
				   DestroyImmediate(tr.gameObject);
				} 
			}
		}
	} 
  	 
	
//	
	void SetRoleTranSparent()
	{
		bool TranSparent=false;
		if(NoSeat!=-1)
		{
			 TableInfo info=Room.Singleton.PokerGame.TableInfo;
			 if(info!=null)
			 {
				 PlayerInfo iteminfo=info.GetPlayer(NoSeat);
				 if(iteminfo!=null)
				 {
					if(iteminfo.IsPlaying==true || iteminfo.IsAllIn==true)
					{
						
  					}
					else
					{
						TranSparent=true;
					}
 				 }
 			}
		}
		 
		
		
		if(TranSparent==true)
		{
			SetRoleInforTranSparent(true);
		} 
		else
		{
			SetRoleInforTranSparent(false);
		}
		
	}
	void ShowIndicatorBarValue()
	{
		if(Util.isMatch())
			return;
		
		//remove UserType.Guest PokerPointer limit
		if(SettingManager.Singleton.PokerPointer==true )//&& User.Singleton.UserData.UserType!=UserType.Guest
		{
			if(transform.FindChild("IndicatorBar")==null)
			{
				GameObject prefab=Resources.Load("prefab/Gambling/IndicatorBar") as GameObject;
				GameObject item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
 				item.transform.parent=transform;
				item.transform.localPosition=new Vector3(-126,-195,0);
				item.transform.localScale =new Vector3(1,1,1);
				item.name="IndicatorBar";
			}
		
		}
		
		if(User.Singleton.UserData.NoSeat==-1)
		{
			if(transform.FindChild("IndicatorBar")!=null)
			{
				Destroy(transform.FindChild("IndicatorBar").gameObject);
			}
 		}
		 
		 
		//remove UserType.Guest limit
//		if(User.Singleton.UserData.UserType==UserType.Guest)
//		{
// 			if(IndicatorBarHasShowed==false && User.Singleton.UserData.NoSeat!=-1)
//			{
//				
//				PlayerInfo playinfo =Room.Singleton.PokerGame.TableInfo.GetPlayer(User.Singleton.UserData.NoSeat); 
//				if(playinfo.IsPlaying==true)
//				{
//					IndicatorBarHasShowed=true;
//					
//					if(playinfo!=null)
//					{
//						GameObject prefab=Resources.Load("prefab/Gambling/IndicatorReg") as GameObject;
//						GameObject item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
//		 				item.transform.parent=topPanel.transform;
//						item.transform.localPosition=new Vector3(0,0,0);
//						item.transform.localScale =new Vector3(1,1,1);
//						
//						
//						Transform  Button_reg=item.transform.FindChild("Button_reg");
//						UIButtonMessage btnmess=Button_reg.GetComponent<UIButtonMessage>();
//						btnmess.functionName="QuickRegister";
//						btnmess.target =transform.parent.gameObject;
//					}
//					
//				}
//			}
//		}
			
		
	}
		
	 
	void showTitleTip(ActorInfor act,string ico_name)
	{
 		UISprite sprite=Title.GetComponent<UISprite>();
		sprite.spriteName=ico_name;
		Title.SetActiveRecursively(true);
		//Title.transform.localScale=new Vector3(45,45,1);
 	}
 	void ShowHanHuaTip(String text)
	{
		if(!SettingManager.Singleton.ChatBubble)
		{
			return;
		}
	    
		if(index==-1)
			return;
		
		Util.DestoryChildrenGameobject(topPanel.transform,"hanhua"+transform.name);
		
  		GameObject hanhuaparen=Instantiate(HanHuaTip,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		hanhuaparen.transform.parent=topPanel.transform;
		hanhuaparen.name="hanhua"+transform.name;
		hanhuaparen.GetComponent<PockerHanHua>().index=index;
		hanhuaparen.GetComponent<PockerHanHua>().text=text;
  	 
 	}
	void SetRoleInforTranSparent(bool iswork)
	{
  		Transform[] Roletrs=transform.GetComponentsInChildren<Transform>();
		foreach(Transform tr in Roletrs)
		{
			if(tr.name != "BigBackCard" && tr.name!="BigFaceCard"  &&  tr.name!="BigCardBack1" && tr.name !="BigCardBack2" && tr.name !="BigCardFace1" && tr.name !="BigCardFace2"
			 && tr.name != "spritelabchip" && tr.name !="Spritechip" && tr.name !="lab" &&
				tr !=SpriteD.transform  && tr.name !="Background")
			{
				UIWidget ui=tr.GetComponent<UIWidget>();
				if(ui!=null)
				{
					Color m=ui.color;
					m.a=iswork?0.5f:1;
					ui.color=m;
				}
			}
 		}
	} 
	
}

//GameObject GetAnimationEasyMotionResouce(ActorInfor actorinfor)
//	{
// 		Transform  Role=null;
//		Transform parentTran=null;
//		if(actorinfor.gamblingNo>=4 && actorinfor.gamblingNo<=7)
//		{
//		    Role=EasyMotionCamera1.transform.FindChild(actorinfor.RoleName);
//			parentTran=EasyMotionCamera1.transform;
//		}
//		else
//		{
//			Role=EasyMotionCamera2.transform.FindChild(actorinfor.RoleName);
//			parentTran=EasyMotionCamera2.transform;
//
//		}
//
//		if(Role==null)
//		{
//		  
// 			Debug.Log("prefab/chactors/"+parentTran.name+actorinfor.EasyMotionName);
//			GameObject roleAn=Resources.Load("prefab/chactors/"+parentTran.name+actorinfor.EasyMotionName) as GameObject;
// 			GameObject roleColne=Instantiate(roleAn,new Vector3(0,0,0),Quaternion.identity) as GameObject; 
//			
// 			roleColne.transform.parent=parentTran;
//			roleColne.name=transform.name;
//			
//  			switch(actorinfor.gamblingNo)
//			{
//			     case 1:
// 					    				roleColne.transform.localPosition=new Vector3(-6,-227,0);
// 
//				    break;
//				 case 2:
//										roleColne.transform.localPosition=new Vector3(-246,-227,0);
//				 
//				    break;
//				 case 3:
//										roleColne.transform.localPosition=new Vector3(-420,-138,0);
// 
//				    break;
//				 case 4:
//										roleColne.transform.localPosition=new Vector3(-420,76,0);
// 
//				    break;
//				 case 5:
//										roleColne.transform.localPosition=new Vector3(-148,160,0);
// 
//				    break;
//				 case 6:
//									roleColne.transform.localPosition=new Vector3(135,160,0);
// 
//				    break;
//				 case 7:
//										roleColne.transform.localPosition=new Vector3(398,72,0);
// 
//				    break;
//				 case 8:
//										roleColne.transform.localPosition=new Vector3(403,-135,0);
// 
//				    break;
//				 case 9:
//										roleColne.transform.localPosition=new Vector3(235,-227,0);
//
//				    break;
//			}
//			
//			if(actorinfor.gamblingNo>=1 && actorinfor.gamblingNo<=5)
//			{
//				if(actorinfor.EasyMotionName=="Pirate" || actorinfor.EasyMotionName=="EurPrin" )
//				{
//					roleColne.transform.localScale=new Vector3(roleColne.transform.localScale.x,roleColne.transform.localScale.y,roleColne.transform.localScale.z);
//
//				}
//				else
//				{
//					roleColne.transform.localScale=new Vector3(-roleColne.transform.localScale.x,roleColne.transform.localScale.y,roleColne.transform.localScale.z);
//				}
//
//			}
//			else
//			{
//				if(actorinfor.EasyMotionName=="Pirate"|| actorinfor.EasyMotionName=="EurPrin" )
//				{
//					roleColne.transform.localScale=new Vector3(-roleColne.transform.localScale.x,roleColne.transform.localScale.y,roleColne.transform.localScale.z);
//
//				}
//				else
//				{
//					roleColne.transform.localScale=new Vector3(roleColne.transform.localScale.x,roleColne.transform.localScale.y,roleColne.transform.localScale.z);
//				}
//			}
//  			
//			if(actorinfor.EasyMotionName=="EurPrin" ||actorinfor.EasyMotionName=="JapLoli"||actorinfor.EasyMotionName=="MaGir" ||actorinfor.EasyMotionName=="Pirate")
//			 		roleColne.transform.localPosition=new Vector3(roleColne.transform.localPosition.x+9,roleColne.transform.localPosition.y+66,0);
//			
// 			return roleColne;
// 			
// 		}
//		else
//		{
//			return Role.gameObject;
//		}
//	} 
//  	
//	
//	void PlayWinAnimationEmasyName(ActorInfor actorinfor){
//		if(!SettingManager.Singleton.Animation)
//		{
//			return;
//		}
//		
//	    if(string.IsNullOrEmpty(actorinfor.EasyMotionName))
//			return; 
//		
//  		GameObject roleColne=GetAnimationEasyMotionResouce(actorinfor);
//		EasyMotion2D.SpriteAnimation spriteanimation=roleColne.GetComponent<EasyMotion2D.SpriteAnimation>();
//		 
//		EasyMotion2D.SpriteAnimationClip[] clips = SpriteAnimationUtility.GetAnimationClips(spriteanimation);
//		foreach(EasyMotion2D.SpriteAnimationClip clip in clips)
//		{
//		    if(clip.name==actorinfor.EasyMotionName+"_Table_Win_Ani")
//			{
//				spriteanimation.clip=clip;
//			}
//		}
//		spriteanimation.Play(actorinfor.EasyMotionName+"_Table_Win_Ani");
//		
//		 
//	   
//		if(actorinfor.gamblingNo>=1 && actorinfor.gamblingNo<=5)
//		{
//			if(actorinfor.EasyMotionName=="MaGir")
//			{
//				roleColne.transform.localScale=new Vector3(roleColne.transform.localScale.x,roleColne.transform.localScale.y,roleColne.transform.localScale.z);
// 	
//			}
//		}
//		else
//		{
//			if(actorinfor.EasyMotionName=="MaGir")
//			{
//				roleColne.transform.localScale=new Vector3(-roleColne.transform.localScale.x,roleColne.transform.localScale.y,roleColne.transform.localScale.z);
//
//			}
//		}
//	}
//	
//	bool checkAniamtion()
//	{
//		Transform par;
//		if(index>=1 && index<=5)
//		{
//			 par=EasyMotionCamera1;
//		}
//		else
//		{
//			 par=EasyMotionCamera2;
//		}
//		
//		return par.FindChild(transform.name)==null;
//		
//		
//	}
//	
//	void PlayLoseAnimationEasyName(ActorInfor actorinfor)
//	{
//		if(!SettingManager.Singleton.Animation)
//		{
//			return;
//		}
//		if(string.IsNullOrEmpty(actorinfor.EasyMotionName))
//			return;
//		
//		GameObject roleColne=GetAnimationEasyMotionResouce(actorinfor);
//		EasyMotion2D.SpriteAnimation spriteanimation=roleColne.GetComponent<EasyMotion2D.SpriteAnimation>();
//		EasyMotion2D.SpriteAnimationClip[] clips = SpriteAnimationUtility.GetAnimationClips(spriteanimation);
//		foreach(EasyMotion2D.SpriteAnimationClip clip in clips)
//		{
//		    if(clip.name==actorinfor.EasyMotionName+"_Table_Lose_Ani")
//			{
//				spriteanimation.clip=clip;
//			}
//		}
//		spriteanimation.Play(actorinfor.EasyMotionName+"_Table_Lose_Ani");
//		
//		ChactorController controller=roleColne.GetComponent<ChactorController>();
//		controller.playerPanel = gameObject;
//		controller.actorinfor = actorinfor;
//		controller.SetAnimationState(actorinfor.EasyMotionName+"_Table_Lose_Ani",spriteanimation);
//	    
//		if(actorinfor.gamblingNo>=1 && actorinfor.gamblingNo<=5)
//		{
//			if(actorinfor.EasyMotionName=="MaGir")
//			{
//				roleColne.transform.localScale=new Vector3(-roleColne.transform.localScale.x,roleColne.transform.localScale.y,roleColne.transform.localScale.z);
// 	
//			}
//		}
//		else
//		{
//			if(actorinfor.EasyMotionName=="MaGir")
//			{
//				roleColne.transform.localScale=new Vector3(roleColne.transform.localScale.x,roleColne.transform.localScale.y,roleColne.transform.localScale.z);
//
//			}
//		}
//	}
//	
//	void PlayAllinAnimationEasyName(ActorInfor actorinfor)
//	{
//		if(!SettingManager.Singleton.Animation)
//		{
//			return;
//		}
//		if(string.IsNullOrEmpty(actorinfor.EasyMotionName))
//			return;
//		
//		GameObject roleColne=GetAnimationEasyMotionResouce(actorinfor);
//		EasyMotion2D.SpriteAnimation spriteanimation=roleColne.GetComponent<EasyMotion2D.SpriteAnimation>();
// 		
//		EasyMotion2D.SpriteAnimationClip[] clips = SpriteAnimationUtility.GetAnimationClips(spriteanimation);
//		foreach(EasyMotion2D.SpriteAnimationClip clip in clips)
//		{
//		    if(clip.name==actorinfor.EasyMotionName+"_Table_Allin_Ani")
//			{
//				spriteanimation.clip=clip;
//			}
//		}
//		spriteanimation.Play(actorinfor.EasyMotionName+"_Table_Allin_Ani");
//		
//		ChactorController controller=roleColne.GetComponent<ChactorController>();
//		controller.playerPanel=gameObject;
//		controller.actorinfor = actorinfor;
// 		controller.SetAnimationState(actorinfor.EasyMotionName+"_Table_Allin_Ani",spriteanimation);
//		if(actorinfor.gamblingNo>=1 && actorinfor.gamblingNo<=5)
//		{
//			if(actorinfor.EasyMotionName=="MaGir")
//			{
//				roleColne.transform.localScale=new Vector3(roleColne.transform.localScale.x,roleColne.transform.localScale.y,roleColne.transform.localScale.z);
// 	
//			}
//		}
//		else
//		{
//			if(actorinfor.EasyMotionName=="MaGir")
//			{
//				roleColne.transform.localScale=new Vector3(-roleColne.transform.localScale.x,roleColne.transform.localScale.y,roleColne.transform.localScale.z);
//
//			}
//		}
//	    
//		
//	}
//
// 	void PlayAngryAnimationEasyName(ActorInfor actorinfor)		
//	{
//		if(!SettingManager.Singleton.Animation)
//		{
//			return;
//		}
//		if(string.IsNullOrEmpty(actorinfor.EasyMotionName))
//			return;
//		GameObject roleColne=GetAnimationEasyMotionResouce(actorinfor);
//		EasyMotion2D.SpriteAnimation spriteanimation=roleColne.GetComponent<EasyMotion2D.SpriteAnimation>();
// 		
//		EasyMotion2D.SpriteAnimationClip[] clips = SpriteAnimationUtility.GetAnimationClips(spriteanimation);
//		foreach(EasyMotion2D.SpriteAnimationClip clip in clips)
//		{
//		    if(clip.name==actorinfor.EasyMotionName+"_Table_Angry_Ani")
//			{
//				spriteanimation.clip=clip;
//			}
//		}
//		spriteanimation.Play(actorinfor.EasyMotionName+"_Table_Angry_Ani");
//		
//		ChactorController controller=roleColne.GetComponent<ChactorController>();
//		controller.playerPanel=gameObject;
//		controller.actorinfor = actorinfor;
// 		controller.SetAnimationState(actorinfor.EasyMotionName+"_Table_Angry_Ani",spriteanimation);
// 		
//		
//		if(actorinfor.gamblingNo>=1 && actorinfor.gamblingNo<=5)
//		{
//			if(actorinfor.EasyMotionName=="MaGir")
//			{
//				roleColne.transform.localScale=new Vector3(-roleColne.transform.localScale.x,roleColne.transform.localScale.y,roleColne.transform.localScale.z);
// 	
//			}
//		}
//		else
//		{
//			if(actorinfor.EasyMotionName=="MaGir")
//			{
//				roleColne.transform.localScale=new Vector3(roleColne.transform.localScale.x,roleColne.transform.localScale.y,roleColne.transform.localScale.z);
//
//			}
//		}
//	}
//	/ <summary>
/// /
/// </summary>