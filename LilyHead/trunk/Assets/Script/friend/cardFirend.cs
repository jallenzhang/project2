using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using DataPersist;
using AssemblyCSharp.Helper;
using LilyHeart;

public class cardFirend : MonoBehaviour {
	
 	public UserData friend;
	AsyncOperation async = null;
	bool hasJoined=false;
	//RoomController controller=RoomController.Singleton;
	// Use this for initialization
	void Start () {
	
	}
	
	IEnumerator NetworkUpdate()
	{
		PhotonClient.Singleton.Update();
		yield return null;
	}
	
	// Update is called once per frame
	void Update () {
		SetProgress();
		StartCoroutine(NetworkUpdate());
	}
	
	/// <summary>
	/// Disableds all btns.
	/// </summary>
//	void disabledAllBtns()
//	{
//		BoxCollider[] boxcoolides = transform.parent.parent.parent.GetComponentsInChildren<BoxCollider>();
//		foreach(BoxCollider one in boxcoolides)
//		{
//			one.enabled=false;
//		}
//		
//	}
	
	void OnDestroy()
	{
		PhotonClient.JoinGameFinished-=GotoGame;
	}
	void test()
	{
		Debug.Log(Time.realtimeSinceStartup);
	}
	
	void SetProgress()
	{
		if (async != null)
		{
			Debug.Log("Progress is: " + async.progress);
			LoadingPercentHelper.Progress = async.progress;
		}
	}
	
	void loadingScene()
	{
		transform.parent.parent.gameObject.AddComponent<ShowLoadingTable>();
		async = Application.LoadLevelAsync("BackGround");
	}
	
	long GetMaxSendChips()
	{
		long chipsOwn = User.Singleton.UserData.Chips;
		chipsOwn -= 3000;
		if (chipsOwn <= 0)
			return 0;
		
		long levelChips = (User.Singleton.UserData.Level - 1) * 500;
		
		long result = chipsOwn - levelChips;
		
		if (result <= 0)
			return 0;
		
		return result;
	}
	
	private void retryGotoFriend()
	{
		PhotonClient.Singleton.GotoFriend(friend.UserId);
	}
	
    void btnGotoFriend()
	{
		ShowLoadingTable table = gameObject.transform.root.GetChild(0).GetChild(0).gameObject.AddComponent<ShowLoadingTable>();
		table.SetCallback(gameObject, "retryGotoFriend");
		
		PhotonClient.Singleton.GotoFriend(friend.UserId);
		Debug.Log("!!!!!!! btnGotoFriend");
	}
	
	void GotoGame(bool iswork,TypeState gamesate)
	{
		UtilityHelper.CloseMaskingTable();
		//Debug.LogWarning("JoinGameFinished");
		PhotonClient.JoinGameFinished-=GotoGame;
		if (PhotonClient.Singleton.ErrorMessage == ErrorCode.Sucess.ToString())
		{
			if(transform.parent.parent.parent.parent.parent!=null)
			{
				transform.parent.parent.parent.parent.parent.gameObject.AddComponent<ShowLoadingTable>();
				Application.LoadLevelAsync("GamblingInterface_Title");
			}
		}
		else if ((PhotonClient.Singleton.ErrorMessage == ErrorCode.TableNotExist.ToString()
			|| PhotonClient.Singleton.ErrorMessage == ErrorCode.GameIdNotExists.ToString()))
		{
			User.Singleton.MessageOperating = true;
			GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new TableNotExistDialog());
			PopUpTips();
			hasJoined=false;
		}
	}
	
	void JoinGameFinished(bool iswork,TypeState gamesate)
	{
		
		UtilityHelper.CloseMaskingTable();
		//Debug.LogWarning("JoinGameFinished");
		PhotonClient.JoinGameFinished-=JoinGameFinished;
		
		if (PhotonClient.Singleton.ErrorMessage == ErrorCode.Sucess.ToString())
		{
			//JoinGamblingSettingInfor.Singleton.haveDone=true;
			PhotonClient.JoinGameFinished += GotoGame;
			PhotonClient.Singleton.CurrentAction=PhotonClient.Singleton.TryJoinGame;
			PhotonClient.Singleton.InviterId=friend.UserId;
			User.Singleton.JoinRoom(friend.UserId);
			//GotoGame();
		}
		else if ((PhotonClient.Singleton.ErrorMessage == ErrorCode.TableNotExist.ToString()
			|| PhotonClient.Singleton.ErrorMessage == ErrorCode.GameIdNotExists.ToString()))
		{
			User.Singleton.MessageOperating = true;
			GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new TableNotExistDialog());
			PopUpTips();
			hasJoined=false;
		}
		else if (PhotonClient.Singleton.ErrorMessage == ErrorCode.ChipsNotEnough.ToString())
		{
			User.Singleton.MessageOperating = true;
			GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new ChipsNotEnoughDialog(LocalizeHelper.Translate("TIPS_START_GAME_CHIP_NOT_ENOUGH_TITLE"),null,string.Empty));
			PopUpTips();
			hasJoined=false;
		}
	}
	
	void btnJoinGame(GameObject btn)
	{
		if(hasJoined==false)
		{
			hasJoined=true;
			//Debug.LogWarning("OOIIIIIIIIIPPPPPPPP");
			PhotonClient.JoinGameFinished += JoinGameFinished;
			PhotonClient.Singleton.TryJoinGame(friend.UserId);
		}
	}
	
	void PopUpTips()
	{
		GameObject prefab=Resources.Load("prefab/tips2") as GameObject;
		GameObject item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		
		item.transform.parent=transform.parent.parent;
		item.transform.localPosition=new Vector3(0,200,-20);
		item.transform.localScale =new Vector3(1,1,1);
	}
	
	/// <summary>
	/// Buttons the sender chips.
	/// btn press to show senderChips
	/// </summary>
	void btnSenderChips()
	{
		if (GetMaxSendChips() <= 0)
		{
			GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new SendChipNotEnoughDialog());
			PopUpTips();
			return;
		}
		
		GameObject prefab=Resources.Load("prefab/cardFriend/senderchip") as GameObject;
		GameObject item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
	 	item.transform.parent=transform.parent.parent;
		item.transform.localPosition=new Vector3(0,-50,-12);
		item.transform.localScale =new Vector3(1,1,1);
		
		SendChip sendChip = (SendChip)item.GetComponent<SendChip>();
		sendChip.friend = friend;
	}
	
	void btnInviteFriend()
	{
		if (User.Singleton.UserData.NoSeat >= 0)
		{
			Debug.Log("Here in game");
			Player.Singleton.InviteFriendToGame(friend.UserId);
		}	
		else
		{
			Debug.Log("Here in room");
			Player.Singleton.InviteFriendToRoom(friend.UserId);
		}
	}
	
	public void realRequestFriend()
	{
		Player.Singleton.RequestFriend(friend.UserId);
	}
	
	/// <summary>
	/// Enables the allbtns.
	/// </summary>
	void enableAllbtns()
	{
		BoxCollider[] boxcoolides =transform.parent.gameObject.GetComponentsInChildren<BoxCollider>();
		foreach(BoxCollider one in boxcoolides)
		{
			one.enabled=true;
		}
	}
	
	void btnClose()
	{
		enableAllbtns();
		Destroy(gameObject);
	}
	
	
}
