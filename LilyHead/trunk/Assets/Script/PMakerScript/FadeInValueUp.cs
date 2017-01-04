using UnityEngine;
using System.Collections;

public class FadeInValueUp : MonoBehaviour {
	
	//public float csFadeIn = 0;
	public float pfadeIn = 0;
	float selfAdd = 0;
	
	// Use this for initialization
	void Start () {
		//testStart();
		SetFadeInValue();	
		
		//InitChipsStatus();
		//useCodeChange();
	}
	
	// Update is called once per frame
	void Update () {
		//testUpdate();
		SetFadeInValue();
		
		//useCodeChange();
	}
	
	void SetFadeInValue(){
		// pfadeIn = (int)(LoadingPercentHelper.Progress * 100);
		// Debug.Log("Fade In LoadingPercentHelper: " + LoadingPercentHelper.Progress);
		if(pfadeIn < LoadingPercentHelper.Progress){
			pfadeIn = LoadingPercentHelper.Progress;
		}		
	}
	
	void testStart(){
		pfadeIn = 0.7f;
		selfAdd = pfadeIn;
	}
	
	void testUpdate(){
		if(pfadeIn > 1) return;
		
		selfAdd += 0.01f;
		pfadeIn = selfAdd;
		// Debug.LogError("pfadeIn: " + pfadeIn);
	}
	
	#region The loading is hight by the code
	
	void useCodeChange(){
		Condition(LoadingPercentHelper.Progress);		
		//Condition(pfadeIn);
	}
	
	void Condition(float myProgress){
		Debug.LogError("pfadeIn: " + myProgress);
		if(myProgress >= 0.8f){
			HightRedChip();
			HightP1Chip();
			HightP2Chip();
			HightYellowChip();
			HightPoint3Chip();
			HightPoint4Chip();
			HightGreenChip();
		} else if(myProgress >= 0.7f){
			HightRedChip();
			HightP1Chip();
			HightP2Chip();
			HightYellowChip();
			HightPoint3Chip();
			HightPoint4Chip();
		} else if(myProgress >= 0.6f){
			HightRedChip();
			HightP1Chip();
			HightP2Chip();
			HightYellowChip();
			HightPoint3Chip();
		} else if(myProgress >= 0.5f){
			HightRedChip();
			HightP1Chip();
			HightP2Chip();
			HightYellowChip();
		} else if(myProgress >= 0.3f){
			HightRedChip();
			HightP1Chip();
			HightP2Chip();
		} else if(myProgress >= 0.2f){
			HightRedChip();
			HightP1Chip();
		} else if(myProgress >= 0.1f){
			HightRedChip();
		}
	}
	
	void HightGreenChip(){
		SetChipStatus("GreenChip");
	}
	
	void HightPoint4Chip(){
		SetChipStatus("point4");
	}
	
	void HightPoint3Chip(){
		SetChipStatus("point3");
	}
	
	void HightYellowChip(){
		SetChipStatus("YellowChip");
	}
	
	void HightP2Chip(){
		SetChipStatus("point2");
	}
	
	void HightP1Chip(){
		SetChipStatus("point1");
	}
	
	void HightRedChip(){
		SetChipStatus("RedChip");
	}
	
	void SetChipStatus(string name){
		float endValue = 1;
	    Transform myTrans = this.transform.FindChild(name);
	 	TweenColor myTweenColor = myTrans.GetComponent<TweenColor>();
		myTweenColor.from.a = endValue;
		myTweenColor.to.a = endValue;
		
		UISprite myUISprite = myTrans.GetComponent<UISprite>();
		myUISprite.alpha = endValue;
	}
	
	void InitChipsStatus(){
		float initValue = 0.3f;
		string[] allChips = new string[]{"RedChip", "point1", "point2", "YellowChip", "point3", "point4", "GreenChip"};
		for(int i=0; i<allChips.Length; i++){
			Transform myTrans = this.transform.FindChild(allChips[i]);
		 	TweenColor myTweenColor = myTrans.GetComponent<TweenColor>();
			myTweenColor.from.a = initValue;
			myTweenColor.to.a = initValue;
			
			UISprite myUISprite = myTrans.GetComponent<UISprite>();
			myUISprite.alpha = initValue;
		}
	}
	
	#endregion
}
