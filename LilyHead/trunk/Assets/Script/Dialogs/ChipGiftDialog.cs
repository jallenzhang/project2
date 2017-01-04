using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using LilyHeart;

public class ChipGiftDialog : MonoBehaviour {
	public GameObject chipsValue;
	private DialogInfo info = null;
	void Start()
	{
		info = GlobalScript.ScriptSingleton.CurrentInfos.Dequeue();
		if (info != null)
		{
			Transform trs_description = gameObject.transform.FindChild("Label");
			UILabel label = trs_description.gameObject.GetComponent<UILabel>();
			label.text = info.Description;
			UILabel chipslabel = chipsValue.GetComponent<UILabel>();
			chipslabel.text = info.Title;
		}
	}
	
	void enableAllbtns()
	{
		BoxCollider[] boxcoolides =transform.parent.gameObject.GetComponentsInChildren<BoxCollider>();
		foreach(BoxCollider one in boxcoolides)
		{
			one.enabled=true;
		}
	}
	
	void onClose()
	{	
		//fadePanel panel = gameObject.GetComponent<fadePanel>();
		//panel.fadeOut();
		Destroy(gameObject);
		//enableAllbtns();
		//((Player)User.Singleton).CurrentInfos = null;
		User.Singleton.MessageOperating = false;
		User.Singleton.UserInfoChanged();
	}
	
	void Update()
	{
	}
	
}
