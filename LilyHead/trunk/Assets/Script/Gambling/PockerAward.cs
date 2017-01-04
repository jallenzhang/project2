using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using DataPersist;
using LilyHeart;

public class PockerAward : MonoBehaviour {

	public GameObject prefab;
	int AwardTime=180;
	bool started=false;
	//float realTime=-1;
	// Use this for initialization
	void Start () {
	
	 
		//realTime=Time.realtimeSinceStartup;
		PhotonClient.OnlineAwardsResponseEvent+=DidOnlineAwardsResponseEvent;
 		
		PhotonClient.StartGameEvent+=DoStartGameEvent;
		PhotonClient.JoinGameEventFinished+=DoJoinGameEventFinished;
		PhotonClient.StandUpEvent+=DoStandUpEvent;
		PhotonClient.SitDownEvent+=DoSitDownEvent;
		PhotonClient.PlayerLeavedEvent+=DoPlayerLeavedEvent;

		gameObject.SetActiveRecursively(false);
		
		
	}
	void DoJoinGameEventFinished(ErrorCode error,TypeState gamestate)
	{
		TableInfo info=Room.Singleton.PokerGame.TableInfo;
 		if(info!=null)
		{
			if(info.Players.Count>2)
			{
				if(! Util.isMatch())
					DoStartGameEvent();
			}
		}
	}
 	// Update is called once per frame
	void Update () {
  		 
	}
	void DoStandUpEvent(int Noseat)
	{
		gameObject.SetActiveRecursively(false);
		CancelInvoke();
		AwardTime=0;
	}
	void DoSitDownEvent(TypeState gamestate)
	{
		TableInfo info=Room.Singleton.PokerGame.TableInfo;
 		if(info!=null)
		{
			if(info.Players.Count>2)
			{
				gameObject.SetActiveRecursively(true);
				ReSetInfor(null);
			}
			else
			{
				started=false;
			}
		}
		
	}
	void DoStartGameEvent()
	{
		Debug.Log("started:"+started.ToString() + "  NoSeat:"+User.Singleton.UserData.NoSeat.ToString() );
		if(started==false && User.Singleton.UserData.NoSeat!=-1)
		{
			gameObject.SetActiveRecursively(true);
			started=true;
			ReSetInfor(null);
		}
		
	}
	void ReSetInfor(GameObject btn)
	{
	    if(btn!=null)
		{
			Destroy(btn.transform.parent.gameObject);
		}
  
		Debug.Log(" User.Singleton.canGetOnlineAwards=="+User.Singleton.canGetOnlineAwards);
		if(User.Singleton.canGetOnlineAwards==true)
		{
			AwardTime=0;
			transform.FindChild("Pic_lingqian").gameObject.SetActiveRecursively(true);
			transform.FindChild("Button/shine_Little").gameObject.SetActiveRecursively(true);
			
			transform.FindChild("Button/Background").gameObject.SetActiveRecursively(false);
	 		transform.FindChild("Label").GetComponent<UILabel>().text="0:00";
			transform.FindChild("Label").gameObject.SetActiveRecursively(false);
		}
		else
		{
 			AwardTime=179;	
			transform.FindChild("Pic_lingqian").gameObject.SetActiveRecursively(false);
			transform.FindChild("Button/shine_Little").gameObject.SetActiveRecursively(false);
			
			transform.FindChild("Button/Background").gameObject.SetActiveRecursively(true);
	 		transform.FindChild("Label").GetComponent<UILabel>().text="2:59";
			transform.FindChild("Label").gameObject.SetActiveRecursively(true);
						CancelInvoke();

			Invoke("ShowLastTime",1f);
		}
	}
	void ShowLastTime()
	{
	    AwardTime--;
		if(AwardTime<0)
			AwardTime=0;
		
		if(AwardTime>0)
		{
			CancelInvoke();
			Invoke("ShowLastTime",1f);
		}
		else
		{
			AwardTime=0;
			//User.Singleton.canGetOnlineAwards=true;
			transform.FindChild("Pic_lingqian").gameObject.SetActiveRecursively(true);
			transform.FindChild("Button/shine_Little").gameObject.SetActiveRecursively(true);
			transform.FindChild("Button/Background").gameObject.SetActiveRecursively(false);
			transform.FindChild("Label").gameObject.SetActiveRecursively(false);
		}
		
		int miniute=AwardTime/60;
		int seconds=AwardTime%60;
		if(seconds>9)
		{
			string lasttime=string.Format("{0}:{1}",miniute,seconds);
			SetLabelValue("Label",lasttime);
		}
		else
		{
			string lasttime=string.Format("{0}:0{1}",miniute,seconds);
			SetLabelValue("Label",lasttime);
		}
		
	}
	 
    void SetLabelValue(string name,string val)
	{
 		Transform label=transform.FindChild(name);
		UILabel uilabel=label.GetComponent<UILabel>();
		uilabel.text=val;
		
		Transform subLabel=label.FindChild(name);
		if(subLabel!=null)
		{
			UILabel subuilabel=subLabel.GetComponent<UILabel>();
			subuilabel.text=val;
		}
	}
	void DidOnlineAwardsResponseEvent(long award)
	{
 		GameObject gameobjec=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		gameobjec.transform.localScale=new Vector3(1,1,1);
 		gameobjec.transform.parent=transform;
		UIButtonMessage btnmess=gameobjec.transform.FindChild("Button_accept").gameObject.GetComponent<UIButtonMessage>();
	    btnmess.target=gameObject;
		btnmess.functionName="ReSetInfor";
		gameobjec.transform.FindChild("Label_title").GetComponent<UILabel>().text=award.ToString();
		
		User.Singleton.canGetOnlineAwards=false;
	}
	void DoPlayerLeavedEvent(int noseat,bool isKick,PlayerLeaveType leavetype)
	{
		TableInfo infor=Room.Singleton.PokerGame.TableInfo;
 		if(infor!=null)
		{
			if(infor.Players.Count<=2)
			{
				gameObject.SetActiveRecursively(false);
				
				CancelInvoke();
				started=false;
				AwardTime=0;
			}
		}
	}
	void OnDestroy()
	{
		PhotonClient.OnlineAwardsResponseEvent-=DidOnlineAwardsResponseEvent;
		PhotonClient.StartGameEvent-=DoStartGameEvent;
		PhotonClient.JoinGameEventFinished-=DoJoinGameEventFinished;
		PhotonClient.PlayerLeavedEvent-=DoPlayerLeavedEvent;
		PhotonClient.StandUpEvent-=DoStandUpEvent;
		PhotonClient.SitDownEvent-=DoSitDownEvent;


	}
	//int ik=0;
	void GetPockerAward(GameObject btn)
	{
		//Debug.LogWarning("JJJJJJJ "+(ik++));
 		if(AwardTime==0)
			PhotonClient.Singleton.GetOnlineAwards();
	}
	 
	public void SetIsStart(bool bIsStart)
	{
		started =  bIsStart;
	}
}
