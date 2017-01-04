using UnityEngine;
using System.Collections;

public class LoadingTable : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		if(MusicManager.Singleton.BgAudio!=null)
		{
			StartCoroutine(MusicManager.Singleton.BgFadeOut(0.5f));
		}
	}
	
	// Update is called once per frame
	void Update () {

	}
}
