using UnityEngine;
using System.Collections;
using System;

public class FadeOutHelper : MonoBehaviour {
	
	public Action FinishBehaviour;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	void MyFinish(){		
		if(FinishBehaviour != null){
			FinishBehaviour();
			FinishBehaviour = null;			
		}
	}
}
