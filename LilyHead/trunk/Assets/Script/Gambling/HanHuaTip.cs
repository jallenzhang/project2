using UnityEngine;
using System.Collections;

public class HanHuaTip : MonoBehaviour {

	// Use this for initialization
	float realTime=0;
	public int padding=5;
	void Start () {
 		
		realTime=Time.realtimeSinceStartup;
	}
	
	// Update is called once per frame
	void Update () {
	 if(Time.realtimeSinceStartup-realTime>padding)
			Destroy(gameObject);
	}
}
