using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using System;
using DataPersist;
using LilyHeart;

public class AvatorDialog : MonoBehaviour {
	
	public GameObject grid;
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
		
		StartCoroutine(NetworkUpdate());
	
	}
	
	public void CloseDialog()
	{
	
		
		
		
		if (grid != null)
		{
			ActorsChooseTable table = (ActorsChooseTable)grid.GetComponent<ActorsChooseTable>();
			string currentAvator = table.CurrentAvator;
			Debug.Log(currentAvator);
			byte avator = (byte)Convert.ToInt32(currentAvator);
			
			//TODO: here 1 should be change later
			User.Singleton.UserData.Avator = avator;
			if (Room.Singleton.RoomData.Users.Exists(u => u.UserId == User.Singleton.UserData.UserId))
			{
				UserData user = Room.Singleton.RoomData.Users.Find(u => u.UserId == User.Singleton.UserData.UserId);
				user.Avator = avator;
			}
			User.Singleton.UserData.UserType = DataPersist.UserType.Normal;
			User.Singleton.Save(avator, User.Singleton.UserData.Password, RoomType.Common);
			
			GameObject ChactorsManage = GameObject.Find("ChactorsManage");
			if(ChactorsManage!=null)
			{
				RoomChactorManage chactorManage=ChactorsManage.GetComponent<RoomChactorManage>();
				chactorManage.Oninit();
			}
		}
		
		if (gameObject)
		{
			fadePanel fpanel = gameObject.GetComponent<fadePanel>();
			fpanel.fadeOut(null);
		}
		
			 
	}
}
