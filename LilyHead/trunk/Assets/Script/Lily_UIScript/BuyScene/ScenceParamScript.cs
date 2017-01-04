using UnityEngine;
using System.Collections;
using AssemblyCSharp.Helper;
using System;

public class ScenceParamScript : TF_ParamScript {
	
	public GameObject scene;
	public GameObject sceneTitle;
	public GameObject sceneDescription;
	public GameObject scenePrice;
	private const string atlas = "Atlas";
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	UIAtlas getAtlas(string tile)
	{
		string path=string.Format("BuySenceTable/{0}Atlas",tile);
 		GameObject loadaltalsObject=Resources.Load(path) as GameObject;
		return loadaltalsObject.GetComponent<UIAtlas>();
	}
	
	protected override void DealWithStringParam ()
	{
		string[] tf_strParams = tf_strParam.Split('|');
		
		if (tf_strParams != null && tf_strParams.Length > 0)
		{
			if (scene != null)
			{
				UISprite sprite = scene.GetComponent<UISprite>();
				sprite.atlas = getAtlas(tf_strParams[0]);
				sprite.spriteName = tf_strParams[0];
				MusicManager.Singleton.PlayForeMusic(tf_strParams[0]);
			}
			
			if (sceneTitle != null)
			{
				UILabel lable = sceneTitle.GetComponent<UILabel>();
				if (lable != null)
				{
					lable.text = LocalizeHelper.Translate(tf_strParams[1]);
				}
			}
			
			if (sceneDescription != null)
			{
				UILabel lable = sceneDescription.GetComponent<UILabel>();
				if (lable != null)
				{
					lable.text = LocalizeHelper.Translate(tf_strParams[2]);
				}
			}
			
			if (scenePrice != null)
			{
				UILabel lable = scenePrice.GetComponent<UILabel>();
				if (lable != null)
				{
					lable.text = tf_strParams[3];
				}
			}
		}
		
	}
	
	protected override void DealWithParams ()
	{
		if (tf_params != null && tf_params.Count > 0)
		{
			BuySceneScript buySceneScript = tf_params[0].GetComponent<BuySceneScript>();
			BuySceneScript mySceneScript = gameObject.GetComponent<BuySceneScript>();
			if (buySceneScript != null && mySceneScript != null)
			{
				mySceneScript.sceneType = buySceneScript.sceneType;
				mySceneScript.sceneState = buySceneScript.sceneState;
				mySceneScript.parentItem = buySceneScript.parentItem;
				mySceneScript.sceneName = buySceneScript.sceneName;
			}
		}
	}
	
	void OnDestroy()
	{
		MusicManager.Singleton.StopForeMusic();
	}
}
