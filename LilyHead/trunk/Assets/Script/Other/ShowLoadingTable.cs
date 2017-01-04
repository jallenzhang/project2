using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using LilyHeart;

public class ShowLoadingTable : MonoBehaviour {

	// Use this for initialization
	float defaultTime = 30.0f;
	float retryTime = 5.0f;
	private GameObject target;
	private string methodName;
	
	void Start () {
		if (User.Singleton.MaskingTableOpened)
			UtilityHelper.CloseMaskingTable();
		
		User.Singleton.MaskingTableOpened = true;
		UtilityHelper.MaskTableTryAgainEvent += TryAgain;
		GameObject item=Instantiate(Resources.Load("LoadingTable_1/LoadingTable")) as GameObject;
		
		if(gameObject.transform.parent.camera!=null)
		{
			gameObject.transform.parent.camera.depth=200;
		}
		item.transform.parent=gameObject.transform;
		item.layer=gameObject.layer;

		item.transform.localPosition=new Vector3(0f,0f,-200f);
		item.transform.localScale=new Vector3(1f,1f,1f);
		
		StartCoroutine(MusicManager.Singleton.BgFadeOut(0.5f));
		StartWait();
		StartOperationAgain();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void SetCallback(GameObject obj, string MethodName)
	{
		target = obj;
		methodName = MethodName;
	}
	
	void StartOperationAgain()
	{
		StartCoroutine("OperationAgain");
	}
	
	int i = 0;
	
	IEnumerator OperationAgain()
	{
		yield return new WaitForSeconds( retryTime );
		
		i++;
		if (i > 2)
		{
			PhotonClient.Singleton.Disconnect();
			i = 0;
		}
		
		if (target != null)
		{
			Debug.Log("methodName is: " + methodName);
			
			target.SendMessage(methodName, gameObject, SendMessageOptions.DontRequireReceiver);
		}
		StopCoroutine("OperationAgain");
		StartOperationAgain();
	}
	
	void TryAgain()
	{
		StopCoroutine("WaitAndClose");
		StartWait();
		if (target != null)
		{
			Debug.Log("methodName is: " + methodName);
			target.SendMessage(methodName, gameObject, SendMessageOptions.DontRequireReceiver);
		}
	}
	
	void StartWait()
	{
		StartCoroutine("WaitAndClose");
	}
	
	IEnumerator WaitAndClose()
	{
		yield return new WaitForSeconds( defaultTime );
		User.Singleton.MessageOperating = true;
		GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new NetworkErrorDialog());
		PopUpTips();
	}
	
	void PopUpTips()
	{
		GameObject prefab=Resources.Load("prefab/tips2") as GameObject;
		GameObject dialog=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		
		dialog.transform.parent=transform;
		dialog.layer = gameObject.layer;
		dialog.transform.FindChild("Button_red").gameObject.layer = gameObject.layer;
		dialog.transform.FindChild("Button_blue").gameObject.layer = gameObject.layer;
		dialog.transform.localPosition=new Vector3(0,200,-401);
		dialog.transform.localScale =new Vector3(1,1,1);
	}
	
	void OnDestroy()
	{
		StopCoroutine("WaitAndClose");
		StopCoroutine("OperationAgain");
		User.Singleton.MessageOperating = false;
		User.Singleton.MaskingTableOpened = false;
		UtilityHelper.MaskTableTryAgainEvent -= TryAgain;
	}
}
