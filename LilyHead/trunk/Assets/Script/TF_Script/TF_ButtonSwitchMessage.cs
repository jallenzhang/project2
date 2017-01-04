using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class TF_ButtonSwitchMessage : MonoBehaviour {
	
	public enum Trigger
	{
		OnClick,
		OnMouseOver,
		OnMouseOut,
		OnPress,
		OnRelease,
		OnDoubleClick,
	}
	
	public GameObject prefab1;
	public GameObject prefab2;
	public GameObject target;
	
	public bool needCondition = false;
	public bool twos = false;
	public GameObject condition_target;
	public string condition_functionName;
	public string param = string.Empty;
	public List<GameObject> tf_params = new List<GameObject>();
	
	public GameObject CallDestory_target;
	public string CallDestory_FunctionName;
	
	public Vector3 prefab1_localPosition=new Vector3(0,0,0);
	public Vector3 prefab1_localScale=new Vector3(1,1,1);
	
	public Vector3 prefab2_localPosition=new Vector3(0,0,0);
	public Vector3 prefab2_localScale=new Vector3(1,1,1);
	
	public Trigger trigger = Trigger.OnClick;
	
	private UIButtonMessage buttonMessage;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	void beginDealCondition()
	{
		UtilityHelper.ChooseFirstOneEvent += ChooseFirstOneEvent;
		
		condition_target.SendMessage(condition_functionName, gameObject, SendMessageOptions.DontRequireReceiver);
	}
	
	void ChooseFirstOneEvent(bool result)
	{
		UtilityHelper.ChooseFirstOneEvent -= ChooseFirstOneEvent;
 		PopUpDialog(result);
		 
	}
	
	void OnClick () 
	{ 
		if (enabled && trigger == Trigger.OnClick)
		{
			if (!needCondition)
				PopUpDialog(true);
			else
			{
				if(twos)
					beginDealCondition();
				else
					PopUpDialog(true);

			}
		}
	}
	
	void PopUpDialog (bool isfirst)
	{	
		GameObject item=Instantiate(isfirst?prefab1:prefab2,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		
		if (!string.IsNullOrEmpty(param))
		{
			TF_ParamScript tf_param = item.GetComponentInChildren<TF_ParamScript>();
			if (tf_param != null)
			{
				 
					tf_param.SetStringParam(param);
			}
			
		}
		if(CallDestory_target!=null)
		{
			TF_ParamScript tf_param = item.GetComponentInChildren<TF_ParamScript>();
			if (tf_param != null)
			{
 				tf_param.SetTargetParam(CallDestory_target,CallDestory_FunctionName);
			}
		}
			 
		
		if (tf_params != null && tf_params.Count > 0)
		{
			TF_ParamScript tf_param = item.GetComponentInChildren<TF_ParamScript>();
			if (tf_param != null)
			{
				tf_param.SetParams(tf_params);
			}
		}
		
		if (target != null)
			item.transform.parent=target.transform;
		item.transform.localPosition=isfirst?prefab1_localPosition:prefab2_localPosition;
		item.transform.localScale = isfirst?prefab1_localScale:prefab2_localScale;
	}
}
