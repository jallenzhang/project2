using UnityEngine;
using System.Collections;
using DataPersist;
using AssemblyCSharp;
using LilyHeart;

public class SendChip : MonoBehaviour {
	
	public UserData friend;
	private UISlider slider;
	public UILabel FGlabel;
	public UILabel BGlabel;
	private long currentSendChips = 0;
	UIEventListener listener;
	// Use this for initialization
	void Start () {
		Transform[] alltrans=gameObject.GetComponentsInChildren<Transform>();
		
		foreach(Transform trs in  alltrans)
		{
			if (trs.gameObject.name == "Slider")
			{
				slider = (UISlider)trs.gameObject.GetComponent<UISlider>();
				slider.sliderValue = 0;
				slider.numberOfSteps = 100;
			}
			else if (trs.gameObject.name == "Thumb")
			{
			}
		}
	}
	
	IEnumerator NetworkUpdate()
	{
		PhotonClient.Singleton.Update();
		yield return null;
	}
	
	// Update is called once per frame
	void Update () {
		StartCoroutine(NetworkUpdate());
	}
	
//	void disabledAllBtns()
//	{
//		
//		
//		BoxCollider[] boxcoolides = transform.GetComponentsInChildren<BoxCollider>();
//		foreach(BoxCollider one in boxcoolides)
//		{
//			one.enabled=false;
//		}
//		
//	}
//	
//	void enableAllbtns()
//	{
//		BoxCollider[] boxcoolides =transform.parent.gameObject.GetComponentsInChildren<BoxCollider>();
//		foreach(BoxCollider one in boxcoolides)
//		{
//			one.enabled=true;
//		}
//	}
//	
//	void btnClose()
//	{
//		enableAllbtns();
//		Destroy(gameObject);
//	}
	
	void UpdateLabel()
	{
		FGlabel.text = slider.sliderValue.ToString();
		BGlabel.text = slider.sliderValue.ToString();
	}
	
	void OnPressThumb(GameObject go, bool pressed)
	{
		Debug.Log("Drag " +  slider.sliderValue.ToString());
		this.UpdateLabel();
	}
	
	void OnDragThumb(GameObject go, Vector2 delta)
	{
		Debug.Log("Drag " +  slider.sliderValue.ToString());
		this.UpdateLabel();
	}
	
	long GetMaxSendChips()
	{
		long chipsOwn = User.Singleton.UserData.Chips;
		chipsOwn -= 3000;
		if (chipsOwn <= 0)
			return 0;
		
		long levelChips = (User.Singleton.UserData.Level - 1) * 500;
		
		long result = chipsOwn - levelChips;
		
		if (result <= 0)
			return 0;
		
		return result;
	}
	
	void OnSliderChange()
	{
		if (slider == null)
			return;
		
		UISlider uislider=slider.GetComponent<UISlider>();
		
		float sliderValue=uislider.sliderValue;
		
		long maxSendChips = GetMaxSendChips();
		
		string str=string.Empty;
		currentSendChips = (long)(maxSendChips * sliderValue);
		if (currentSendChips>1000000000)
			str = string.Format("${0:N1}G",(currentSendChips/1000000000000.0f));
		else if (currentSendChips >= 1000000)
			str=string.Format("${0:N1}M",(currentSendChips/1000000.0f));
		else if(currentSendChips>=1000)
		    str=string.Format("${0:N1}K",(currentSendChips/1000.0f));
		else
			str=string.Format("${0}",(currentSendChips));
		
		UILabel flabel = FGlabel.GetComponent<UILabel>();
		flabel.text = str;
		UILabel blabel = BGlabel.GetComponent<UILabel>();
		blabel.text = str;
	}
	
	void onSend()
	{
		if (currentSendChips > 0)
		{
			PhotonClient.Singleton.SendChip(friend.UserId, currentSendChips);
			fadePanel fpanel = gameObject.GetComponent<fadePanel>();
			fpanel.fadeOut(null);
			User.Singleton.UserData.Chips -= currentSendChips;
			User.Singleton.UserInfoChanged();
		}
	}
}
