using System;
using UnityEngine;

public class TF_RadioButton:MonoBehaviour
{
	public GameObject button;
	public UISlicedSprite BtnBackGroupUISlicedSprite;
	public string RadioButtonName;
	public string SelectBtnBackGroup = "STBtn_Choose";
	public string NoSelectBtnBackGroup = "STBtn_normal";
	public bool bUserSelfBackGroup = false;
	
	public GameObject RadioButtonParentGameObject;
	public string ButtonParentMethodName;
	public bool bUseInRadioButtons;
	
	public string StringSelectValue;//if this button is select,and return this value
	public  bool bIsSelect{get;set;}// if this value is select
	
	private TF_RadioButtons RadioButtonParent;
	
		
//	public GameObject OnSelectTarget;
//	public string OnSelectFunctionName;
//	public GameObject OnNoSelectTarget;
//	public string OnNoSelectFunctionName;
	
	public enum Trigger
	{
		OnClick,
		OnMouseOver,
		OnMouseOut,
		OnPress,
		OnRelease,
		OnDoubleClick,
	}
	//SELECT is ture,NOSELECT is false
	public enum DefaultButtonStatus
	{
		SELECT,
		NOSELECT
	}
	public DefaultButtonStatus DefaultStatus = DefaultButtonStatus.NOSELECT;
	public Trigger trigger = Trigger.OnClick;
	//The button status : true is select,false is no select
	
	// Use this for initialization
	void Start () {
		if(!bUseInRadioButtons)
		{
			Debug.Log("DefaultStatus:"+DefaultStatus.ToString());
			if(DefaultStatus == DefaultButtonStatus.NOSELECT)
				bIsSelect = false;
			else
				bIsSelect = true;
			SetRiBackGroup(SelectBtnBackGroup,NoSelectBtnBackGroup);
		}
		if(string.IsNullOrEmpty(RadioButtonName)&&button!=null)
			RadioButtonName = button.name;
	}
	// Update is called once per frame
	void Update () {
	}
	public void OnClick () 
	{ 
		Debug.Log("RadioButtonOnClick");
		if (enabled &&trigger == Trigger.OnClick)
		{
			Debug.Log("RadioButtonName:"+this.RadioButtonName);
			bIsSelect = !bIsSelect;
			if(bUseInRadioButtons&&!string.IsNullOrEmpty(RadioButtonName))
			{
				if(RadioButtonParentGameObject!=null)
				{
					RadioButtonParent = RadioButtonParentGameObject.GetComponent<TF_RadioButtons>();
					if(RadioButtonParent!=null&&!string.IsNullOrEmpty(ButtonParentMethodName))
					{
						if(bUseInRadioButtons)
						{
							if(bIsSelect)//if from noSelect change to Select
							{
								RadioButtonParent.SetRadioName(RadioButtonName);
								RadioButtonParent.Invoke(ButtonParentMethodName,0f);
							}	
						}
						else
						{
							RadioButtonParent.SetRadioName(RadioButtonName);
							RadioButtonParent.Invoke(ButtonParentMethodName,0f);
						}
					}
				}
			}
//			else
//			{
//				SetRiBackGroup(SelectBtnBackGroup,NoSelectBtnBackGroup);
//				if(bIsSelect)
//				{
//					SendEventMessage(OnSelectTarget,OnSelectFunctionName);
//				}
//				else
//				{
//					SendEventMessage(OnNoSelectTarget,OnNoSelectFunctionName);
//				}
//			}
		}
	}
	public void SetRiBackGroup(string SelectBackGroupSpriteName,string NoSelectBackGroupSpriteName)
	{
		if(BtnBackGroupUISlicedSprite!=null)
		{
			if(bIsSelect) 
			{
				BtnBackGroupUISlicedSprite.spriteName = SelectBackGroupSpriteName;
			}
			else
			{
				BtnBackGroupUISlicedSprite.spriteName = NoSelectBackGroupSpriteName;
			}
		}
	}
	private void SendEventMessage(GameObject EventObject,string MethodName,UnityEngine.Object Obj)
	{
		if(EventObject!=null&&!string.IsNullOrEmpty(MethodName))
			EventObject.SendMessage(MethodName,Obj,SendMessageOptions.DontRequireReceiver);
	}
	private void SendEventMessage(GameObject EventObject,string MethodName)
	{
		if(EventObject!=null&&!string.IsNullOrEmpty(MethodName))
			EventObject.SendMessage(MethodName,SendMessageOptions.DontRequireReceiver);
	}
	void OnDestroy()
	{
 		CancelInvoke();
	}
}
