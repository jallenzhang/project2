using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using System.Collections.Generic;
using DataPersist;
using System;
using System.Linq;
using LilyHeart;

 
namespace AssemblyCSharp
{
		public class TableState {
		
		public enum TableInfoTypeState
		{
			Init,
	        DidJoinGame,
			DidGameStart,
			ShowHandcard,
			PlayerTurnBegin,
			PlayerTurnEnd,
			ShowPublicCard,
			DidPlayerJoin,
			DidPlayerLeave,
			DidBetTurnBegin,
			DIdBetTurnEnd,
			DidStandUp,
			ChipToPot, 
			DistributeMoney,
			PlayerWonPot,
			DidGameEnd
		}
		
		public enum GameTableType
		{
			GameTableType_Normal=0,
			GameTableType_Match,
		}
		
		public int TotallNumPlayer=9;
		public int CurrentNoSeat=-1;
		
		public bool isPlaying=false;
		
		public bool DoWonAimation=false;
		
 		
		private static TableState singleton;
		
		public List<ActorInfor> players{get;set;}
		private Queue<object> Notifications {get;set;}
		
		public bool SendinghandCard=false;
 		public bool playingAnimation=false;
		
		public GameTableType gametabletype=GameTableType.GameTableType_Normal;
		
	    public static event Action<ActorInfor> DoPockerJoinGame;  	
		public static event Action<ActorInfor> DoPockerGameStartSetRoleInfor;
	    public static event Action<ActorInfor,bool,PlayerLeaveType> DoPckerPlayerLeave; 
		
		public static event Action<ActorInfor> DoPockerPlayerJoin;
		
		public static event Action<int,bool> DoPockerPlayerTurnBegan;
		public static event Action<int> DoPockerPlayerTurnEnd;
		
		public static event Action DopockerBetTurnEnd;
		public static event Action DopockerBetTurnBegan;
		
		public static event Action<long,List<ActorInfor>> DoPockerGameEnd; 
		
		public static event Action  DoPockerGameStart;
		
        public static event Action<int>  DoPockerGameStandUp; 
		
		public static event Action<ActorInfor> DoPockerGameSitDown;
		
		public static event Action DoPockerGameStateReSet;
		public static event Action DoPockerGameNoseatReSet; 
		
		public static event Action<List<ActorInfor>> DoPockerPlayerHoleCardsChangedEvent;
		
		public static event Action<int,string> DoPlayersShowCardsFinished;
 	 
		public static event Action<TypeState> DoActorControllerGameSitDown;
		
		public static event Action<ActorInfor,ActorInfor,int,List<ActorInfor>> DoPockerSendGiftEvent;
		
		public TableState()
		{
			Debug.Log("Table State");
			players=new List<ActorInfor>();
			Notifications=new Queue<object>();
			RegisterEventAction();
		}
		
		
		void AddNotifications(object item)
		{
			Notifications.Enqueue(item);
			
			if(Notifications.Count>0)
				CheckNotifications();
		}
		
		public static TableState Singleton{
			
			get {
				
				if(singleton==null)
				{
					singleton=new TableState();
				}
				return singleton;
			}
		}
 		  
	 	public TableInfoTypeState  tableInfotypeState = TableInfoTypeState.Init;
	    
		public void Standup()
		{
			User.Singleton.StandUp();
			if(User.Singleton.UserData.NoSeat==-1)
			{
				if(DoPockerGameStandUp!=null)
				{
					DoPockerGameStandUp(-1);
				}
			}
		}
		void PockerGameStart()
		{
		    isPlaying=true;
			DoWonAimation=false;
			
			 
			if(DoPockerGameNoseatReSet!=null)
			{
				DoPockerGameNoseatReSet();
			}
			
			//Debug.LogWarning(players.Count);
 			foreach(ActorInfor ac in players)
 			{
//				Debug.LogError(ac.name+" "+ac.NoSeat);
 				if(DoPockerGameStartSetRoleInfor!=null)
 				{
					DoPockerGameStartSetRoleInfor(ac);
 				}
 			}
  			if(DoPockerGameStart!=null)
			{
				DoPockerGameStart();
			}
			
		}
		
		ActorInfor initOneActorInfor(PlayerInfo  playinfo)
		{
			if(CurrentNoSeat==-1)
				Debug.LogError("CurrentNoSeat "+CurrentNoSeat);
			
			ActorInfor item=new ActorInfor();
 			int index;
			if(playinfo.NoSeat-CurrentNoSeat>=0)
			{
				index = playinfo.NoSeat-CurrentNoSeat+1;
			}
			else
				index = playinfo.NoSeat-CurrentNoSeat+TotallNumPlayer+1;
			
		//	Debug.LogWarning(index+ "  " + playinfo.NoSeat + " playinfo.Name: " + playinfo.Name);
 			item.gamblingNo=index;
			
			item.RoleName="RoleInfo_"+index;
			item.nextOne=index+1;
  			item.NoSeat=playinfo.NoSeat;
			
			item.name=playinfo.Name;
			
			if(index==TotallNumPlayer)
				item.nextOne=1;
		    if(index==1)
				item.bigCard=true;
			
			if(playinfo.Avator==(byte)PlayerAvator.DaHeng)
			{
			     item.EasyMotionName="ArbTy";
			}
			else if(playinfo.Avator==(byte)PlayerAvator.Dalaopo)
			{
			     item.EasyMotionName="RusWif";
			}
			else if(playinfo.Avator==(byte)PlayerAvator.European)
			{
			     item.EasyMotionName="EurPrin";
			}
			else if(playinfo.Avator==(byte)PlayerAvator.Luoli)
			{
			     item.EasyMotionName="JapLoli";
			}
			else if(playinfo.Avator==(byte)PlayerAvator.Songer)
			{
			     item.EasyMotionName="Blasta";
			}
			else if(playinfo.Avator==(byte)PlayerAvator.Yitaitai)
			{
			     item.EasyMotionName="Song";
			}
			else if(playinfo.Avator==(byte)PlayerAvator.Qianjing)
			{
			     item.EasyMotionName="MaGir";
	 		}
			else if(playinfo.Avator==(byte)PlayerAvator.Captain)
			{
			     item.EasyMotionName="Pirate";
			}
			
			else if(playinfo.Avator==(byte)PlayerAvator.Guest)
			{
			   item.EasyMotionName="Guest";
			} 
			
 			return item;
		}
		
		void DelogListPlayerInfo()
		{
			foreach(ActorInfor ac in players)
			{
				Debug.Log(ac.RoleName +" "+ac.NoSeat +" "+ac.gamblingNo);
				
			}
		}
		void PockerJoinGame()
		{
			if(DoPockerGameStateReSet!=null)
			{
				DoPockerGameStateReSet();
			}
			Debug.LogWarning("PockerJoinGame  "+User.Singleton.UserData.NoSeat);
			
			players.Clear();
			TableInfo info=Room.Singleton.PokerGame.TableInfo;
			if(info!=null)
			{
				if(info.Players.Count>2)
				{
					isPlaying=true;
				}
			}
			if(info != null  && User.Singleton.UserData.NoSeat != -1)
			{
 				List<PlayerInfo> temp=new List<PlayerInfo>();
				temp.AddRange(info.Players.FindAll(rs=>rs.NoSeat<User.Singleton.UserData.NoSeat));
				temp.AddRange(info.Players.FindAll(rs=>rs.NoSeat>=User.Singleton.UserData.NoSeat));
				
				foreach(PlayerInfo playinfo in temp)
				{
					ActorInfor ac=initOneActorInfor(playinfo);
					if(!players.Contains(ac))
					{
						players.Add(ac); 
						if(DoPockerJoinGame!=null)
						{
							DoPockerJoinGame(ac);
						}
					}
					
				}
		 
   			}
			
			
		}
		
		void pockerPlayerLeave(int NoSeat,bool isKlick,PlayerLeaveType levettype)
		{
  			ActorInfor  tempAc=null;
			foreach(ActorInfor ac in players)
			{
				//Debug.LogWarning(ac.RoleName+" "+ac.NoSeat);
				if(ac.NoSeat==NoSeat)
				{
					tempAc=ac;
					break;
					
				}
			}
			if(tempAc!=null)
			{
				players.Remove(tempAc);
				
				if(DoWonAimation==false || (NoSeat == User.Singleton.UserData.NoSeat && isKlick==true))
				{
					if(DoPckerPlayerLeave!=null && tempAc!=null)
					{
						if(DoPckerPlayerLeave!=null)
							DoPckerPlayerLeave(tempAc,isKlick,levettype);
					}
				}
			}
			
		}
		
		void pockerPlayerJoin(int Noseat)
		{
			//Debug.LogWarning("pockerPlayerJoin"+Noseat +"  "+User.Singleton.UserData.NoSeat);
			if(CurrentNoSeat==-1)
				return;
			
			if(Noseat==User.Singleton.UserData.NoSeat)
				return;
  			
			TableInfo info=Room.Singleton.PokerGame.TableInfo;
			if(info != null)
			{
				PlayerInfo playerInfo=info.GetPlayer(Noseat);
				if(playerInfo!=null)
				{
					ActorInfor tempAc=initOneActorInfor(playerInfo);
					bool flag=true;
 					foreach(ActorInfor ac in players)
					{
 						if(ac.NoSeat==tempAc.NoSeat)
						{
							flag=false;
							break;
							
						}
					}
					if(flag==true)
					{
						players.Add(tempAc);
 						if(DoWonAimation==false)
						{
	 						if(DoPockerPlayerJoin!=null)
								DoPockerPlayerJoin(tempAc);
						}
 					}
 				}
			}
		}
		
		void pockerPlayerTurnBegan(int Noseat,bool isClient)
		{
			if(DoPockerPlayerTurnBegan!=null)
				DoPockerPlayerTurnBegan(Noseat,isClient);
		}
		public void Fold()
		{
//			if(DoPockerPlayerTurnEnd!=null)
//			{
//				TableInfo info=Room.Singleton.PokerGame.TableInfo;
//				if(info!=null && User.Singleton.UserData.NoSeat!=-1)
//				{
//					PlayerInfo nextPlayer=info.GetPlayingPlayerNextTo(User.Singleton.UserData.NoSeat);
//					info.NoSeatCurrPlayer=nextPlayer.NoSeat;
//					Room.Singleton.PokerGame.PlayerActionNames[User.Singleton.UserData.NoSeat]=PhotonClient.ACTION_FOLD;
//					DoPockerPlayerTurnEnd(User.Singleton.UserData.NoSeat);
//				}
//			}
			User.Singleton.Fold();
		 
		}
		public void Call()
		{
			
//			if(DoPockerPlayerTurnEnd!=null)
//			{
//				TableInfo info=Room.Singleton.PokerGame.TableInfo;
//				if(info!=null && User.Singleton.UserData.NoSeat!=-1)
//				{
//					PlayerInfo nextPlayer=info.GetPlayingPlayerNextTo(User.Singleton.UserData.NoSeat);
//					info.NoSeatCurrPlayer=nextPlayer.NoSeat;
//					PlayerInfo myitem=info.GetPlayer(User.Singleton.UserData.NoSeat);
//
//  					myitem.MoneyBetAmnt=info.HigherBet;
//					myitem.MoneySafeAmnt=myitem.MoneySafeAmnt-info.HigherBet;
//					
//					DoPockerPlayerTurnEnd(User.Singleton.UserData.NoSeat);
//				}
//			}
			User.Singleton.Call();
		}
		public void Raise(long chip)
		{
//			if(DoPockerPlayerTurnEnd!=null)
//			{
//				TableInfo info=Room.Singleton.PokerGame.TableInfo;
//				if(info!=null && User.Singleton.UserData.NoSeat!=-1)
//				{
//					PlayerInfo nextPlayer=info.GetPlayingPlayerNextTo(User.Singleton.UserData.NoSeat);
//					PlayerInfo myitem=info.GetPlayer(User.Singleton.UserData.NoSeat);
//					
//					if(chip>=myitem.MoneySafeAmnt)
//					{
//						Room.Singleton.PokerGame.PlayerActionNames[User.Singleton.UserData.NoSeat]=PhotonClient.ACTION_ALLIN;
//						myitem.IsAllIn=true;
//						myitem.IsPlaying=false;
//						myitem.MoneyBetAmnt=myitem.MoneyBetAmnt+myitem.MoneySafeAmnt;
//						myitem.MoneySafeAmnt=0;
//					}
//					else
//					{
// 						myitem.MoneyBetAmnt=myitem.MoneyBetAmnt+chip;
//						myitem.MoneySafeAmnt=myitem.MoneySafeAmnt-myitem.MoneyBetAmnt;
//					}
//					 
//					info.NoSeatCurrPlayer=nextPlayer.NoSeat;
// 					DoPockerPlayerTurnEnd(User.Singleton.UserData.NoSeat);
//				}
//			}
			User.Singleton.Raise(chip);
 			 
		}
		void pockerPlayerTurnEnd(int Noseat)
		{
 			 if(DoPockerPlayerTurnEnd!=null )
			{
//				if(Noseat!= User.Singleton.UserData.NoSeat )
//				{
					DoPockerPlayerTurnEnd(Noseat);
 				//}
//				else{
//					if(Room.Singleton.PokerGame.PlayerActionNames[Noseat]==PhotonClient.ACTION_FOLD)
//					{
//					   DoPockerPlayerTurnEnd(Noseat);
//					} 
//				}
			}
			
		}
		void pockerBetTurnEnd()
		{
			if(DopockerBetTurnEnd!=null)
			{
				DopockerBetTurnEnd();
			}
		}
		void pockerBetTurnBegan()
		{
 			if(DopockerBetTurnBegan!=null)
			{
				DopockerBetTurnBegan();
			}
		}
		void PockerGameEnd(long tax)
		{
			isPlaying=false;
			DoWonAimation=true;
 			if(DoPockerGameEnd!=null)
			{
				DoPockerGameEnd(tax,players);
			}
		}
		void PockerGameStandUp(int Noseat)
		{
			if(DoPockerGameStandUp !=null)
			{
				DoPockerGameStandUp(Noseat);
			}
		}
		
		ActorInfor initOneActorIfnfor()
		{
			 
			
			ActorInfor item=new ActorInfor();
 			int index;
			if(2-CurrentNoSeat>=0)
			{
				index = 2-CurrentNoSeat+1;
			}
			else
				index = 3-CurrentNoSeat+TotallNumPlayer;
			
		//	Debug.LogWarning(index+ "  " + playinfo.NoSeat + " playinfo.Name: " + playinfo.Name);
 			item.gamblingNo=index;
			
			item.RoleName="RoleInfo_"+index;
			item.nextOne=index+1;
  			item.NoSeat=2;
			item.name="jianghuayang";
			
			return item;
		}
 		void PlayerHoleCardsChanged()
		{
  			TableInfo info=Room.Singleton.PokerGame.TableInfo;
			if(info!=null)
			{
				List<ActorInfor> temp=new List<ActorInfor>();
				
				players=players.Distinct(new ActorInforCompare()).ToList();
			//	temp.Add(initOneActorIfnfor	());
				
				temp.AddRange(players.FindAll(rs=>rs.NoSeat>=info.NoSeatSmallBlind));
				temp.AddRange(players.FindAll(rs=>rs.NoSeat<info.NoSeatSmallBlind));
				
				List<ActorInfor> temp2=new List<ActorInfor>();
				foreach(ActorInfor ac in temp)
				{
 					PlayerInfo item = info.GetPlayer(ac.NoSeat);
					if(item!=null)
					{
						if(item.IsPlaying==false && item.IsAllIn==false)
							temp2.Add(ac);
					}
					else
					{
						temp2.Add(ac);
					}
				}
				
				if(temp2.Count>0)
				{
					foreach(ActorInfor ac in temp2)
					{
						temp.Remove(ac);
					}
				}
				
				

	//temp.Remove(temp2);// (temp2);
				int gg=0;
				foreach(ActorInfor item in temp)
				{
					Debug.LogWarning(item.name +" : "+item.NoSeat +" "+item.RoleName +" "+item.gamblingNo+"  "+gg++);
				}
			
				
				if(DoPockerPlayerHoleCardsChangedEvent!=null)
				{
					DoPockerPlayerHoleCardsChangedEvent(temp);
	 			}
			}
			
			 
 		}
		void PlayersShowCardsFinished(Dictionary<int,string> plays)
		{
			
			List<int> keys = plays.Keys.ToList();
		    foreach(int key in keys)
			{
				string  cardvalues=null;
				plays.TryGetValue(key,out cardvalues);
				//Debug.Log("Key :"+key);
				//Debug.Log("cardvalues "+cardvalues);
				//string[] cards=cardvalues.Split(' ');
				
				if(DoPlayersShowCardsFinished !=null)
				{
					DoPlayersShowCardsFinished(key,cardvalues);
				}
			}
			
 		}
		ActorInfor getActorByNoSeat(int noseat)
		{
			if(players.Count>0)
			{
				foreach(ActorInfor ac in players)
				{
					if(ac.NoSeat==noseat)
						return ac;
				}
				
			}
			return null;
		}
		void playerSendGift(int sendnoseat,int receivenoseat,int ID)
		{
	 
			ActorInfor sendActor=getActorByNoSeat(sendnoseat);
			ActorInfor receiveActor=getActorByNoSeat(receivenoseat);
  			if(DoPockerSendGiftEvent!=null)
			{
				DoPockerSendGiftEvent(sendActor,receiveActor,ID,players);
			}
		 
		}
		void PockerGameSitDown(TypeState gametype)
		{
			Debug.LogWarning("PockerGameSitDown");
			DoWonAimation=false;

			if(DoPockerGameStateReSet!=null)
			{
				DoPockerGameStateReSet();
			}
 			
			CurrentNoSeat=User.Singleton.UserData.NoSeat;
			
			players.Clear();
			TableInfo info=Room.Singleton.PokerGame.TableInfo;
			if(info != null  && User.Singleton.UserData.NoSeat != -1)
			{
 				List<PlayerInfo> temp=new List<PlayerInfo>();
				temp.AddRange(info.Players.FindAll(rs=>rs.NoSeat<User.Singleton.UserData.NoSeat));
				temp.AddRange(info.Players.FindAll(rs=>rs.NoSeat>=User.Singleton.UserData.NoSeat));
				
				foreach(PlayerInfo playinfo in temp)
				{
					ActorInfor ac=initOneActorInfor(playinfo);
					if(!players.Contains(ac))
					{
						players.Add(ac); 
						if(DoPockerGameSitDown!=null)
						{
							DoPockerGameSitDown(ac);
						}
					}
					
				}
				
				if(DoActorControllerGameSitDown!=null)
				{
					DoActorControllerGameSitDown(gametype);
				}
    		}
 		}
		void PlayerWonPotImproveEvent(int potId,int[] winner,long[] winamnt,int[] attachedplayer)
		{
			
			//WonPot apot=new WonPot(potId,winner,winamnt,attachedplayer);
  			
		}
 		void CheckNotifications()
		{
		    object  notification = Notifications.Dequeue() as object;
			if(notification is BetTurnEndedNotification)
			{
				tableInfotypeState=TableInfoTypeState.DIdBetTurnEnd;
				pockerBetTurnEnd();
			}
			else if(notification is BetTurnStartedNotification)
			{
				tableInfotypeState=TableInfoTypeState.DidBetTurnBegin;
				pockerBetTurnBegan();
			}
			else if(notification is  GameStartedNotification)
			{
				tableInfotypeState=TableInfoTypeState.DidGameStart;
				
				PockerGameStart();
			}
			else if(notification is PlayerTurnBeganNotification)
			{
				tableInfotypeState=TableInfoTypeState.PlayerTurnBegin;
				pockerPlayerTurnBegan((notification as PlayerTurnBeganNotification).NoSeat,(notification as PlayerTurnBeganNotification).IsClient);
			}
			else if(notification is PlayerTurnEndedNotification)
			{
				tableInfotypeState=TableInfoTypeState.PlayerTurnEnd;
				pockerPlayerTurnEnd((notification as PlayerTurnEndedNotification).NoSeat);

			}
			else if(notification is GameStartedNotification)
			{
				tableInfotypeState=TableInfoTypeState.DidGameStart;
			}
			else if(notification is PlayerWonPotNotification)
			{
				tableInfotypeState=TableInfoTypeState.PlayerWonPot; 
			}
			else if(notification is PlayerLeavedNotification)
			{
				tableInfotypeState=TableInfoTypeState.DidPlayerLeave;
 				pockerPlayerLeave((notification as PlayerLeavedNotification).NoSeat,(notification as PlayerLeavedNotification).click,(notification as PlayerLeavedNotification).gameleaveType);
			}
			else if(notification is PlayJoinedNotification)
			{
				tableInfotypeState=TableInfoTypeState.DidPlayerJoin;
				pockerPlayerJoin((notification as PlayJoinedNotification).NoSeat);
			}
			else if(notification is JoinGameNotification)
			{
				PockerJoinGame();
			}
			else if(notification is GameEndNotification)
			{
				PockerGameEnd((notification as GameEndNotification).taxAnimt);
			}
			else if(notification is StandUpNotification)
			{
				PockerGameStandUp((notification as StandUpNotification).Noseat);
			}
			else if(notification is GameSitDownNotification)
			{
				PockerGameSitDown((notification as GameSitDownNotification).gameType);
			}
			else if(notification is PlayerHoleCardsChangedNotification)
			{
 				 
 					PlayerHoleCardsChanged();
				 
			}
			else if(notification is PlayersShowCardsFinishedNotification)
			{
				PlayersShowCardsFinished((notification as PlayersShowCardsFinishedNotification).plays);
			}
			else if(notification is PlayerWonPotImproveNotification)
			{
				PlayerWonPotImproveNotification  WonPot = (notification as PlayerWonPotImproveNotification); 
				PlayerWonPotImproveEvent(WonPot.potId,WonPot.winner,WonPot.winamnt,WonPot.attachedplayer);
			}
			else if(notification is  SendGiftNotification)
			{
				playerSendGift((notification as SendGiftNotification).SenderNoSeat,(notification as SendGiftNotification).ReceiverNoSeat,(notification as SendGiftNotification).GiftId);
			}
			
			Debug.LogWarning(notification);
			
			
		}
		public void CheckInTableInfoState(GameTableType type)
		{
			gametabletype=type;
			if(gametabletype==GameTableType.GameTableType_Normal)
				TotallNumPlayer=9;
			else
				TotallNumPlayer=5;
			CheckInTableInfoState();
		}
		public void CheckInTableInfoState()
		{
			isPlaying=true;
			DoWonAimation=false;
 			players.Clear();
			Notifications.Clear();
			
			
			
		}
		void DidStartGameEvent ()
		{
			AddNotifications(new GameStartedNotification());
 		}
		 
		void DidPlayerWonPotEvent(int potId, long wonmoney,int noseat)
		{
			
		}
		void DidJoinGameFinished(bool iswrok,TypeState state)
		{
			CurrentNoSeat=User.Singleton.UserData.NoSeat;
			AddNotifications(new JoinGameNotification());
		}
		
		void DidEndGameEvent(long taxAnimt)
		{
 			AddNotifications(new GameEndNotification(taxAnimt));

		}
		void DidPlayerTurnBeganEvent(int curretNoseat,bool isClient)
		{
			AddNotifications(new PlayerTurnBeganNotification(curretNoseat,isClient));
		}
		
		void DidPlayerTurnEndedEvent(int noseat,bool isClient)
		{
			AddNotifications(new PlayerTurnEndedNotification(noseat,isClient));
		}
		void DidPlayerJoinedEvent(int noseat)
		{
		   AddNotifications(new PlayJoinedNotification(noseat));
		}
		void DidPlayerLeavedEvent(int noseat,bool isKick,PlayerLeaveType leavetype)
		{
			AddNotifications(new PlayerLeavedNotification(noseat,isKick,leavetype));
		}
		void DidBetTurnEndedEvent()
		{
			AddNotifications(new BetTurnEndedNotification());
		}
		void DidBetTurnStartEvent()
		{
			AddNotifications(new BetTurnStartedNotification());
		}
 		void DidToPlayerHoleCardsChangedEvent(PlayerInfo playinfo)
		{
 			AddNotifications(new PlayerHoleCardsChangedNotification());
			//Debug.LogError("AddNotifications");
		}
		void DidBroadcastMessageInTableEvent(int nosear,string message)
		{
		}
		void DidSitDownEvent(TypeState gamestate)
		{
			AddNotifications(new GameSitDownNotification(gamestate));
		}
		
		void DidLevelUpEvent(int level)
		{
		}
		
		void DidPlayerWonPotImproveEvent(int potId,int[] winner,long[] winamnt,int[] attachedplayer)
		{
			AddNotifications(new PlayerWonPotImproveNotification(potId,winner,winamnt,attachedplayer));
 		}
		
					

		void DidSendGiftEvent(int sendnoseat,int receivenoseat,int ID)
		{
			AddNotifications(new SendGiftNotification(sendnoseat,receivenoseat,ID));
		}
		void DidRegisterOrUpgradeEvent(bool iswork)
		{
			
		}
		
		void ReceiveSameAccountLoginEvent()
		{
			
		}
		void DidPlayersShowCardsFinished(Dictionary<int,string> playes)
		{
			AddNotifications(new PlayersShowCardsFinishedNotification(playes));
		}
		void DidSyncGameDataTableInfoEvent(bool iswork)
		{
			
		}
		void StartAdditionalBehaviour()
		{
			
		}
		
		void DidLeaveGameFinished()
		{
			
		}
		
		void GotoRoom()
		{
		}
		
		void PopupMaskingTable()
		{
			
		}
 
		void DidStandUpEvent(int noseat)
		{
		    AddNotifications(new StandUpNotification(noseat));
		} 
	 	void RegisterEventAction()
		{
			PhotonClient.StartGameEvent+=DidStartGameEvent;
			PhotonClient.EndGameEvent+=DidEndGameEvent;
			PhotonClient.JoinGameFinished+=DidJoinGameFinished;
			
			PhotonClient.PlayerWonPotEvent+=DidPlayerWonPotEvent;
			PhotonClient.PlayerTurnBeganEvent+=DidPlayerTurnBeganEvent;
	        PhotonClient.PlayerTurnEndedEvent+=DidPlayerTurnEndedEvent;
			PhotonClient.PlayerJoinedEvent+=DidPlayerJoinedEvent;
			PhotonClient.PlayerLeavedEvent+=DidPlayerLeavedEvent;
			
			PhotonClient.BetTurnEndedEvent+=DidBetTurnEndedEvent;
			PhotonClient.BetTurnStartEvent+=DidBetTurnStartEvent;
			PhotonClient.ToPlayerHoleCardsChangedEvent+=DidToPlayerHoleCardsChangedEvent;
			
			PhotonClient.BroadcastMessageInTableEvent+=DidBroadcastMessageInTableEvent;
			//PhotonClient.QueryUserEvent+=DidQueryUserEvent;
			
			PhotonClient.SitDownEvent+=DidSitDownEvent;
			PhotonClient.LevelUpEvent+=DidLevelUpEvent;
			
			PhotonClient.PlayerWonPotImproveEvent+=DidPlayerWonPotImproveEvent;
			PhotonClient.SendGiftEvent+=DidSendGiftEvent;
			
			PhotonClient.RegisterOrUpgradeEvent+=DidRegisterOrUpgradeEvent;
			PhotonClient.SameAccountLoginEvent+=ReceiveSameAccountLoginEvent;
			
			PhotonClient.DidPlayersShowCardsFinished+=DidPlayersShowCardsFinished;
			
			PhotonClient.SyncGameDataTableInfoEvent+=DidSyncGameDataTableInfoEvent;
			UtilityHelper.MaskTableAdditionalBehaviourEvent += StartAdditionalBehaviour;
			PhotonClient.LeaveGameFinished+=DidLeaveGameFinished;
			PhotonClient.GotoBackgroundSceneEvent += GotoRoom;
			PhotonClient.MaskingTablePopUPEvent +=PopupMaskingTable;
			
			PhotonClient.StandUpEvent+=DidStandUpEvent;
	 	}
		public void OutRegisterEventAction()
		{
			PhotonClient.EndGameEvent-=DidEndGameEvent;
			PhotonClient.StartGameEvent-=DidStartGameEvent;
			PhotonClient.JoinGameFinished-=DidJoinGameFinished;

			
			PhotonClient.PlayerWonPotEvent-=DidPlayerWonPotEvent;
			PhotonClient.PlayerTurnBeganEvent-=DidPlayerTurnBeganEvent;
	        PhotonClient.PlayerTurnEndedEvent-=DidPlayerTurnEndedEvent;
			PhotonClient.PlayerJoinedEvent-=DidPlayerJoinedEvent;
			PhotonClient.PlayerLeavedEvent-=DidPlayerLeavedEvent;
			
			PhotonClient.BetTurnEndedEvent-=DidBetTurnEndedEvent;
			PhotonClient.BetTurnStartEvent-=DidBetTurnStartEvent;
			
			PhotonClient.ToPlayerHoleCardsChangedEvent-=DidToPlayerHoleCardsChangedEvent;
					
			PhotonClient.BroadcastMessageInTableEvent-=DidBroadcastMessageInTableEvent;
			//PhotonClient.QueryUserEvent-=DidQueryUserEvent;
			PhotonClient.SitDownEvent-=DidSitDownEvent;
			PhotonClient.LevelUpEvent-=DidLevelUpEvent;
			
			PhotonClient.PlayerWonPotImproveEvent-=DidPlayerWonPotImproveEvent;
			PhotonClient.SendGiftEvent-=DidSendGiftEvent;
			PhotonClient.GotoBackgroundSceneEvent -= GotoRoom;
			PhotonClient.RegisterOrUpgradeEvent-=DidRegisterOrUpgradeEvent;
			PhotonClient.SameAccountLoginEvent-=ReceiveSameAccountLoginEvent;
			//PhotonClient.JoinGameFinished-=DidJointGame;
		   //PhotonClient.JoinGameEventFinished-=DidJoinGameEventFinished;
	
			PhotonClient.DidPlayersShowCardsFinished-=DidPlayersShowCardsFinished;
			
			PhotonClient.SyncGameDataTableInfoEvent-=DidSyncGameDataTableInfoEvent;
			PhotonClient.LeaveGameFinished-=DidLeaveGameFinished;
			UtilityHelper.MaskTableAdditionalBehaviourEvent -= StartAdditionalBehaviour;
			
			PhotonClient.MaskingTablePopUPEvent -=PopupMaskingTable;
			
			PhotonClient.StandUpEvent-=DidStandUpEvent;
			
		}
		
		
		 
	}
}

 