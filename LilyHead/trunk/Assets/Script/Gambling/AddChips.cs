using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using DataPersist;

public class AddChips : MonoBehaviour {
	
	public GameObject slider;
	
	void InitChip()
	{
		
	}
	
	
	void setlabChip(string name,string str)
	{
		Transform tr=transform.FindChild(name);
		UILabel lb=tr.GetComponent<UILabel>();
		lb.text=str;
		Transform sub=tr.FindChild("Label_sub");
		UILabel sublb=sub.GetComponent<UILabel>();
		sublb.text=str;
	}
	
	
	void addChips()
	{
		
	}
	
	
	void subChips()
	{
		
	}
	void OnSliderChange()
	{
		
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
