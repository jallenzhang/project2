using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class fadePanel : MonoBehaviour {

	// Use this for initialization
 
	public float duration=0.1f;
 	
	float mStart=0f;
	bool  user=false;
	bool  IsDestory=false;
	public AudioSource AudioSource {get;set;}
	void Awake()
	{
		
 	}
	void Start () {
 		mStart = Time.realtimeSinceStartup;
				AudioSource=gameObject.AddComponent<AudioSource>();

  	}
	void FadeinCallWhenFinished()
	{
		 
		Debug.Log("FadeinCallWhenFinished");
		Transform[] trs=transform.GetComponentsInChildren<Transform>();
		 foreach(Transform tr in trs)
		 {
		    UIWidget widget=tr.GetComponent<UIWidget>();
			if(widget)
			{
				Color mColor=widget.color;
				mColor.a=1;
				
 				widget.color=mColor;
				mColor.a=1;
 			TweenColor.Begin(tr.gameObject, 0, mColor);
				 
				
				
			}
		 }
 	}
	void fadeInAction()
	{
 		 Transform[] trs=transform.GetComponentsInChildren<Transform>();
		 foreach(Transform tr in trs)
		 {
		    UIWidget widget=tr.GetComponent<UIWidget>();
			if(widget)
			{
				Color mColor=widget.color;
				mColor.a=0;
				
 				widget.color=mColor;
				mColor.a=1;
 			    TweenColor colow=	TweenColor.Begin(tr.gameObject, 0.1f, mColor);
				colow.eventReceiver=gameObject;
				colow.callWhenFinished="FadeinCallWhenFinished";
			 
 			}
		 }
  	}
	
	public void fadeOut(GameObject btn)
	{
		if (btn != null)
		{
			BoxCollider box = btn.GetComponent<BoxCollider>();
			box.enabled = false;
			Destroy(btn);
		}
		
		SoundHelper.PlaySound("Music/Other/viewClose",AudioSource,0);

		IsDestory=true;
		mStart=Time.realtimeSinceStartup;
		
 		 Transform[] trs=transform.GetComponentsInChildren<Transform>();
		 foreach(Transform tr in trs)
		 {
		    UIWidget widget=tr.GetComponent<UIWidget>();
			if(widget)
			{
				Color mColor=widget.color;
				mColor.a=1;
				
 				widget.color=mColor;
				mColor.a=0;
 			 	TweenColor.Begin(tr.gameObject, 0.1f, mColor);
 			}
		 }
	}
	/// <summary>
	/// Enables the allbtns.
	/// </summary>
	void enableAllbtns()
	{
		BoxCollider[] boxcoolides =transform.parent.parent.gameObject.GetComponentsInChildren<BoxCollider>();
		foreach(BoxCollider one in boxcoolides)
		{
			one.enabled=true;
		}
	}
	// Update is called once per frame
	void Update () {
 		if(Time.realtimeSinceStartup-mStart>0.3 && user==false)
		{
 		       user=true;
		}
		
		if(Time.realtimeSinceStartup-mStart>0.2 && IsDestory)
		{
			if(gameObject.name=="SetupTable"&&MusicManager.Singleton.BgAudio.volume<1f)
			{
				return;
			}
			//enableAllbtns();
        	Destroy(gameObject);
		}
 	
	}
}
