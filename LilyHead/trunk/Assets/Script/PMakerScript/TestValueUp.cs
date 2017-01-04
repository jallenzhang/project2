using UnityEngine;
using System.Collections;

public class TestValueUp : MonoBehaviour {
	
	public float csFadeIn = 0;
	int selfAdd = 0;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		//selfAdd ++;
		csFadeIn = LoadingPercentHelper.Progress;//(float)selfAdd / 100;
	}
}
