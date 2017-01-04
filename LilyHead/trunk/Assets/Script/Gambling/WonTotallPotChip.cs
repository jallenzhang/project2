using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using System.Collections.Generic;
using DataPersist;
using AssemblyCSharp.Helper;
using LilyHeart;

public class WonTotallPotChip : MonoBehaviour {
	
  
 	long TotalPotAmnt;
	List<ActorInfor> players;
	
	public List<WonPot> QueueWinAnimation
	{
		get;set;
	}
	// Use this for initialization
	void Start () {
  		
		if(QueueWinAnimation==null)
					QueueWinAnimation=new List<WonPot>();
  	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void UnRegisterEventAction()
	{
		TableState.DopockerBetTurnEnd-=DopockerBetTurnEnd;
		TableState.DoPockerGameEnd-=DoPockerGameEnd;
		TableState.DoPockerGameStart-=DoPockerGameStart;
 
 	 	PhotonClient.PlayerWonPotImproveEvent-=DidPlayerWonPotImproveEvent;
 		
     }
   
	void TheTotallChipMoveToWinner(ActorInfor atinfor,long chipiwon,float delayTime,bool[] winners)
	{
		Debug.LogWarning("TheTotallChipMoveToWinner  "+atinfor.NoSeat +"  "+atinfor.gamblingNo +"  "+atinfor.RoleName +" "+atinfor.name);
		Transform role=null;
		if(Util.isMatch())
		{
			role=MatchInforController.Singleton.transform.FindChild(atinfor.RoleName);
		}
		else
		{
			role=ActorInforController.Singleton.transform.FindChild(atinfor.RoleName);

		}
  		if(role!=null)
		{
			if(role.GetComponent<PockerPlayer>().NoSeat==-1|| role.GetComponent<PockerPlayer>().NoSeat != atinfor.NoSeat)
			{
				return;
			}
		}
		
  	 	GameObject blurchip=Resources.Load("prefab/Gambling/Chips") as GameObject;
		GameObject totalchipbgcoln=Instantiate(blurchip.gameObject,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		if(Util.isMatch())
		{
					
			totalchipbgcoln.transform.parent=MatchInforController.Singleton.transform.FindChild(atinfor.RoleName);

		}
		else
		{
			totalchipbgcoln.transform.parent=ActorInforController.Singleton.transform.FindChild(atinfor.RoleName);
		}
		
		totalchipbgcoln.transform.localScale=new Vector3(1,1,1);
		totalchipbgcoln.transform.localPosition=new Vector3(0,0,-20);
		totalchipbgcoln.name="totalchipbgcoln"+atinfor.RoleName;
		totalchipbgcoln.GetComponent<SpriteChip>().OnInit(chipiwon,atinfor,atinfor.NoSeat,atinfor.gamblingNo,SpriteChip.ChipType.WonPotChip,delayTime,winners);
	 
  	}
	 
	void DidPlayerWonPotImproveEvent(int potId,int[] winner,long[] winamnt,int[] attachedplayer)
	{
//		Debug.LogWarning("potId:"+potId+" winner:"+winner+" winamnt:"+winamnt+" attachedplayer:"+attachedplayer);
// 		for(int i=0;i<winner.Length;i++)
//		{
//			Debug.Log("winner:"+winner[i]);
//		}
//		for(int i=0;i<winamnt.Length;i++)
//		{
//			Debug.Log("winner:"+winamnt[i]);
// 		}
//		for(int i=0;i<attachedplayer.Length;i++)
//		{
//			Debug.Log("winner:"+attachedplayer[i]);
// 		}
		if(potId==0)
		{
			for(int i=0;i<winner.Length;i++)
			{
				if(User.Singleton.UserData.NoSeat==winner[i])
				{
					
					if(winamnt[i]>User.Singleton.UserData.BiggestWin)
					{
						ActorInforController.BreakBiggestWonPot=true;
					}
					break;
				}
			}
		}
  		WonPot apot=new WonPot();
		apot.potId=potId;
		apot.winner=winner;
		apot.winamnt=winamnt;
		apot.attachedplayer=attachedplayer;
 		//potlist.Add(apot);
		Debug.Log("apot.potId:"+apot.potId);
		
		
		QueueWinAnimation.Add(apot);
	}
	void DoPockerGameStart()
	{
		QueueWinAnimation.Clear();
  		Util.SetLabelValue(transform,"wonpottotalchip",string.Empty);
		transform.FindChild("spritechip").GetComponent<UISprite>().spriteName=Util.GetChipSpriteByChip(0);
		TotalPotAmnt=0;
		if(players!=null)
			players.Clear();
		
	}
	void DestoryAnmationTips(string name)
	{
	    Transform[] trs=null;
	    if(Util.isMatch())
		{
			trs=MatchInforController.Singleton.transform.GetComponentsInChildren<Transform>();
		}
		else
		{
 			trs=ActorInforController.Singleton.transform.GetComponentsInChildren<Transform>();
		} 
 		foreach(Transform tr in trs)
		{
			if(tr.name==name)
			{
				Destroy(tr.gameObject);
			}
		}
 	}
	 

	
	ActorInfor GetActorinfor(int No)
	{
		foreach(ActorInfor ac in players)
		{
			if(ac.NoSeat==No)
				return ac;
		}
		
		return null;
		
	}
	 
	
	void WonPotMoveToWinner()
	{
		Debug.LogWarning("WonPotMoveToWinner");
 
		if(players!=null)
		{
		    
			TableInfo info=Room.Singleton.PokerGame.TableInfo;
			if(info!=null)
			{
			
				float totallTime=0;
				foreach(WonPot wonpot in QueueWinAnimation)
				{
					bool[] cards=null;
 					if(info.Cards[0].ToString()!="--" && info.Cards[3].ToString()!="--" && info.Cards[4].ToString()!="--")
					{
				    	cards=Room.Singleton.PokerGame.GetBestFiveCards(info.Cards,wonpot.winner);
					}
					for(int i=0;i<wonpot.winner.Length;i++)
					{
						ActorInfor acinfor=GetActorinfor(wonpot.winner[i]);
		 				if(acinfor !=null && acinfor.NoSeat!=-1)
		 				{
		 
								TotalPotAmnt=TotalPotAmnt-wonpot.winamnt[i];
		 				        if(TotalPotAmnt<10)
									TotalPotAmnt=0;
						
								StartCoroutine(setWonPotTotallChip(totallTime,TotalPotAmnt));
	 							TheTotallChipMoveToWinner(acinfor,wonpot.winamnt[i],totallTime,cards);
	  					}
					}
						totallTime+=2.25f;
					
					
				}
				
				totallTime+=2.25f;
				
				StartCoroutine(setWonPotTotallChip(totallTime,0));
		 
				QueueWinAnimation.Clear();
			}
		}
		
  
	
	}
	
	IEnumerator setWonPotTotallChip(float time, long  tatallpot)
	{
		yield return new WaitForSeconds(time);
		
 		Util.SetLabelValue(transform,"wonpottotalchip", Util.getLableMoneyK_MModel(tatallpot));
// 		if(TotalPotAmnt==0)
//			TableState.Singleton.DoWonAimation=false;
 	}
	
	 
	
	IEnumerator SubTaxAnmit(float time, long  taxAnimt)
	{
		  yield return new WaitForSeconds(time);
		 
			TotalPotAmnt=TotalPotAmnt-taxAnimt;
			if(TotalPotAmnt<0)
				TotalPotAmnt=0;
		
			Util.SetLabelValue(transform,"wonpottotalchip", Util.getLableMoneyK_MModel(TotalPotAmnt));
			Invoke("WonPotMoveToWinner",1.0f);
 
	}
	
	void DoPockerGameEnd(long taxAnimt,List<ActorInfor> plays)
	{
		players=new List<ActorInfor>(plays);
 		
		TableInfo info=Room.Singleton.PokerGame.TableInfo;
		if(info!=null)
		{
			TotalPotAmnt=info.TotalPotAmnt;
			Util.SetLabelValue(transform,"wonpottotalchip",Util.getLableMoneyK_MModel(TotalPotAmnt));
			StartCoroutine(SubTaxAnmit(1.05f,taxAnimt));
 		}
 	}
	
	IEnumerator SetPockerTotallPot(float tim)
	{
		yield return new WaitForSeconds(tim);
		// Debug.LogWarning(totallPot +"  "+Util.getLableMoneyK_MModel(totallPot));
		Util.SetLabelValue(transform,"wonpottotalchip",Util.getLableMoneyK_MModel(TotalPotAmnt));
		transform.FindChild("spritechip").GetComponent<UISprite>().spriteName=Util.GetChipSpriteByChip(TotalPotAmnt);
 	 }
	
	void DopockerBetTurnEnd()
	{
		TableInfo info=Room.Singleton.PokerGame.TableInfo;
		if(info!=null)
		{
			TotalPotAmnt=info.TotalPotAmnt;
 		 	StartCoroutine(SetPockerTotallPot(1.15f));
		}
	}
	void RegisterEventAction()
	{
	   	TableState.DopockerBetTurnEnd+=DopockerBetTurnEnd;

		PhotonClient.PlayerWonPotImproveEvent +=DidPlayerWonPotImproveEvent;
		TableState.DoPockerGameStart+=DoPockerGameStart;

 		TableState.DoPockerGameEnd+=DoPockerGameEnd;

   	}
	void OnDestroy()
	{
 		UnRegisterEventAction();
 	}
	void Awake()
	{
		RegisterEventAction();
	}
	
	
   
	
}
