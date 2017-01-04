using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using LilyHeart;

public class MaskingTable : MonoBehaviour {
	
	GameObject item = null;
	float defaultTime = 30.0f;
	float retryTime = 10.0f;
	public bool bDisableAllbtns = true;
	private GameObject target = null;
	private string methodName;
	private bool needPopupDialog = true;
	
	// Use this for initialization
	void Start () {
		User.Singleton.MaskingTableOpened = true;
		User.Singleton.MessageOperating = true;
		UtilityHelper.MaskTableCloseEvent += close;
		UtilityHelper.MaskTableTryAgainEvent += TryAgain;
		
		GameObject obj=Resources.Load("prefab/blackmask") as GameObject;
	    item=Instantiate(obj) as GameObject;
		
		item.transform.parent=gameObject.transform;
		item.layer=gameObject.layer;

		item.transform.localPosition=new Vector3(0f,0f,-20f);
		item.transform.localScale=new Vector3(1,1,1f);
		StartWait();
		StartOperationAgain();
		//UtilityHelper.TimerEnd();
	}
	
	IEnumerator OperationAgain()
	{
		yield return new WaitForSeconds( retryTime );
		
		if (target != null)
		{
			Debug.Log("methodName is: " + methodName);
			target.SendMessage(methodName, gameObject, SendMessageOptions.DontRequireReceiver);
		}
		StopCoroutine("OperationAgain");
		StartOperationAgain();
	}
	
//	void disabledAllBtns()
//	{
//		BoxCollider[] boxcoolides = gameObject.GetComponentsInChildren<BoxCollider>();
//		foreach(BoxCollider one in boxcoolides)
//		{
//			one.enabled=false;
//		}
//	}
	
	// Update is called once per frame
	void Update () {
	}
	
	public void SetLayer(int layer)
	{
		item.layer = layer;
	}
	
	public void SetNeedPopupDialog(bool needPopupDialog)
	{
		this.needPopupDialog = needPopupDialog;
	}
	
	public void SetCallback(GameObject obj, string MethodName)
	{
		target = obj;
		methodName = MethodName;
	}
	
	void PopUpTips()
	{
		//disabledAllBtns();
		bDisableAllbtns = false;
		GameObject prefab=Resources.Load("prefab/tips2") as GameObject;
		GameObject dialog=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		
		dialog.transform.parent=transform;
		dialog.layer = gameObject.layer;
		dialog.transform.FindChild("Button_red").gameObject.layer = gameObject.layer;
		dialog.transform.FindChild("Button_blue").gameObject.layer = gameObject.layer;
		dialog.transform.localPosition=new Vector3(0,200,-21);
		dialog.transform.localScale =new Vector3(1,1,1);
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
	
	void StartOperationAgain()
	{
		StartCoroutine("OperationAgain");
	}
	
	void StartWait()
	{
		//try
		{
			StartCoroutine("WaitAndClose");
		}
//		catch(MissingReferenceException ex)
//		{
//			UtilityHelper.ResetFlags();
//			string levelName = string.Empty;
//			if (Application.loadedLevelName == "GamblingInterface_Title")
//				levelName = "BackGround";//if network error, then go to background anyway, if it is login already before.
//			else
//				levelName = Application.loadedLevelName;
//			Application.LoadLevelAsync(levelName);
//		}
	}
	
	void StartAdditionalBehaviour()
	{
		UtilityHelper.MaskTableAdditionalBehaviour();
	}
	
	IEnumerator WaitAndClose()
	{
		yield return new WaitForSeconds( defaultTime );
		//close();
		if (needPopupDialog)	
		{
			User.Singleton.MessageOperating = true;
			GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new NetworkErrorDialog());
			PopUpTips();
		}
	}
	
	public void close()
	{
		bDisableAllbtns = false;
		User.Singleton.MessageOperating = false;
		User.Singleton.MaskingTableOpened = false;
		UtilityHelper.MaskTableCloseEvent -= close;
		UtilityHelper.MaskTableTryAgainEvent -= TryAgain;
		StopCoroutine("WaitAndClose");
		StopCoroutine("OperationAgain");
		target = null;
		
		//UtilityHelper.TimerStart();
		
		if (item)
		{
			fadePanel panel = item.GetComponent<fadePanel>();
			panel.fadeOut(null);
		}
		// get tablleinfo 
		StartAdditionalBehaviour();
		
		MaskingTable t = gameObject.GetComponent<MaskingTable>();
		if (t != null)
			GameObject.Destroy(t);
	}
	
	void OnDestroy()
	{
		StopCoroutine("WaitAndClose");
		StopCoroutine("OperationAgain");
		User.Singleton.MessageOperating = false;
		User.Singleton.MaskingTableOpened = false;
		UtilityHelper.MaskTableCloseEvent -= close;
		UtilityHelper.MaskTableTryAgainEvent -= TryAgain;
	}
}
