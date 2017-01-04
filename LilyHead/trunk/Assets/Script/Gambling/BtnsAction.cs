using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using DataPersist;
 using System;
using LilyHeart;
public class BtnsAction : MonoBehaviour {
	
 	public GameObject topPanel;
 
	public AudioSource AudioSource {get;set;}
	private AudioSource as2;
 
	public PlayMakerFSM fsm5;
	public PlayMakerFSM fsm4;
	public PlayMakerFSM fsm3;
	public PlayMakerFSM fsm2;
	public PlayMakerFSM fsm1;
	
	long TheRaise=-1;
	
	long PreGenzhu=-1;
	
	void TurnToMyOther(TableInfo info,PlayerInfo playinfo)
	{
		Debug.LogWarning("TurnToMyOther");
	//	Debug.LogWarning(fsm3.ActiveStateName+" "+PreGenzhu);
		fsm4.SendEvent("btn4Suiyigenzhupre");
	    
	    
		fsm1.SendEvent("btn1gaipaipre");
		
	
	   	if(info.HigherBet<=playinfo.MoneyBetAmnt)
		{
			fsm2.SendEvent("btn2g_pPre");
			fsm3.SendEvent("btn3guopaopre");
		}
		else
		{
		   
			if(info.HigherBet>playinfo.MoneySafeAmnt+playinfo.MoneyBetAmnt)
			{
				if(preaction==3)
				{
					preaction=-1;
					PreGenzhu=-1;
				}
				fsm3.SendEvent("btn3genzhugray");
			}
			else
			{
				if(fsm3.ActiveStateName=="btn3guopaipreselect" )
				{
				    if(preaction==3 )
					{
						preaction=-1;
					}
				}
			
				if(fsm3.ActiveStateName=="btn3genzhuselect")
				{
					//Debug.LogWarning("info.HigherBet: "+info.HigherBet +" "+PreGenzhu );
					if(info.HigherBet>PreGenzhu && PreGenzhu!=-1)
					{
						PreGenzhu=-1;
						preaction=-1;
					}
				}
				
				if(fsm3.ActiveStateName=="btn3genzhuselect")
				{
					if(PreGenzhu==-1)
						fsm3.SendEvent("pre");
				}
				else
					fsm3.SendEvent("btn3genzhupre");
			}
	        
			if(fsm2.ActiveStateName != "btn2g_pSelect")
				fsm2.SendEvent("btn2g_pGray");
		}
	
		long tempo;
		if((2*(info.HigherBet-playinfo.MoneyBetAmnt)+playinfo.MoneyBetAmnt)==0)
				tempo=info.BigBlindAmnt;
		else
				tempo=(2*(info.HigherBet-playinfo.MoneyBetAmnt)+playinfo.MoneyBetAmnt);
	
		if(tempo>playinfo.MoneySafeAmnt && info.HigherBet > playinfo.MoneyBetAmnt)
		{
			fsm5.SendEvent("btn5quanxia_pre");
		}
		else
		{
			if(tempo>=getMaxAllinValue() && info.HigherBet > playinfo.MoneyBetAmnt)
			{
				fsm5.SendEvent("btn5quanxia_pre");
			}
			else
			{
				fsm5.SendEvent("btn5JiazhuGray");
			}
		}
	}
	
	void TurnToMySelf(TableInfo info,PlayerInfo playinfo)
	{
		Debug.LogWarning("TurnToMySelf");
		fsm1.SendEvent("btn1gaipai");
		fsm2.SendEvent("btn2g_pGray");
		
		 
	    if(info.HigherBet<=playinfo.MoneyBetAmnt)
		{
			// Debug.Log("info.HigherBet<=playinfo.MoneyBetAmnt");
		    fsm3.SendEvent("btn3guopai");
			fsm4.SendEvent("btn4genzhupray");
			fsm5.SendEvent("btn5jiazhu");
		}
		else
		{
			// Debug.Log("info.HigherBet>playinfo.MoneyBetAmnt");
					// Debug.Log(fsm3.ActiveStateName);

			if(fsm3.ActiveStateName=="btn3genzhuselect" || fsm3.ActiveStateName=="btn3genzhupre")
		   		fsm3.SendEvent("btn3genzhugray");
			else
			    fsm3.SendEvent("btn3guopaigray");
		
		
			//Debug.Log(fsm5.ActiveStateName);
		    if(info.HigherBet<=playinfo.MoneySafeAmnt+playinfo.MoneyBetAmnt)
			{
 				fsm4.SendEvent("btn4genzhu");
 			}
			else
			{
				fsm4.SendEvent("btn4genzhupray");
				//fsm5.SendEvent("btn5quanxia");
			}
			
			long tempo;
			if((2*(info.HigherBet-playinfo.MoneyBetAmnt)+playinfo.MoneyBetAmnt)==0)
					tempo=info.BigBlindAmnt;
			else
					tempo=(2*(info.HigherBet-playinfo.MoneyBetAmnt)+playinfo.MoneyBetAmnt);
		
			if(tempo>=playinfo.MoneySafeAmnt)
			{
				fsm5.SendEvent("btn5quanxia");
			}
			else
			{
				if(tempo>=getMaxAllinValue())
				{
					fsm5.SendEvent("btn5quanxia");
				} 
				else
				{
				    fsm5.SendEvent("btn5jiazhu");
				}
			}
	
			
		}
	}
	
	void GrayALlButtons()
	{
		if(fsm1==null || fsm3==null || fsm2==null || fsm4==null || fsm5==null)
			return;
		
		fsm1.SendEvent("btn1gaipaigray");
		fsm2.SendEvent("btn2g_pGray");
		fsm3.SendEvent("btn3guopaigray");
		
		if(fsm4.ActiveStateName == "btn4genzhu")
		{
		    fsm4.SendEvent("btn4genzhupray");	
		}
		else
			fsm4.SendEvent("btn4suiyigenzhugray");
		fsm5.SendEvent("btn5JiazhuGray");
		PreGenzhu=-1;
	}
	
    void  Buttons_StateCheck() 
	{
			//Debug.Log(fsm3.ActiveStateName);
 			 //Debug.LogWarning("99999999999999999999BButtons_StateCheck");
			TableInfo info=Room.Singleton.PokerGame.TableInfo;
			      
			//Debug.Log("User.Singleton.UserData.NoSeat "+User.Singleton.UserData.NoSeat);	
			if(User.Singleton.UserData.NoSeat==-1 || Room.Singleton.PokerGame.TableInfo==null || info==null)
			{
				GrayALlButtons();
 			}
			else
			{
				
				PlayerInfo playinfo=info.GetPlayer(User.Singleton.UserData.NoSeat);
				// Debug.LogWarning(";;;;;"+info.NoSeatCurrPlayer+"   "+playinfo.IsPlaying +"  "+playinfo.Name);
				if(playinfo!=null)
				{
					if(playinfo.IsPlaying==false || info.NoSeatCurrPlayer==-1 || Room.Singleton.PokerGame.PlayerActionNames[playinfo.NoSeat]==PhotonClient.ACTION_ALLIN ||
					Room.Singleton.PokerGame.PlayerActionNames[playinfo.NoSeat]==PhotonClient.ACTION_FOLD )
					{
						GrayALlButtons();
					}
					else
					{
						if(info.NoSeatCurrPlayer==User.Singleton.UserData.NoSeat)
						{
							TurnToMySelf(info,playinfo);
						}
						else
						{
  						   TurnToMyOther(info,playinfo);
 					 	}
					}
  				}
				else
				{
						GrayALlButtons();
 				}
			}
			 
	 
	}
	
	int preaction=-1;
	
	void btn5PreAction()
	{
  		if(fsm1.ActiveStateName=="btn1gaipaiselect")
    		fsm1.SendEvent("preAc");
 		if(fsm2.ActiveStateName=="btn2g_pSelect")
 	    	fsm2.SendEvent("preAc");
 		if(fsm4.ActiveStateName=="btn4SuiyigenzhupreSelect")
 			fsm4.SendEvent("preAc");
 		if(fsm3.ActiveStateName=="btn3guopaipreselect")
		{
			fsm3.SendEvent("guopaiPreAc");
		}
		else if(fsm3.ActiveStateName=="btn3genzhuselect")
		{
 			fsm3.SendEvent("genzhupreAc");
		}
 	}
	void unbtn5PreAction()
	{
 	} 
 
 	void btn4PreAction()
	{
 		if(fsm1.ActiveStateName=="btn1gaipaiselect")
    		fsm1.SendEvent("preAc");
 		if(fsm2.ActiveStateName=="btn2g_pSelect")
 	    	fsm2.SendEvent("preAc");
 		if(fsm3.ActiveStateName=="btn3guopaipreselect")
		{
			fsm3.SendEvent("guopaiPreAc");
		}
		else if(fsm3.ActiveStateName=="btn3genzhuselect")
		{
 			fsm3.SendEvent("genzhupreAc");
		}
 		if(fsm5.ActiveStateName=="btn5quanxia_preSelect")
 			fsm5.SendEvent("preAc");
 	}
	void unbtn4PreAction()
	{
 
	}
	
	void btn3PreAction()
	{
		if(fsm1.ActiveStateName=="btn1gaipaiselect")
    		fsm1.SendEvent("preAc");
		if(fsm2.ActiveStateName=="btn2g_pSelect")
 	    	fsm2.SendEvent("preAc");
 		if(fsm4.ActiveStateName=="btn4SuiyigenzhupreSelect")
 			fsm4.SendEvent("preAc");
		if(fsm5.ActiveStateName=="btn5quanxia_preSelect")
 			fsm5.SendEvent("preAc");
 		Debug.Log("btn3PreAction");
		
	}
	void unbtn3PreAction()
	{
	 
 
	}
 
 	void btn2PreAction()
	{
 		if(fsm1.ActiveStateName=="btn1gaipaiselect")
    		fsm1.SendEvent("preAc");
		if(fsm3.ActiveStateName=="btn3guopaipreselect")
		{
			fsm3.SendEvent("guopaiPreAc");
		}
		else if(fsm3.ActiveStateName=="btn3genzhuselect")
		{
 			fsm3.SendEvent("genzhupreAc");
		}
 		if(fsm4.ActiveStateName=="btn4SuiyigenzhupreSelect")
 			fsm4.SendEvent("preAc");
		if(fsm5.ActiveStateName=="btn5quanxia_preSelect")
 			fsm5.SendEvent("preAc");		
 		Debug.Log("btn2PreAction");
		
	}
	void unbtn2PreAction()
	{
		 
		
 
	}
	void btn1PreAction()
	{
 		if(fsm3.ActiveStateName=="btn3guopaipreselect")
		{
			fsm3.SendEvent("guopaiPreAc");
		}
		else if(fsm3.ActiveStateName=="btn3genzhuselect")
		{
 			fsm3.SendEvent("genzhupreAc");
		}
		if(fsm2.ActiveStateName=="btn2g_pSelect")
 	    	fsm2.SendEvent("preAc");
		if(fsm4.ActiveStateName=="btn4SuiyigenzhupreSelect")
 			fsm4.SendEvent("preAc");
		if(fsm5.ActiveStateName=="btn5quanxia_preSelect")
 			fsm5.SendEvent("preAc");
		
		 
		Debug.Log("btn1PreAction");
		
	}
	void unbtn1PreAction()
	{
		 
 
	}
 	
	
 	void Start () {
		preaction=-1;
		PreGenzhu=-1;
	      
 		 CheckBtnState(false);
 		AudioSource=gameObject.AddComponent<AudioSource>();
		as2 = gameObject.AddComponent<AudioSource>();
 	}
	
 
 
 
 
	void CheckBtnState(bool NeedTocheck)
	{
		if(!CheckAllButtonDisAble())
		{
 			Buttons_StateCheck(); 
		}
 	}
	
	 
	void Update() {
	   
	}
	
	bool CheckAllButtonDisAble()
	{
		bool flag=topPanel.transform.FindChild("Sprite (blackmaskbg)").gameObject.active;
		if(flag && preaction==3)
		{
			 TableInfo info=Room.Singleton.PokerGame.TableInfo;
			 if(info!=null && User.Singleton.UserData.NoSeat!=-1)
			 {
				 PlayerInfo myitem=info.GetPlayer(User.Singleton.UserData.NoSeat);
				 if(myitem.MoneyBetAmnt<info.HigherBet)
				 {
					 preaction=-1;
				 }
			 }
		}
		return flag;
	}
	
	void BecomeButtonBeGray(Transform btn,bool isGray)
	{
		  UISprite	Background=btn.transform.FindChild("Background").GetComponent<UISprite>();
		  Background.color=isGray?Color.gray:Color.white ;
		   
		  UISprite	btnText=btn.transform.FindChild("btnText").GetComponent<UISprite>();
	 	  btnText.color=isGray?Color.gray:Color.white ;
	
		Transform Boxtr=btn.transform.FindChild("GIFBox"); 
		if(Boxtr!=null)
		{
			UISprite boxprite=Boxtr.GetComponent<UISprite>();
	 		Color mcolor=boxprite.color;
			mcolor.a=0;
			boxprite.color=mcolor;
			
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
	
	void DoActionByPreBtnAction()
	{
		TableInfo info=Room.Singleton.PokerGame.TableInfo;
		if(info==null)
			return;
		 PlayerInfo iteminfo=info.GetPlayer(User.Singleton.UserData.NoSeat);
 		 
		
		if(preaction==1)
		{
			TableState.Singleton.Fold();
			SoundHelper.PlaySound("Music/Other/EveryoneFold",AudioSource,0);
			
			GameSoundHelper.PlaySound(PlayerBehavior.Fold, as2, GetSexy((int)User.Singleton.UserData.Avator));
 		}
		else if(preaction==2)
		{
  			if(iteminfo.NoSeat==User.Singleton.UserData.NoSeat)
			{
 				if(info.CallAmnt(iteminfo)>0)
				{
					TableState.Singleton.Fold();
					SoundHelper.PlaySound("Music/Other/EveryoneFold",AudioSource,0);
					GameSoundHelper.PlaySound(PlayerBehavior.Fold, as2, GetSexy((int)User.Singleton.UserData.Avator));
 				}
				else
				{
					TableState.Singleton.Call();
					SoundHelper.PlaySound("Music/Other/Check",AudioSource,0);
					GameSoundHelper.PlaySound(PlayerBehavior.Pass, as2, GetSexy((int)User.Singleton.UserData.Avator));
					 
				}
			}
		}
		else if(preaction==3)
		{
			if(iteminfo.MoneyBetAmnt == info.HigherBet)
			{
			  	SoundHelper.PlaySound("Music/Other/Check",AudioSource,0);
				GameSoundHelper.PlaySound(PlayerBehavior.Pass, as2, GetSexy((int)User.Singleton.UserData.Avator));
			}
		    else
			{
			  	SoundHelper.PlaySound("Music/Other/Call",AudioSource,0);
				GameSoundHelper.PlaySound(PlayerBehavior.Follow, as2, GetSexy((int)User.Singleton.UserData.Avator));
			}
			TableState.Singleton.Call();
			 
			
			
 		}
		else if(preaction==4)
		{
			if(iteminfo.MoneyBetAmnt == info.HigherBet)
			{
			    SoundHelper.PlaySound("Music/Other/Check",AudioSource,0);
				GameSoundHelper.PlaySound(PlayerBehavior.Pass, as2, GetSexy((int)User.Singleton.UserData.Avator));
			}
		   else
			{
			    SoundHelper.PlaySound("Music/Other/Call",AudioSource,0);
				GameSoundHelper.PlaySound(PlayerBehavior.Follow, as2, GetSexy((int)User.Singleton.UserData.Avator));
			}
			TableState.Singleton.Call();
			 
 		}
		else if(preaction==5)
		{
			TableState.Singleton.Raise(getMaxAllinValue());
			SoundHelper.PlaySound("Music/Other/Allin",AudioSource,0);
			GameSoundHelper.PlaySound(PlayerBehavior.AllIn, as2, GetSexy((int)User.Singleton.UserData.Avator));
 		}
		preaction=-1;
		PreGenzhu=-1;
	}
	
	void DidPlayerTurnBeganEvent(int curretNoseat,bool isClient)
	{
   	   	if(User.Singleton.UserData.NoSeat>-1)
		{
			if(curretNoseat==User.Singleton.UserData.NoSeat)
			{
				Debug.Log("preaction:"+preaction);
				if(preaction==-1)
				{
					CheckBtnState(true);
  				}
				else
				{
  					DoActionByPreBtnAction();
				}
   			}
			else 
			{
				CheckBtnState(true);

			}
 		}
  		 
	}
	
	
	 
	
	void DoPockerPlayerTurnEnd(int NoSeat)
	{
		 
   		 CheckBtnState(true);
 		 
   	}

	public void DisAllBtns(bool iswork)
	{
	    
		if(iswork==true)
		{
		   fsm1.SendEvent("pregray");
		   fsm2.SendEvent("pregray");
		   fsm3.SendEvent("pregray");
		   fsm4.SendEvent("pregray");
		   fsm5.SendEvent("pregray");
 		}
		else
		{
		//	Debug.Log("DisAllBtns(bool iswork)");
 			Buttons_StateCheck(); 
			
		//	Debug.LogWarning(preaction);
			if(preaction==1)
				fsm1.SendEvent("pre");
			
			else if(preaction==2)
				fsm2.SendEvent("pre");
			else if(preaction==3)
				fsm3.SendEvent("pre");
			else if(preaction==4)
				fsm4.SendEvent("pre");
			else if(preaction==5)
				fsm5.SendEvent("pre");
		 
		}
	}
 
	bool CheckPreBtnAction()
	{
		TableInfo info=Room.Singleton.PokerGame.TableInfo;
		
		return (info.NoSeatCurrPlayer!= User.Singleton.UserData.NoSeat);
	}
	void ShowJiaZhuViewTip(bool iswork)
	{
		 topPanel.transform.FindChild("Button_GreenBtn").gameObject.SetActiveRecursively(iswork);
		 topPanel.transform.FindChild("sliderchip").gameObject.SetActiveRecursively(iswork);
  
		if(iswork==true)
		{
			topPanel.transform.FindChild("sliderchip").GetComponent<UISlider>().sliderValue=0;
			
		}
		DisAbleRoleBtn(!iswork);
 
	}
	
	 
	void btn4action(GameObject btn)
	{
		 TableInfo info=Room.Singleton.PokerGame.TableInfo;
		if (info == null)
			return;
		
		if(CheckPreBtnAction())
		{
			if(preaction==4)
				preaction=-1;
			else
				preaction=4;
 		}
		else
		{
 			 ShowJiaZhuViewTip(false);
			TableState.Singleton.Call();
			SoundHelper.PlaySound("Music/Other/Call",AudioSource,0);
			GameSoundHelper.PlaySound(PlayerBehavior.Follow, as2, GetSexy((int)User.Singleton.UserData.Avator));
 		}
		
	}
	
	void btn3action(GameObject btn)
	{
		TableInfo info=Room.Singleton.PokerGame.TableInfo;
		if (info == null)
			return;
		if(CheckPreBtnAction())
		{
			if(preaction==3)
			{
				preaction=-1;
				PreGenzhu=-1;
			}
			else
			{
				PreGenzhu=info.HigherBet;
				
				Debug.LogWarning(PreGenzhu+" "+info.HigherBet);
				preaction=3;
			}
   		}
		else
		{	
			
 			ShowJiaZhuViewTip(false);
			PlayerInfo iteminfo=info.GetPlayer(User.Singleton.UserData.NoSeat);
			if(iteminfo!=null)
			{
			   if(iteminfo.MoneyBetAmnt == info.HigherBet)
			   {
				  SoundHelper.PlaySound("Music/Other/Check",AudioSource,0);
				  GameSoundHelper.PlaySound(PlayerBehavior.Pass, as2, GetSexy((int)User.Singleton.UserData.Avator));
			   }
			   else
			   {
				  SoundHelper.PlaySound("Music/Other/Call",AudioSource,0);
				  GameSoundHelper.PlaySound(PlayerBehavior.Follow, as2, GetSexy((int)User.Singleton.UserData.Avator));
			   }
			}
			TableState.Singleton.Call();
			 
		}
	}
	
	 
	void btn2action(GameObject btn)
	{
		TableInfo info=Room.Singleton.PokerGame.TableInfo;
		if (info == null)
			return;
		
   		if(CheckPreBtnAction())
		{
			 if(preaction==2)
				preaction=-1;
			else
				preaction=2;
  		}
		else
		{
			//ShowJiaZhuViewTip(false);
			//User.Singleton.Call();
		}
	}
	
	 
 	void btn5action(GameObject btn)
	{
		TableInfo info=Room.Singleton.PokerGame.TableInfo;
		if (info == null)
			return;
 		if(CheckPreBtnAction())
		{ 
			 if(preaction==5)
				preaction=-1;
			else
				preaction=5;
  		} 
		else
		{
			Debug.LogWarning("btn5action");
			if(GetLabelTextOfBtn(5)=="GIFQuanXia")
			{
				Debug.Log("GIFQuanXia");
				//PlayerInfo item=Room.Singleton.PokerGame.TableInfo.GetPlayer(User.Singleton.UserData.NoSeat);
 				TableState.Singleton.Raise(getMaxAllinValue());
				//SoundHelper.PlaySound("Music/Other/Call",AudioSource,0);
				SoundHelper.PlaySound("Music/Other/Allin",AudioSource,0);
  				GameSoundHelper.PlaySound(PlayerBehavior.AllIn, as2, GetSexy((int)User.Singleton.UserData.Avator));
			}
			else
			{
				ShowJiaZhuViewTip(true);
				ChangeSlider();
 			}
			
 		}
  		
		 
	}
 	void btn1action(GameObject btn)
	{
	 	TableInfo info=Room.Singleton.PokerGame.TableInfo;
		if (info == null)
			return;
	 
		if(CheckPreBtnAction())
		{
			if(preaction==1)
				preaction=-1;
			else
				preaction=1;
 		}
		else
		{
 			 ShowJiaZhuViewTip(false);
 			 TableState.Singleton.Fold();
			 SoundHelper.PlaySound("Music/Other/EveryoneFold",AudioSource,0);
			 GameSoundHelper.PlaySound(PlayerBehavior.Fold, as2, GetSexy((int)User.Singleton.UserData.Avator));
		}
	}
 
	long GetCurrentSliderValue(float rawValue)
	{
		TableInfo infor=Room.Singleton.PokerGame.TableInfo;
		if(infor!=null)
		{
			 
			PlayerInfo playerinforitem= infor.GetPlayer(User.Singleton.UserData.NoSeat); 
		
			if(playerinforitem!=null)
			{
		 		double currentValue =  (rawValue*100);
 		 	 
				long max=getMaxAllinValue();
 				
				Debug.Log("infor.HigherBet:"+infor.HigherBet+"playerinforitem.MoneyBetAmnt:"+playerinforitem.MoneyBetAmnt);
				long tempo=0;
				if((2*(infor.HigherBet-playerinforitem.MoneyBetAmnt)+playerinforitem.MoneyBetAmnt)==0)
						tempo=infor.BigBlindAmnt;
				else
						tempo=(2*(infor.HigherBet-playerinforitem.MoneyBetAmnt)+playerinforitem.MoneyBetAmnt);
				
				 tempo= (long)((currentValue*((max-tempo)/100.0f) +tempo)>max ? max:(currentValue*((max-tempo)/100.0f) +tempo));
				
				if(currentValue==0)
				{
					return tempo;
				}
				else if(currentValue==1)
				{
					return (long)(max); 
				}
 				return tempo;
				 
			}
		}
		return 0;
		
	}
	void DisableAllinChipsAnimation(bool iswork)
	{
 		topPanel.transform.FindChild("sliderchip/QuanjinStarAnim").gameObject.SetActiveRecursively(iswork);
		topPanel.transform.FindChild("sliderchip/Quanjin (GIFWhole_pic)").gameObject.SetActiveRecursively(iswork);
   	}
	long getMaxAllinValue()
	{
	    TableInfo info=Room.Singleton.PokerGame.TableInfo;
		if(info!=null)
		{
		 	PlayerInfo playerinforitem= info.GetPlayer(User.Singleton.UserData.NoSeat);
			long max=0;
			
			foreach(PlayerInfo item in info.Players)
			{	
			   	if(max<(item.MoneySafeAmnt+item.MoneyBetAmnt) && item.NoSeat!=User.Singleton.UserData.NoSeat)
				{
					if(item.IsAllIn==true || item.IsPlaying==true)
						max=(item.MoneySafeAmnt+item.MoneyBetAmnt);
				}
			}
			
			if(max>=(playerinforitem.MoneySafeAmnt+playerinforitem.MoneyBetAmnt))
				max=playerinforitem.MoneySafeAmnt;
			else
				max=max-playerinforitem.MoneyBetAmnt;
			
			return max;
		}
		return 0;
	}
	
	void ChangeSlider()
	{
       Transform sliderTrs=topPanel.transform.FindChild("sliderchip");
	    UISlider sld= sliderTrs.GetComponent<UISlider>();
	  
	     DisableAllinChipsAnimation(sld.sliderValue == 1);
	    
	     if(sld.sliderValue != 1)
		 {
	 	 	TheRaise=GetCurrentSliderValue(sld.sliderValue);
		 }
		else
		{
 				TheRaise=getMaxAllinValue();
		}
 
 		Debug.Log("TheRaise:"+TheRaise);
	   	SetJiaZHuLabelVale(TheRaise);
 		
	}
	 
	
	void SetJiaZHuLabelVale(long num)
	{
		 SoundHelper.PlaySound("Music/Other/slider",AudioSource,0);
		
		Transform sliderTrs=topPanel.transform.FindChild("sliderchip");
		Transform Thumbs=sliderTrs.transform.FindChild("Thumb");
		Transform Label=Thumbs.transform.FindChild("Label");
		UILabel uilabl=Label.GetComponent<UILabel>();
		Debug.Log("num"+num);
		if(num<1000)
			uilabl.text="$"+num.ToString();
		else if(num>=1000 && num<1000000)
		{
 			uilabl.text=string.Format("${0:N1}K",(num/1000.0f));//"$"+num.ToString()+"K";
		}
		else
		{
			uilabl.text=string.Format("${0:N1}M",(num/1000000.0f));//"$"+num.ToString()+"K";

		}
 	}
	string GetLabelTextOfBtn(int index)
	{
		Transform childbtn=transform.FindChild("Button_"+index);
		Transform btnText=childbtn.FindChild("btnText");
		UISprite sprite=btnText.GetComponent<UISprite>();
		return sprite.spriteName;
	}
	

	void ChipBeSureBtn(GameObject btn)
	{
		Debug.LogWarning("User.Singleton.Raise:"+TheRaise +" "+getMaxAllinValue());
 		
		if(TheRaise==getMaxAllinValue())
		{
			SoundHelper.PlaySound("Music/Other/Allin",AudioSource,0);
			GameSoundHelper.PlaySound(PlayerBehavior.AllIn, as2, GetSexy((int)User.Singleton.UserData.Avator));
		}
		else
		{
			SoundHelper.PlaySound("Music/Other/Call",AudioSource,0);
		}
		
		TableState.Singleton.Raise(TheRaise);
		TheRaise=-1;
		ShowJiaZHuTip(false);
	} 
	 
	void ShowJiaZHuTip(bool iswork)
	{
		topPanel.transform.FindChild("Button_GreenBtn").gameObject.SetActiveRecursively(iswork);
		topPanel.transform.FindChild("sliderchip").gameObject.SetActiveRecursively(iswork);
  		
		if(iswork==true)
		{
			topPanel.transform.FindChild("sliderchip").GetComponent<UISlider>().sliderValue=0;	
		}
		
		DisAbleRoleBtn(!iswork);
 
	}
	void DisAbleRoleBtn(bool iswork)
	{
		for(int i=1;i<= TableState.Singleton.TotallNumPlayer;i++)
		{
  			 transform.parent.FindChild("RoleInfo_"+i+"/Button_Role").GetComponent<BoxCollider>().enabled=iswork;
			 transform.parent.FindChild("RoleInfo_"+i+"/Button_gift").GetComponent<BoxCollider>().enabled=iswork;
  		}
	}
 
	void DidStartGameEvent()
	{
		preaction=-1;
		PreGenzhu=-1;
		 CheckBtnState(true);
 
   		 
      }
 	void DidEndGameEvent(long taxAnimt)
	{
 		preaction=-1;
		PreGenzhu=-1;
	    CheckBtnState(false);
		
	}
	void UnRegisterEventAction()
	{
 		PhotonClient.StartGameEvent-=DidStartGameEvent; 
		PhotonClient.EndGameEvent-=DidEndGameEvent;
 	    PhotonClient.PlayerTurnBeganEvent-=DidPlayerTurnBeganEvent; 
		//PhotonClient.PlayerTurnEndedEvent-=DidPlayerTurnEndedEvent;
		
		TableState.DoPockerPlayerTurnEnd-=DoPockerPlayerTurnEnd;
   	}
	void RegisterEventAction()
	{
		PhotonClient.StartGameEvent+=DidStartGameEvent; 
		PhotonClient.EndGameEvent+=DidEndGameEvent;

		PhotonClient.PlayerTurnBeganEvent+=DidPlayerTurnBeganEvent; 	
		//PhotonClient.PlayerTurnEndedEvent+=DidPlayerTurnEndedEvent;
				TableState.DoPockerPlayerTurnEnd+=DoPockerPlayerTurnEnd;

   	}
	void OnDestroy()
	{
 		UnRegisterEventAction();
 	}
	void Awake()
	{
	   // DisButtomBtns(false);
		 RegisterEventAction();
		
	}
 
}
