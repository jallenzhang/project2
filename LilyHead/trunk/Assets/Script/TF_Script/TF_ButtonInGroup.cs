using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TF_ButtonInGroup : MonoBehaviour {
	
	public enum Trigger
	{
		OnClick,
		OnMouseOver,
		OnMouseOut,
		OnPress,
		OnRelease,
		OnDoubleClick,
	}
	
	public GameObject target;
	public GameObject parentItem;
	public GameObject selectedBg;
	public string functionName;
	public GameObject addtionalTarget;
	public string addtional_functionName;
	public Trigger trigger = Trigger.OnClick;
	public List<Vector3> selectedBgPositions = new List<Vector3>();//1st, the default postion, and 2nd is selected position
	
	// Use this for initialization
	void Start () {
		if (selectedBg != null)
		{
			if (selectedBgPositions != null && selectedBgPositions.Count > 0)
			selectedBg.transform.localPosition = selectedBgPositions[0];
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnSelect()
	{
		if (enabled && trigger == Trigger.OnClick)
		{
			if (addtionalTarget != null && !string.IsNullOrEmpty(addtional_functionName))
			{
				addtionalTarget.SendMessage(addtional_functionName, gameObject, SendMessageOptions.DontRequireReceiver);
			}
			if(selectedBg!=null&&selectedBgPositions!=null&&selectedBgPositions.Count>0)
				selectedBg.transform.localPosition = selectedBgPositions[1];
			if (parentItem != null)
			{
				TF_ButtonGroup tf_butonGroup = parentItem.GetComponent<TF_ButtonGroup>();
				if (tf_butonGroup != null && tf_butonGroup.buttons != null)
				{
					foreach(var item in tf_butonGroup.buttons)
					{
						if (item.name != gameObject.name)
						{
							TF_ButtonInGroup tf_btnInGroup = item.GetComponent<TF_ButtonInGroup>();
							tf_btnInGroup.OnDeselect();
						}
					}
				}
			}
		}
	}
	
	public void OnDeselect()
	{
		if(selectedBg!=null&&selectedBgPositions!=null&&selectedBgPositions.Count>0)
			selectedBg.transform.localPosition = selectedBgPositions[0];
	}
}
