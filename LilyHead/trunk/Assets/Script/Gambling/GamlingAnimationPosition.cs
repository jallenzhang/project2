using UnityEngine;
using System.Collections;

public class GamlingAnimationPosition : MonoBehaviour {

	// Use this for initialization
 	public Vector3 position1=new Vector3(-6,-227,0); 
	public Vector3 position2=new Vector3(-246,-227,0); 
	public Vector3 position3=new Vector3(-420,-138,0); 
	public Vector3 position4=new Vector3(-420,76,0);
	public Vector3 position5=new Vector3(-148,160,0);
	public Vector3 position6=new Vector3(135,160,0);
	public Vector3 position7=new Vector3(398,72,0);
	public Vector3 position8=new Vector3(403,-135,0);
	public Vector3 position9=new Vector3(235,-227,0);
	
	public Vector3 LocalScale=new Vector3(0.5f,0.5f,1);
	
	public int GamblingNo=-1;
		
	void Start () {
		
		Vector3 locateposition=(Vector3)this.GetType().GetField("RoleBigFaceCardPosition"+GamblingNo).GetValue(this);
		transform.localPosition=locateposition;
		transform.localScale=LocalScale;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	 
}
