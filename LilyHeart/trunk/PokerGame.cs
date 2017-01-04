using System;
using DataPersist;
using System.Collections;
using System.Collections.Generic;
using DataPersist.CardGame;
using PokerWorld.HandEvaluator;
using UnityEngine;

namespace LilyHeart
{
	public class PokerGame
	{
		public TableInfo TableInfo {get;set;}
		public Dictionary<int,string> PlayerActionNames {get;set;}
		public int TotalRounds {get;set;}
		public int StepId {get;set;}
		public Queue<PlayerAction> PlayerActions {get;private set;}
		public bool IsWin{get;set;}
		
		public PokerGame ()
		{
			this.PlayerActionNames=new Dictionary<int, string>();
			this.PlayerActions=new Queue<PlayerAction>();
		}
		
		public void WriteDropCache()
		{                 
			string dropCacheContent=Room.Singleton.RoomData.Owner.UserId+"|"+TotalRounds+"|"+StepId;
			Debug.LogWarning("WriteDropCache"+Room.Singleton.RoomData.Owner.UserId+"|"+TotalRounds+"|"+StepId);
			FileIOHelper.WriteFile(FileType.DropCache,dropCacheContent);
		}
		
		public void ClearDropCache(){
			FileIOHelper.WriteFile(FileType.DropCache,string.Empty);
		}
		
		public bool ReadDropCache(out string roomId,out int totalRounds,out int stepId)
		{
			string dropCacheContent=FileIOHelper.ReadFile(FileType.DropCache);
			if(!string.IsNullOrEmpty(dropCacheContent))
			{
				string[] fields=dropCacheContent.Split('|');
				roomId=fields[0];
				totalRounds=Convert.ToInt32(fields[1]);
				stepId=Convert.ToInt32(fields[2]);				
				return true;
			}
			else
			{
				roomId=string.Empty;
				totalRounds=0;
				stepId=0;
				return false;
			}
		}
		
		public void SetPlayerActions(List<PlayerAction> playerActions)
		{
			if(playerActions!=null&&playerActions.Count>0)
			{
				this.PlayerActions.Clear();
				foreach(PlayerAction playerAction in playerActions)
				{
					this.PlayerActions.Enqueue(playerAction);
				}
				GlobalManager.Log ("In PokerGame PlayerActions:"+PlayerActions.Count);
			}
		}
		
		public bool[] GetBestFiveCards(GameCard[] board,int[] winners){	
			 
			bool[] showCards=new bool[7]{false,false,false,false,false,false,false};
			for (int i = 0; i < 5; i++)
            {
                if (board[i].Id < 0)
                {
                    board[i] = null;
                }
            }	
			string b=string.Join(" ",Array.ConvertAll(board,(GameCard card)=>{
				if(card==null)
					return string.Empty;
				return card.ToString ();			
			})).TrimEnd();
			if(TableInfo!=null)
			foreach (int noSeat in winners) {
				PlayerInfo player=TableInfo.GetPlayer(noSeat);
				if(player==null)
					continue;
 				if(player.Cards==null||player.Cards[0].Id<0)
					continue;
				//player.WinsInTable++;
				string p=string.Join(" ",Array.ConvertAll(player.Cards,(GameCard card)=>{return card.ToString ();}));
				bool[] playerCardsShow=GetBestFiveCards(player.Cards,board);
				for (int i = 0; i < 7; i++) {
					showCards[i]=showCards[i]||playerCardsShow[i];
				}
			}
			
			if(!Array.Exists(showCards,(bool s)=>{ return s==true;}))
				showCards=new bool[7]{true,true,true,true,true,true,true};
				
			return showCards;
		}	
		
		
		public bool[] GetBestFiveCards(GameCard[] pocket,GameCard[] board){
			
			for (int i = 0; i < 5; i++)
            {
                if (board[i].Id < 0)
                {
                    board[i] = null;
                }
            }	
		
			string p=string.Join(" ",Array.ConvertAll(pocket,(GameCard card)=>{return card.ToString ();}));
			string b=string.Join(" ",Array.ConvertAll(board,(GameCard card)=>{
				if(card==null)
					return string.Empty;
				return card.ToString ();			
			})).TrimEnd();
			
				
			
			Hand h=new Hand(p,b);
			ulong maskvalue=h.MaskValue;
			
			string fiveCards=p+" "+b;
			
			if(Hand.BitCount (maskvalue)>5){
				
				fiveCards=Hand.BestFiveFromMask (maskvalue);
			}		
			
			bool[] showCards=new bool[7]{false,false,false,false,false,false,false};
			
			int icount=0;
			
			foreach (GameCard item in pocket) {
				if(item!=null&&fiveCards.Contains (item.ToString ()))
					showCards[icount]=true;
				icount++;
			}
			
			foreach (GameCard item in board) {
				if(item!=null&&fiveCards.Contains (item.ToString ()))
					showCards[icount]=true;
				icount++;
			}	
			
			
			Debug.LogWarning(p+" "+b);				
			Debug.LogWarning(string.Join(" ",Array.ConvertAll(showCards,(bool show)=>{return show.ToString ();})));
			
			return showCards;
		}
		
	}
}

