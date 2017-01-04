using UnityEngine;
using System.Collections;

public class gameBtnAction : MonoBehaviour {
	
	public GameObject parentItem;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	IEnumerable loadEmptyScene()
	{
		AsyncOperation async = Application.LoadLevelAsync("LoadingTable");
		yield return async;
	}
	
    void btnGoBack()
	{
		foreach(AsyncOperation op in loadEmptyScene())
		{
			if (op.isDone)
				break;
		}
		Application.LoadLevelAsync("BackGround");
	}
	
	void disabledAllBtns()
	{
		
		
		BoxCollider[] boxcoolides = parentItem.GetComponentsInChildren<BoxCollider>();
		foreach(BoxCollider one in boxcoolides)
		{
			one.enabled=false;
		}
		
	}
	
	void OnBack()
	{
		disabledAllBtns();
		
		Debug.Log("!!!!!!! OnBack");
		
		btnGoBack();
	}
}
