using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using System.Timers;
using System;
using LilyHeart;

public class LevelUpDialog : MonoBehaviour {
	private AudioSource audioSource;
	
	public Action popDialogBehavior; 
	
	void Awake()
	{
 
		audioSource=gameObject.AddComponent<AudioSource>();
	}
	// Use this for initialization
	void Start () {
		
 		if(GlobalScript.ScriptSingleton.CurrentInfos.Count>0)
		{
			DialogInfo info = GlobalScript.ScriptSingleton.CurrentInfos.Dequeue();
 			 
 			UISprite sprite1=transform.FindChild("Sprite2").GetComponent<UISprite>();
	 		sprite1.spriteName=string.Format("ATUpgradea_{0} ", info.Description[0]);
 			
			UISprite sprite2=transform.FindChild("Sprite1").GetComponent<UISprite>();
			sprite2.spriteName=string.Format("ATUpgradea_{0} ", info.Description[1]);
			 
 	    	Invoke("timeOut", 5);
			
			SoundHelper.PlaySound("Music/Other/levelUpgrade",audioSource);
		}
	}

	void timeOut ()
	{
		CloseDialog();
	}
	
	void CloseDialog()
	{		
		CancelInvoke("timeOut");
		
		if(popDialogBehavior != null){
			popDialogBehavior();			
		}
		
		fadePanel fPanel = transform.GetComponent<fadePanel>();
		fPanel.fadeOut(null);
		Destroy(audioSource);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

public class LevelUpInfo : DialogInfo
{
	public LevelUpInfo(int level)
	{
		this.Description = string.Format("{0:D2}", level);
	}
}
