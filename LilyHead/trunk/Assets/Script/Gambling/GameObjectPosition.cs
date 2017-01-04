using UnityEngine;
using System.Collections;

public class GameObjectPosition : MonoBehaviour {

	// Use this for initialization
	public int index=-1;
	public Vector3 Localposition1=new Vector3(10,-150,0);
	public Vector3 Localposition2=new Vector3(-46,-126,0);
	public Vector3 Localposition3=new Vector3(-94,-102,0);
	public Vector3 Localposition4=new Vector3(-138,82,0);
	public Vector3 Localposition5=new Vector3(-178,63,0);
	public Vector3 Localposition6=new Vector3(-84,-26,0);
	public Vector3 Localposition7=new Vector3(-10,-47,0);
	public Vector3 Localposition8=new Vector3(55,-78,0);
	public Vector3 Localposition9=new Vector3(96,-98,0);
	public Vector3 LocalScale1=new Vector3(1,1,1);
	public Vector3 LocalScale2=new Vector3(1,1,1);
	public Vector3 LocalScale3=new Vector3(1,1,1);
	public Vector3 LocalScale4=new Vector3(1,1,1);
	public Vector3 LocalScale5=new Vector3(1,1,1);
	public Vector3 LocalScale6=new Vector3(1,1,1);
	public Vector3 LocalScale7=new Vector3(1,1,1);
	public Vector3 LocalScale8=new Vector3(1,1,1);
	public Vector3 LocalScale9=new Vector3(1,1,1);
	public Quaternion LocaleRotate1=Quaternion.Euler(0,0,0);
	public Quaternion LocaleRotate2=Quaternion.Euler(0,0,0);
	public Quaternion LocaleRotate3=Quaternion.Euler(0,0,0);
	public Quaternion LocaleRotate4=Quaternion.Euler(0,0,0);
	public Quaternion LocaleRotate5=Quaternion.Euler(0,0,0);
	public Quaternion LocaleRotate6=Quaternion.Euler(0,0,0);
	public Quaternion LocaleRotate7=Quaternion.Euler(0,0,0);
	public Quaternion LocaleRotate8=Quaternion.Euler(0,0,0);
	public Quaternion LocaleRotate9=Quaternion.Euler(0,0,0);

	void Start () {
		
		if(index!=-1)
		{
			Vector3 Localposition=(Vector3)this.GetType().GetField("Localposition"+index).GetValue(this);
			Vector3 LocalScale=(Vector3)this.GetType().GetField("LocalScale"+index).GetValue(this);
			Quaternion LocaleRotate=(Quaternion)this.GetType().GetField("LocaleRotate"+index).GetValue(this);
	
			transform.localScale=LocalScale;
			transform.localPosition=Localposition;
			transform.localRotation=LocaleRotate;
		}
	
	}
 	// Update is called once per frame
	void Update () {
	
	}
}
