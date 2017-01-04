using UnityEngine;
using System.Collections;
using System;
using AssemblyCSharp;
using DataPersist;
using DataPersist.CardGame;
using System.Collections.Generic;
using EasyMotion2D;
using AssemblyCSharp.Helper;


public class ChactorController : MonoBehaviour {

	// Use this for initialization
	
	public GameObject playerPanel;
 	
	public float realTime=-1;
	
	public ActorInfor actorinfor;
	
	
	EasyMotion2D.SpriteAnimationState animationState;
 	EasyMotion2D.SpriteAnimation spriteAnimation;
//	EasyMotion2D.SpriteAnimationClip animationClip;
	
	void Start () {
	    //spriteAnimation=GetComponent<EasyMotion2D.SpriteAnimation>();
		//animationClip =spriteAnimation.clip;
		//animationState=spriteAnimation.Play(animationClip.name);
		   
	}
	
	public void SetAnimationState(string name,EasyMotion2D.SpriteAnimation spriteAn)
	{
		//Debug.LogWarning("currentPlayInfo.NoSeat"+currentPlayInfo.NoSeat+" currentPlayInfo.name"+currentPlayInfo.Name);
		ActorInforController controller=playerPanel.GetComponent<ActorInforController>();
		//controller.ShowRoleBg(actorinfor,false);
		
 		spriteAnimation=spriteAn;
 		animationState = spriteAnimation[name];
		 
		realTime=Time.realtimeSinceStartup;
 
 		
	}
	
	void StopAnimation()
	{
		spriteAnimation.Stop();
	 
		ActorInforController controller=playerPanel.GetComponent<ActorInforController>();
		//controller.ShowRoleBg(actorinfor,true);
		
		//Destroy(gameObject);
 	}
	
	// Update is called once per frame
	void Update () {
		 
		if(animationState!=null)
		{
  			if(realTime!=-1 && Time.realtimeSinceStartup-realTime>animationState.length)
			{
				StopAnimation();
	 		}
		}
	  
	}
}
