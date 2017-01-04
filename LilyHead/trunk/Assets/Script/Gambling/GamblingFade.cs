using UnityEngine;
using System.Collections;

public class GamblingFade : MonoBehaviour {

	// Use this for initialization
	
	public float fadeIntime=0.5f;
	public float fadeOutime=0.75f;
	void Start () {
	
		//fadeInAction();
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
				mColor.r=0;
				mColor.g=0;
				mColor.b=0;
				
 				widget.color=mColor;
				mColor.a=1;
				mColor.r=1;
				mColor.g=1;
				mColor.b=1;
 			   TweenColor colow= TweenColor.Begin(tr.gameObject, fadeIntime, mColor);
				 colow.eventReceiver=gameObject;
				 colow.callWhenFinished="FadeinCallWhenFinished";
			 
 			}
		}
  	}
	
	public void FadeOut()
	{
		 Animation ainmm=GetComponent<Animation>();
		 Destroy(ainmm);
		 Transform[] trs=transform.GetComponentsInChildren<Transform>();
		 foreach(Transform tr in trs)
		 {
		    UIWidget widget=tr.GetComponent<UIWidget>();
			if(widget)
			{
				Color mColor=widget.color;
				mColor.a=0;
				mColor.r=0;
				mColor.g=0;
				mColor.b=0;
 
 			    TweenColor colow =TweenColor.Begin(tr.gameObject, fadeOutime, mColor);
				colow.eventReceiver=gameObject;
				colow.callWhenFinished="FadeOutCallWhenFinished";
			 
 			}
		}
	}
	void FadeinCallWhenFinished()
	{
		transform.GetComponent<Animation>().Play("rgb_2");
	}
	void FadeOutCallWhenFinished()
	{
		Destroy(gameObject);
		//Debug.Log("FadeinCallWhenFinished");
	}
	// Update is called once per frame
	void Update () {
	
	}
}
