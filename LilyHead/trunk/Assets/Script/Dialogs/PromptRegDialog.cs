using UnityEngine;
using System.Collections;

public class PromptRegDialog : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnRegister()
	{
		//PopupRegTable();
		
		Destroy(gameObject);
	}
	
//	void disabledAllBtns()
//	{
//		BoxCollider[] boxcoolides = transform.parent.GetComponentsInChildren<BoxCollider>();//parentItem
//		foreach(BoxCollider one in boxcoolides)
//		{
//			one.enabled=false;
//		}
//	}
	
	void PopupRegTable()
	{
		//disabledAllBtns();
		GameObject prefab=Resources.Load("prefab/RegTable_2") as GameObject;
 		GameObject item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
			
		item.transform.parent=transform.parent;
		item.transform.localPosition=new Vector3(0,0,-12);
		item.transform.localScale =new Vector3(1,1,1);
	}
}
