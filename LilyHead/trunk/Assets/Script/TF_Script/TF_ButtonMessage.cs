using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class TF_ButtonMessage : MonoBehaviour {
	
	public enum Trigger
	{
		OnClick,
		OnMouseOver,
		OnMouseOut,
		OnPress,
		OnRelease,
		OnDoubleClick,
	}
	
	public GameObject prefab;
	public GameObject target;
	
	public bool needCondition = false;
	public GameObject condition_target;
	public string condition_functionName;
	public string param = string.Empty;
	public List<GameObject> tf_params = new List<GameObject>();
	
	public Vector3 prefab_localPosition;
	public Vector3 prefab_localScale;
	
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
		UtilityHelper.PreConditionEvent += endDealCondition;
		
		condition_target.SendMessage(condition_functionName, gameObject, SendMessageOptions.DontRequireReceiver);
	}
	
	void endDealCondition(bool result)
	{
		UtilityHelper.PreConditionEvent -= endDealCondition;
		if (result)
		{
			PopUpDialog();
		}
	}
	
	void OnClick () 
	{ 
		if (enabled && trigger == Trigger.OnClick)
		{
			if (!needCondition)
				PopUpDialog();
			else
				beginDealCondition();
		}
	}
	
	void PopUpDialog ()
	{	
		GameObject item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		
		if (!string.IsNullOrEmpty(param))
		{
			TF_ParamScript tf_param = item.GetComponentInChildren<TF_ParamScript>();
			if (tf_param != null)
			{
				tf_param.SetStringParam(param);
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
		item.transform.localPosition=prefab_localPosition;
		item.transform.localScale = prefab_localScale;
	}
}
