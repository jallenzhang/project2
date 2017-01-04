using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using DataPersist;
using System.Collections.Generic;
using LilyHeart;

public class IndicatorBar : MonoBehaviour {
	
   
	bool IndicatorBarHasShowed;
	// Use this for initialization
	void Start () {
	 
		setBarStausValue(0);
		TableState.DopockerBetTurnBegan+=DopockerBetTurnBegan;
		TableState.DoPockerPlayerHoleCardsChangedEvent+=DoPockerPlayerHoleCardsChangedEvent;
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void checkShowBarStaus()
	{
	    //
	}
	void OnDestroy()
	{
		//Debug.LogWarning("OnDes  "+index);
		UnRegisterEventAction();
	}
	void UnRegisterEventAction()
	{
	  	TableState.DopockerBetTurnBegan-=DopockerBetTurnBegan;
		TableState.DoPockerPlayerHoleCardsChangedEvent-=DoPockerPlayerHoleCardsChangedEvent;

	}
	void DoPockerPlayerHoleCardsChangedEvent( List<ActorInfor> temp)
	{
		if(User.Singleton.UserData.NoSeat!=-1)
 			CaculateIndicatorBar(false); 
 	}
	void DopockerBetTurnBegan()
	{
		Debug.LogWarning("DopockerBetTurnBegan");
 		TableInfo info=Room.Singleton.PokerGame.TableInfo;
		if(info!=null && User.Singleton.UserData.NoSeat!=-1)
		{
			if(info.Round == TypeRound.Flop)
			{
					 CaculateIndicatorBar(true);
	 		}
			else if(info.Round==TypeRound.Turn)
			{
					 CaculateIndicatorBar(true);
	    	}
			else if(info.Round==TypeRound.River)
			{
					 CaculateIndicatorBar(true);
			}
		}
	}
	void CaculateIndicatorBar(bool two)
	{
  		TableInfo info=Room.Singleton.PokerGame.TableInfo;
		if(info!=null)
		{
			PlayerInfo playinfo=info.GetPlayer(User.Singleton.UserData.NoSeat);
			if(playinfo!=null)
			{
 				int values= (int)HandStrengthHelper.GetCurrentHandStrength(playinfo.Cards,two?info.Cards:null);
				if(values<10)
				{
					setBarStausValue(values+1);
				}
			}
		}
 	}
	void setBarStausValue(int va)
	{
		Debug.Log("Va "+va);
 		UISlider slider=transform.GetComponent<UISlider>();
		slider.sliderValue=va/10.0f;
   	}
}
