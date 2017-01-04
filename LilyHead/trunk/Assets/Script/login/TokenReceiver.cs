using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class TokenReceiver : MonoBehaviour {
	public GameObject SceneManagerGameObject;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void DeviceTokenReceived(string result)
	{
		Debug.Log("here the Device token is: " + result);
		DeviceTokenHelper.myDeviceToken = result;
		if(SceneManagerGameObject!=null)
		{
			SceneManager sceneManger = SceneManagerGameObject.GetComponent<SceneManager>();
			if(sceneManger!=null)
			{
				Debug.Log("sceneManger!=null");
				if(sceneManger.getISODeviceTokenFinishEvent!=null)
				{
					Debug.Log("sceneManger.getISODeviceTokenFinishEvent!=null");
					sceneManger.getISODeviceTokenFinishEvent();
				}
			}
		}
		//PhotonClient.Singleton.SystemSetting();
	}
}

public class DeviceTokenHelper
{
	public static string myDeviceToken = string.Empty;
}
