using UnityEngine;
using System.Collections;
using ExitGames.Client.Photon;
using AssemblyCSharp.Helper;
using AssemblyCSharp;
using LilyHeart;

public class DebugUI : MonoBehaviour {
	private float frame=0f;
	private float lastInternal=0f;
	private float updateInternal=0.5f;
	private string email="";
	private string password="";
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
			if(Time.realtimeSinceStartup-lastInternal>updateInternal)
		{
			lastInternal=Time.realtimeSinceStartup;
			frame=0f;
		}
		frame++;
	}
	
	void OnGUI()
	{
		GUILayout.BeginArea(new Rect(50,100,200,300));
		GUILayout.Box("fps:"+(frame/(Time.realtimeSinceStartup-lastInternal)).ToString());
		//GUILayout.Box ("test:"+StatusCode.ExceptionOnConnect);
		email=GUILayout.TextField(email);
		password=GUILayout.PasswordField(password,'$');
		if(GUILayout.Button("login"))
		{
			User.Singleton.Login(email,password.getMD5(),DeviceTokenHelper.myDeviceToken);
		}
		GUILayout.EndArea();
	}
}
