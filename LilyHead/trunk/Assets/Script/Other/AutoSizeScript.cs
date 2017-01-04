using UnityEngine;
using System.Collections;
using System;

public class AutoSizeScript : MonoBehaviour {
	private const float BACKGROUND_WIDTH=960f;
	private const float BACKGROUND_HEIGHT=640f;
	
	public static int Height {get;private set;}
	public static int Width {get;private set;}
	
	void Awake()
	{
	}
	
	// Use this for initialization
	void Start () {
		int screenHeight=0;
		int screenWidth=0;

		if(Screen.height>Screen.width)
		{
			Screen.orientation=ScreenOrientation.Landscape;
			screenHeight=Screen.width;
			screenWidth=Screen.height;
		}
		else
		{
			screenHeight=Screen.height;
			screenWidth=Screen.width;
		}
		
		float bg_length_width=BACKGROUND_WIDTH/BACKGROUND_HEIGHT;
		float dev_length_width=(float)screenWidth/(float)screenHeight;
		UIRoot uiroot=gameObject.GetComponent(typeof(UIRoot)) as UIRoot;	
		if(bg_length_width>dev_length_width)
		{
			float scale=screenWidth/BACKGROUND_WIDTH;
			uiroot.manualHeight=Convert.ToInt32(screenHeight/scale);
			Width=Convert.ToInt32(BACKGROUND_WIDTH);
			Height=Convert.ToInt32(BACKGROUND_HEIGHT);
		}
		else
		{
			uiroot.manualHeight=Convert.ToInt32(BACKGROUND_HEIGHT);
			Width=Convert.ToInt32(BACKGROUND_WIDTH);
			Height=uiroot.manualHeight;
		}
		StartCoroutine(ChangeScreenOrentation());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	IEnumerator ChangeScreenOrentation()
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		
		Screen.orientation=ScreenOrientation.AutoRotation;
		yield break;
	}
}
