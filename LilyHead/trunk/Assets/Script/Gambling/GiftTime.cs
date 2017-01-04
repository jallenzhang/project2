using UnityEngine;
using System.Collections;

public class GiftTime : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	float liveTime=600;
	float realTime=-1;
	public void setCaculateGiftTime(string iconName)
	{
		realTime=Time.realtimeSinceStartup;
		transform.parent.GetComponent<PockerPlayer>().GiftIcon=iconName;
	}
	 
    public void cancelShowGift()
	{
		realTime=-1;
		transform.FindChild("Background").GetComponent<UISprite>().spriteName="GIFGiftBtn";
		transform.FindChild("Background").transform.localScale=new Vector3(47,47,1);
		transform.parent.GetComponent<PockerPlayer>().GiftIcon="GIFGiftBtn";
 	}
	// Update is called once per frame
	void Update () {
	
		if(realTime!=-1 && Time.realtimeSinceStartup-realTime>liveTime)
		{
			cancelShowGift();
		}
	}
	
	
}
