using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuySceneGridScript : MonoBehaviour {
	public List<GameObject> scenes = new List<GameObject>();
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void UpdateScenes()
	{
		if (scenes != null && scenes.Count > 0)
		{
			foreach (GameObject obj in scenes)
			{
				BuySceneScript sceneScript = obj.GetComponent<BuySceneScript>();
				sceneScript.UpdateScene();
			}
		}
	}
}
