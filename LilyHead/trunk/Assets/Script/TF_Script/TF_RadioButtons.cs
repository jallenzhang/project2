using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TF_RadioButtons : MonoBehaviour
{
	public List<GameObject>buttons;
	public GameObject TargetGameObjectScene;
	public string MethodName;
	public string SelectBtnBackgroup ="STBtn_Choose";
	public string NoSelectBtnBackgroup = "STBtn_normal";
	private string StringSelectReturnValue;
	public string StringTRUEValue;
	private int priSelectIndex = -1;
	private bool RealValue;
	private  string strRadioName = string.Empty;
	// Use this for initialization
	void Start () {
		int i =-1;
		if(buttons!=null&&buttons.Count>0)
		{
			foreach(GameObject rButton in buttons)
			{
				i++;
				TF_RadioButton TFRadioBtn = rButton.GetComponent<TF_RadioButton>();
				if(TFRadioBtn!=null)
				{
					TFRadioBtn.bUseInRadioButtons = true;
					if(TFRadioBtn.DefaultStatus == TF_RadioButton.DefaultButtonStatus.SELECT)
					{
						if(priSelectIndex == -1)
							priSelectIndex = i;
					}
				}
			}
			i = -1;
			foreach(GameObject rButton in buttons)
			{
				i++;
				TF_RadioButton TFRadioBtn = rButton.GetComponent<TF_RadioButton>();
				if(TFRadioBtn!=null) {
					if(TFRadioBtn.RadioButtonName == strRadioName)
					{
						priSelectIndex = i;
						break;
					}
				}
			}
		}
	}
	// Update is called once per frame
	void Update () {
	}
	private void OnRadioButtonClick()
	{
		Debug.Log("OnRadioButtonClick");
		string RadioName = string.Empty;
		RadioName = strRadioName;
		Debug.Log(strRadioName+"RadioName");
		if(string.IsNullOrEmpty(RadioName)||buttons==null||buttons.Count==0)
			return;
		int nIndex = -1;
		foreach(GameObject rButton in buttons)
		{
			nIndex++;
			TF_RadioButton TFRadioBtn = rButton.GetComponent<TF_RadioButton>();
			if(TFRadioBtn!=null) {
				if(TFRadioBtn.RadioButtonName == RadioName)
				{
					break;
				}
			}
		}
		Debug.Log("nIndex:"+nIndex+"priSelectIndex:"+priSelectIndex+" buttons.Count:"+buttons.Count);
		if(nIndex >=0 &&nIndex < buttons.Count)
		{
			TF_RadioButton button = buttons[nIndex].GetComponent<TF_RadioButton>();
			if(button!=null)
			{
				SetTFRadioButtpnBackGroup(button);
				StringSelectReturnValue = button.StringSelectValue;
				if(priSelectIndex>=0&&priSelectIndex<buttons.Count)
				{
					TF_RadioButton buttonPri = buttons[priSelectIndex].GetComponent<TF_RadioButton>();
					if(buttonPri!=null&&priSelectIndex!=nIndex)
					{
						buttonPri.bIsSelect = false;
						SetTFRadioButtpnBackGroup(buttonPri);
					}
					priSelectIndex = nIndex;
				}
				else
					priSelectIndex = -1;
			}
		}
		if(StringSelectReturnValue == StringTRUEValue)
			RealValue = true;
		else
			RealValue = false;
		if(TargetGameObjectScene!=null&&!string.IsNullOrEmpty(MethodName))
		{
			TargetGameObjectScene.SendMessage(MethodName,RealValue,SendMessageOptions.DontRequireReceiver);
		}
	}
	private void SetTFRadioButtpnBackGroup(TF_RadioButton button)
	{
		if(buttons==null) return;
		if(!button.bUserSelfBackGroup)
		{
			button.SetRiBackGroup(SelectBtnBackgroup,NoSelectBtnBackgroup);
		}
		else
		{
			button.SetRiBackGroup(button.SelectBtnBackGroup,button.NoSelectBtnBackGroup);
		}
	}
	public bool getRealValue()
	{
		return RealValue;
	}
	public void SetRadioName(string str)
	{
		strRadioName = str;
	}
	public void InitRadioButtonStatus(bool bValue)
	{
		RealValue = bValue;
		priSelectIndex = -1;
		if(buttons == null||buttons.Count == 0)
			return;
		int i = -1;
		foreach(GameObject rButton in buttons)
		{
			i++;
			TF_RadioButton TFRadioBtn = rButton.GetComponent<TF_RadioButton>();
			if(TFRadioBtn!=null) {
				if(TFRadioBtn.StringSelectValue == StringTRUEValue)
				{		
					TFRadioBtn.bIsSelect = bValue;
					if(bValue)
					{
						priSelectIndex = i;
						StringSelectReturnValue = TFRadioBtn.StringSelectValue;
					}
				}
				else
				{
					TFRadioBtn.bIsSelect = !bValue;
					if(!bValue)
					{
						priSelectIndex = i;
						StringSelectReturnValue = TFRadioBtn.StringSelectValue;
					}
				}
				SetTFRadioButtpnBackGroup(TFRadioBtn);
			}
		}
	}
}


