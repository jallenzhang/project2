using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using DataPersist;
using LilyHeart;

public class KeyboardListener : MonoBehaviour {
	public PlayMakerFSM FSMObject;
	public static bool EscapePressed;
	
	public const string SCENE_LAUNCH_TABLE="LaunchTable";
	public const string SCENE_LAUNCH_TABLE_SIMPLE="LaunchTable_simple";
	public const string SCENE_LAUNCH_TABLE_91="LaunchTable_91";
	public const string SCENE_BACKGROUND="BackGround";
	public const string SCENE_BACKGROUND_SIMPLE="BackGround_simple";
	public const string SCENE_GAME="GamblingInterface_Title";
	public const string SCENE_MATCH="GamblingInterface_game";
	
	// Use this for initialization
	void Start () {
		EscapePressed=false;
	}
	
	// Update is called once per frame
	void Update () {
		if(FSMObject!=null)
		{
			if(!EscapePressed&&Input.GetKeyUp(KeyCode.Escape))
			{
				EscapePressed=true;
				switch(Application.loadedLevelName)
				{
				case SCENE_LAUNCH_TABLE:
				case SCENE_LAUNCH_TABLE_SIMPLE:
				case SCENE_LAUNCH_TABLE_91:
					GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new CloseDialog());
					break;
				case SCENE_BACKGROUND:
				case SCENE_BACKGROUND_SIMPLE:
//					if(User.Singleton.UserData.UserType==DataPersist.UserType.Guest)
//					{
						GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new CloseDialog());
//					}
//					else
//					{
//						((Player)User.Singleton).CurrentInfos.Enqueue(new LogoutDialog());
//					}
					break;
				case SCENE_GAME:
				case SCENE_MATCH:
					GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new GoRoomDialog(Application.loadedLevelName));
					break;
				}
				FSMObject.SendEvent("OnEscapeDialog");
			}
		}
	}
}
