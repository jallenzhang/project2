using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using AssemblyCSharp.Helper;

public class PMSceneManager : MonoBehaviour {
//	bool loginOK = false;
//	private User player;
//	bool isContinue = true;
//	// Use this for initialization
//	void Start () {
//		
//		loginOK=false;
//		player = User.Singleton;
//		RegisterRemoteNotification();
//		if (player == null)
//		{
//			player=User.Singleton;
//			PhotonClient.Singleton.Connect();
//			
//			UtilityHelper.LoadResources();
//		}
//	}
//	
//	IEnumerator NetworkUpdate()
//	{
//		PhotonClient.Singleton.Update();
//		yield return null;
//	}
//	
//	// Update is called once per frame
//	void Update () {
//		StartCoroutine(NetworkUpdate());
//	}
//	
//	private void RegisterRemoteNotification()
//	{
//		#if UNITY_IPHONE
//		NotificationServices.RegisterForRemoteNotificationTypes(UnityEngine.RemoteNotificationType.Alert 
//			| UnityEngine.RemoteNotificationType.Badge
//			| UnityEngine.RemoteNotificationType.Sound);
//		#endif
//	}
//	
//	#region LaunchTable State
//	
//	public void LaunchTableState(){
//		if (player.GameStatus==GameStatus.Connected && isContinue)
//		{
//			isContinue = false;
//			player.GameStatus = GameStatus.NoStatus;
//			
//			if (MyVersion.CurrentVersion != GlobalManager.Singleton.CurrentVersion)//need popup dialog to download the latest version  Upgrade
//			{
//				GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new NewVesionDialog(GlobalManager.Singleton.CurrentVersion));
//				PopupUpgradeDialog();
//			}
//			else
//			{
//				if(PlatformHelper.CanAutoLogin())
//				{
//					PlatformHelper.AutoLogin();
//				}
//				else
//				{
//					Application.LoadLevelAsync("LaunchTable");
//				}
//			}
//		}		
//	}
//	
//	void PopupUpgradeDialog()
//	{
//		GameObject prefab=Resources.Load("prefab/tips") as GameObject;
//		GameObject item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
//		
//		item.transform.parent=transform;
//		item.transform.localPosition=new Vector3(0,110,-1);
//		item.transform.localScale =new Vector3(1,1,1);
//	}
//	
//	#endregion
//	
//	#region GotoRoomOrGame
//	
//	public void GoRoomOrGameState(){
//		if((player.GameStatus==GameStatus.InRoom||player.GameStatus==GameStatus.InGame) 
//			&& !loginOK
//			&& isContinue)
//		{
//			isContinue = false;
//			loginOK=true;
//			if(player.GameStatus==GameStatus.InRoom)
//			{
//				Debug.Log("!!!!!!! SceneManager");
//				GotoRoom();
//			}
//			else if(player.GameStatus==GameStatus.InGame)
//			{
//				GotoGame();
//			}
//		}
//	}
//	
//	void GotoRoom()
//	{
//		transform.parent.parent.gameObject.AddComponent<ShowLoadingTable>();
//		AsyncOperation async;
//		if (GlobalManager.Singleton.version == KindOfVersion.Ultimate)
//			async = Application.LoadLevelAsync("BackGround");
//		else
//			async = Application.LoadLevelAsync("BackGround_simple");
//		StartCoroutine(internalLoadLevelAsync(async));
//	}
//	
//	void GotoGame()
//	{
//		transform.parent.parent.gameObject.AddComponent<ShowLoadingTable>();
//		AsyncOperation async = Application.LoadLevelAsync("GamblingInterface_Title");
//		StartCoroutine(internalLoadLevelAsync(async));
//	}
//	
//	private IEnumerator internalLoadLevelAsync(AsyncOperation async)
//	{
//		while(!async.isDone){
//        	LoadingPercentHelper.Progress = async.progress;
//			Debug.Log("aaaaaaaaa " + LoadingPercentHelper.Progress);
//			yield return async.progress;
//		}
//
//		LoadingPercentHelper.Progress = 0;
//    }
//	
//	#endregion
//	
//	#region Error State
//	
//	public void ErrorState(){
//		if(player.GameStatus==GameStatus.Error && !loginOK && isContinue)
//		{
//			isContinue = false;
//			player.GameStatus = GameStatus.NoStatus;
//			GlobalManager.Log(PhotonClient.Singleton.ErrorMessage);
//			UtilityHelper.ShowAlertDialog(LocalizeHelper.Translate("DIALOG_TITLE_ERROR"), PhotonClient.Singleton.ErrorMessage, LocalizeHelper.Translate("DIALOG_BUTTON_OK"));
//			Application.LoadLevelAsync("LaunchTable");
//		}
//	}
//	
//	#endregion
//	
//	#region BackToLaunchTable
//	
//	public void BackToLaunchTable(){
//		if (player.GameStatus == GameStatus.Logout && !loginOK && isContinue)
//		{
//			isContinue = false;
//			player.GameStatus = GameStatus.NoStatus;
//			//FileIOHelper.DeleteFile(FileType.Account);
//			FileIOHelper.WriteFile(FileType.Account, string.Empty);
//			Application.LoadLevelAsync("LaunchTable");
//		}		
//	}
//	
//	#endregion
}
