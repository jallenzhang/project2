using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using DataPersist;
using AssemblyCSharp.Helper;
using LilyHeart;

public class WinAndActionTitle : MonoBehaviour {
	
	public float fadeInTime=0.25f;
 	public  float fadeOutTime=0.25f;
	public GameObject Wintitlebg;
	
	
	public  string bestCardValue=string.Empty; 
	 
 	// Use this for initialization
	void Start () {
 	  
	  Util.FadeWidtsOutWithTime(gameObject.transform,0.1f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void Awake()
	{   
		RegisterEventAction();
	}
	
	void SetWintitlebgLabelValue(string str)
	{
 		if(!Util.isFadein(Wintitlebg.transform))	
		//Debug.LogWarning("SetWintitlebgLabelValue "+str);
		Util.FadeWidtsInWithTime(gameObject.transform);
		
 		Util.SetLabelValue(Wintitlebg.transform,str);
 	}
	
	void OnDestroy()
	{
		UnRegisterEventAction();
	}
	void RegisterEventAction()
	{
 		TableState.DoPockerPlayerTurnBegan+=DoPockerPlayerTurnBegan;
		TableState.DoPockerPlayerTurnEnd+=DoPockerPlayerTurnEnd;
		TableState.DopockerBetTurnBegan+=DopockerBetTurnBegan;
 		TableState.DoPockerGameStart+=DoPockerGameStart;
     }
	
	void DoPockerGameStart()
	{
		Debug.Log("WinAndActionTitle --DidStartGameEvent ");
		bestCardValue=string.Empty;
		Util.FadeWidtsOutWithTime(gameObject.transform,0.1f);
	}
	 
	void DoPockerPlayerTurnEnd(int curretNoseat)
	{
		if(User.Singleton.UserData.NoSeat!=-1)
		{
			if(curretNoseat==User.Singleton.UserData.NoSeat)
			{
				Util.FadeWidtsOutWithTime(gameObject.transform);
 			}
		}
	}
	 
	void DoPockerPlayerTurnBegan(int curretNoseat,bool isClient)
	{
		if(User.Singleton.UserData.NoSeat!=-1)
		{
			if(curretNoseat==User.Singleton.UserData.NoSeat)
			{
				TableInfo infor=Room.Singleton.PokerGame.TableInfo;
				
				if(infor!=null)
				{
					PlayerInfo myplayerinfor=infor.GetPlayer(User.Singleton.UserData.NoSeat);
					if(infor.HigherBet>myplayerinfor.MoneyBetAmnt)
					{
 						if(infor.HigherBet<myplayerinfor.MoneySafeAmnt)
						{
							SetWintitlebgLabelValue(LocalizeHelper.Translate("GENZHU")+Util.getLableMoneyK_MModel(infor.HigherBet-myplayerinfor.MoneyBetAmnt));
						}
						else
						{
							 SetWintitlebgLabelValue(LocalizeHelper.Translate("GAIPAIHUOQUANXIA"));
						} 
		
					}
					else
					{
						 SetWintitlebgLabelValue(LocalizeHelper.Translate("GUOPAIHUOJIAZHU"));
					}  
				}
			}
		}
	}
	void DopockerBetTurnBegan()
	{
		TableInfo info=Room.Singleton.PokerGame.TableInfo;
		if(info!=null)
		{
			if(info.Round == TypeRound.Flop)
			{
				SetWintitlebgLabelValue(LocalizeHelper.Translate("FANPAI"));
	  		}
			else if(info.Round==TypeRound.Turn)
			{			
				SetWintitlebgLabelValue(LocalizeHelper.Translate("ZHUAANPAI"));
	      	}
			else if(info.Round==TypeRound.River)
			{			
				SetWintitlebgLabelValue(LocalizeHelper.Translate("HEPAI"));
	 		}  
		}
	}
	void UnRegisterEventAction()
	{
		TableState.DoPockerGameStart-=DoPockerGameStart;
		TableState.DoPockerPlayerTurnBegan-=DoPockerPlayerTurnBegan;
		TableState.DoPockerPlayerTurnEnd-=DoPockerPlayerTurnEnd;
		TableState.DopockerBetTurnBegan-=DopockerBetTurnBegan;
		 
      
   }
	
	 
	 
	public void fadeOutgameobject()
	{
		Debug.LogWarning("fadeOutgameobject");
 		if(!string.IsNullOrEmpty(bestCardValue))
		{
			 SetWintitlebgLabelValue(bestCardValue);
		} 
		else
		{
			if(TableState.Singleton.isPlaying)
				 Util.FadeWidtsOutWithTime(gameObject.transform,fadeOutTime);
		}
	}
	
    
}
