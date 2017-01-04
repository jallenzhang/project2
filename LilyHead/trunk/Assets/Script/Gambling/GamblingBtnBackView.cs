using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using AssemblyCSharp.Helper;
using DataPersist;
using LilyHeart;

public class GamblingBtnBackView : TF_ParamScript {
	
	public GameObject GamblingBtnBackObject;
	public GameObject GamblingBtnBackStandUpObject;
	public GameObject GamblingBtnBackChangeTable;
	public GameObject GamblingGoingtoObject;
	public UILabel lable1;
	public UILabel lable2;
	public UILabel lable3;
	public UILabel lable4;
	public UILabel lable5;
	public UILabel lable6;
	
	public GameObject label;
	// Use this for initialization
	void Start () {
		
		if(Util.isMatch())
		{
			if(MatchInforController.Singleton.HasMatchStarted==false)
			{
			     PopUpBeforeMatchTips();   	
			}
			else if(MatchInforController.Singleton.LoseWinAndStandup==true)
			{
				PopUpMatchOvertips();
			}
			else
				PopUpMatchingTips();// 
		}
		else
		{
			InitLable();
			if(User.Singleton.UserData.NoSeat == -1)
			{
				Util.BeGrayGameObject(GamblingBtnBackStandUpObject,false);
			}

		}
	}
	
	//before match tips
	void PopUpBeforeMatchTips()
	{
		Destroy(lable1.gameObject);
		Destroy(lable2.gameObject);
		Destroy(lable3.gameObject);
		Destroy(lable4.gameObject);
		Destroy(lable5.gameObject);
		Destroy(lable6.gameObject);
        Util.SetLabelValue(label,LocalizeHelper.Translate("BEFORE_MATCH_TIP_TITLE"));
		Util.BeGrayGameObject(GamblingBtnBackStandUpObject,false);
		Util.BeGrayGameObject(GamblingBtnBackChangeTable,false);
	}
	
	//match tips
	void PopUpMatchingTips()
	{
 		Util.SetLabelValue(label,LocalizeHelper.Translate("OVER_MATCH_TIP4"));

		InitLable();
		
   			Util.SetLabelValue(lable3.gameObject,string.Format("{0}",MatchInforController.Singleton.RankPlace));

 		Util.BeGrayGameObject(GamblingBtnBackStandUpObject,false);
		Util.BeGrayGameObject(GamblingBtnBackChangeTable,false);
		
		//Util.BeGrayGameObject(GamblingGoingtoObject,false);

	}
	
	void CalcuteBonus(int place)
	{   
		int num=1;
	    TableInfo info=Room.Singleton.PokerGame.TableInfo;
		if(info!=null)
		{
			
			
			if(User.Singleton.UserData.NoSeat!=-1)
			{
				PlayerInfo myitem=info.GetPlayer(User.Singleton.UserData.NoSeat);
				
				foreach(PlayerInfo item in info.Players)
				{
					if(item.NoSeat != myitem.NoSeat)
					{
						if(item.MoneySafeAmnt == myitem.MoneySafeAmnt)
						{
							num++;
						}
					}
				}
			}
			
			float bonus=0;
			if(place==1)
				bonus=Champion.barChaBarData.fFirstBonus;
			else
				bonus=Champion.barChaBarData.fSecondBonus;
			
			
			Util.SetLabelValue(lable4.gameObject,string.Format("{0}",bonus/num));
 			
		}
	}
	
	//match over
	void PopUpMatchOvertips()
	{
		InitLable();
		Util.SetLabelValue(label,LocalizeHelper.Translate("OVER_MATCH_TIP5"));
		
		Destroy(lable5.gameObject);
		
	
		//Debug.LogError(MatchInforController.Singleton.RankPlace);
		//if(MatchInforController.Singleton.RankPlace!=0)
   		Util.SetLabelValue(lable3.gameObject,string.Format("{0}",MatchInforController.Singleton.RankPlace));
		
		if(MatchInforController.Singleton.RankPlace<=2)
		{
			CalcuteBonus(MatchInforController.Singleton.RankPlace);
			if(MatchInforController.Singleton.RankPlace!=1)
			{
				Util.SetLabelValue(lable6.gameObject,LocalizeHelper.Translate("OVER_MATCH_TIP2"));
			}
			else
			{
				Destroy(lable6.gameObject);
			} 
   			//Util.SetLabelValue(lable4.gameObject,string.Format("{0}",MatchInforController.Singleton.RankPlace));
		}
		else
		{
			Util.SetLabelValue(lable4.gameObject,"0");
						Util.SetLabelValue(lable6.gameObject,LocalizeHelper.Translate("OVER_MATCH_TIP2"));

		}

		//else
		//	Util.SetLabelValue(lable3.gameObject,"5");
			
		Util.BeGrayGameObject(GamblingBtnBackStandUpObject,false);
		Util.BeGrayGameObject(GamblingBtnBackChangeTable,false);
		Util.BeGrayGameObject(GamblingGoingtoObject,false);
	}
	
	// Update is called once per frame
	void Update () {
	}
	void UpdateStandUpButton(GameObject btn)
	{
		if(User.Singleton.UserData.NoSeat == -1)
		{
			if(btn!=null)
			{
				UISlicedSprite StandUpBackGroup = btn.GetComponentInChildren<UISlicedSprite>();
				if(StandUpBackGroup!=null)
					StandUpBackGroup.color = Color.gray;
				BoxCollider box = btn.GetComponent<BoxCollider>();
				if(box!=null)
					box.enabled = false;
			}
		}	
	}
	void BtnBack(GameObject btn)
	{
		if(TableState.Singleton.gametabletype==TableState.GameTableType.GameTableType_Normal)
			ActorInforController.Singleton.BtnBackToRoom();
		else
			MatchInforController.Singleton.BtnBackToRoom();

	}
	void RoleStandUp(GameObject btn)
	{
		User.Singleton.StandUp();
	}
	void QuickRegister(GameObject btn)
	{
		User.Singleton.StandUp();
		ActorInforController.Singleton.QuickRegister();
	}
	void QuickStart()
	{
		if(GamblingBtnBackObject!=null)
			GamblingBtnBackObject.SetActiveRecursively(false);
		
		
		JoinGamblingSettingInfor.Singleton.isFastStart = true;
		PhotonClient.Singleton.isFastStart = true;
		//AsyncOperation async;
		//async =
			Application.LoadLevelAsync("GamblingInterface_Title");
		Destroy(gameObject);
	}
	void PopUpTips()
	{
		GameObject prefab=Resources.Load("prefab/tips2") as GameObject;
		GameObject item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		
		item.layer = ActorInforController.Singleton.topPanel.layer;
		item.transform.parent=transform;
		item.transform.localPosition=new Vector3(0,200,-22);
		item.transform.localScale =new Vector3(1,1,1);
		Transform Button_red = item.transform.FindChild("Button_ok");
		Button_red.gameObject.layer = item.layer;
//		UIButtonMessage redmes = Button_red.GetComponent<UIButtonMessage>();
//		redmes.functionName="BtnBackToRoom";
//		redmes.target=gameObject;
	}
	void GoToHome()
	{
		PhotonClient.Singleton.LeaveGame();
		User.Singleton.JoinRoom(User.Singleton.UserData.UserId);
	}
	void InitLable()
	{
		TableInfo info=Room.Singleton.PokerGame.TableInfo;
		int NWonGameNum = 0;
		int NPlayGameNum = 0;
		long lGetInGameChip = 0;
		long lNowChip = 0;
		long lWinInGameChip = 0;
		string strWinInGameChip = string.Empty;
		if(info!=null)
		{
			PlayerInfo playerInfo = info.GetPlayer(User.Singleton.UserData.NoSeat);
			if(playerInfo != null)
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
				
				if(TableState.Singleton.gametabletype==TableState.GameTableType.GameTableType_Normal)
				{
					ActorInforController.Singleton.NPlayGameNum = NPlayGameNum;
					ActorInforController.Singleton.NWonGameNum = NWonGameNum;
					ActorInforController.Singleton.lGetInGameChip = lGetInGameChip;
					ActorInforController.Singleton.lNowChip = lNowChip;
					ActorInforController.Singleton.lWinInGameChip =lWinInGameChip;
				}
				else
				{
					
					MatchInforController.Singleton.NPlayGameNum = NPlayGameNum;
					MatchInforController.Singleton.NWonGameNum = NWonGameNum;
					MatchInforController.Singleton.lGetInGameChip = lGetInGameChip;
					MatchInforController.Singleton.lNowChip = lNowChip;
					MatchInforController.Singleton.lWinInGameChip =lWinInGameChip;
				}
			}
			else
			{
				if(Util.isMatch())
				{
					NPlayGameNum = MatchInforController.Singleton.NPlayGameNum;
					NWonGameNum = MatchInforController.Singleton.NWonGameNum;
					lGetInGameChip = MatchInforController.Singleton.lGetInGameChip;
					lNowChip = MatchInforController.Singleton.lNowChip;
					lWinInGameChip = MatchInforController.Singleton.lWinInGameChip;
				}
				else
				{
					NPlayGameNum = ActorInforController.Singleton.NPlayGameNum;
					NWonGameNum = ActorInforController.Singleton.NWonGameNum;
					lGetInGameChip = ActorInforController.Singleton.lGetInGameChip;
					lNowChip = ActorInforController.Singleton.lNowChip;
					lWinInGameChip = ActorInforController.Singleton.lWinInGameChip;
				}
				
				
			}
		}
		else
		{
			NWonGameNum = 0;
			NPlayGameNum = 0;
			lGetInGameChip = 0;
			lNowChip = 0;
			lWinInGameChip = 0;
		}
		if(lable1!=null)
			lable1.text = NPlayGameNum.ToString();
		if(lable2!=null)
			lable2.text = NWonGameNum.ToString();
		if(lable3!=null)
			lable3.text = lGetInGameChip >= 100 ? string.Format("{0:0,00}",lGetInGameChip) : lGetInGameChip.ToString();
		if(lable4!=null)
			lable4.text = lNowChip >= 100 ? string.Format("{0:0,00}",lNowChip) : lNowChip.ToString(); lNowChip.ToString();
		if(lable5!=null)
		{
			strWinInGameChip = Mathf.Abs(lWinInGameChip)>= 100 ? string.Format("{0:0,00}",lWinInGameChip) : lWinInGameChip.ToString(); 
			if(lWinInGameChip>0)
				strWinInGameChip = "+" + strWinInGameChip;
			lable5.text = strWinInGameChip;
		}
	}
}
