using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using DataPersist;
using LilyHeart;

public class HelpPromtScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (User.Singleton.UserData.Avator == 0 || (User.Singleton.UserData.UserType == UserType.Guest 
			&& !User.Singleton.guestInfoDialog))
		{
			GameObject obj = Resources.Load("prefab/HelpPromt") as GameObject;
			GameObject item=Instantiate(obj) as GameObject;
			
			item.transform.parent=gameObject.transform;
			item.layer=gameObject.layer;
	
			item.transform.localPosition=new Vector3(0f,0f,-1f);
			item.transform.localScale=new Vector3(1,1,1f);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
