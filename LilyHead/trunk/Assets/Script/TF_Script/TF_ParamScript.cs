using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TF_ParamScript : MonoBehaviour {
	
	protected string tf_strParam;
	protected List<GameObject> tf_params = new List<GameObject>();
	
	protected GameObject target;
	protected string functionName;
		
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void SetStringParam(string param)
	{
		tf_strParam = param;
		DealWithStringParam();
	}
	
	public void SetTargetParam(GameObject tar,string Fname)
	{
 		target=tar;
		functionName=Fname;

	}
	
	public void SetParams(List<GameObject> tf_params)
	{
		this.tf_params = tf_params;
		DealWithParams();
	}
	
	protected virtual void DealWithStringParam()
	{
		
	}
	void OnDestroy()
	{
		if(target!=null)
		{
			target.SendMessage(functionName, gameObject, SendMessageOptions.DontRequireReceiver);
		}
	}
	protected virtual void DealWithParams()
	{
		
	}
}
