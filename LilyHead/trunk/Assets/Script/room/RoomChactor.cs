using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
 

public class RoomChactor : MonoBehaviour {

		
      public string EasyMotionName="ArbTy";	
	  float speedX=20.0f;
	  float speedY=10.0f;
	
      float stopTime=5;
	  float realTime=-1;
	
	  EasyMotion2D.SpriteAnimation spriteAnimation;
	
	    
	  
	   	enum PlayerEasyMotionState
		{
		 	Hall_Walk_Rdw_Ani=1,
 		    Hall_Stand_Rdw_Ani,
			Hall_Walk_Rup_Ani,
		    Hall_Stand_Rup_Ani,
		   
		    Other,
  		}
	
	  	PlayerEasyMotionState currentState=PlayerEasyMotionState.Hall_Walk_Rdw_Ani;
  
		 
		public int GetRandomNum(int min,int max)
	    {
		     System.Random random=new System.Random();
	         return random.Next(min,max);
	    }
		// Use this for initialization
		void Start () {
		
			Debug.Log("currentState:"+currentState);
			spriteAnimation=GetComponent<EasyMotion2D.SpriteAnimation>();
		 spriteAnimation.Stop();
			spriteAnimation.Play(EasyMotionName+"_"+currentState.ToString());
			speedX=GetRandomNum(5,15);
			speedY=GetRandomNum(1,3);
		
 	 	}
 	    
	    void NextEasyMotionState()
	    {
		  
		    currentState=currentState+1;
		 	if(currentState==PlayerEasyMotionState.Other)
				currentState=PlayerEasyMotionState.Hall_Walk_Rdw_Ani;
		
			Debug.Log("currentState"+currentState);
			if(currentState==PlayerEasyMotionState.Hall_Walk_Rdw_Ani||currentState==PlayerEasyMotionState.Hall_Stand_Rdw_Ani)
			{
 			   transform.localScale=new Vector3(transform.localScale.x>0?transform.localScale.x:-transform.localScale.x,transform.localScale.y,1);
			}
		    else
		    {
			    transform.localScale=new Vector3(transform.localScale.x<0?transform.localScale.x:-transform.localScale.x,transform.localScale.y,1);

		    }
		
		    if(currentState==PlayerEasyMotionState.Hall_Stand_Rdw_Ani||currentState==PlayerEasyMotionState.Hall_Stand_Rup_Ani)
			{
				realTime=Time.realtimeSinceStartup;
			}
		    
		    spriteAnimation.Stop();
 		 	spriteAnimation.Play(EasyMotionName+"_"+currentState.ToString());
  	    }
	
	  
	    
	    void isNeedToChangeState()
		{
 		
		    if(currentState==PlayerEasyMotionState.Hall_Walk_Rdw_Ani)
			{
			   if(transform.localPosition.x>-40)
			   {
			    	NextEasyMotionState();
			   }
		
		    }
			else if(currentState==PlayerEasyMotionState.Hall_Stand_Rdw_Ani)
			{
				if(realTime!=-1 && Time.realtimeSinceStartup-realTime>stopTime)
				{
					NextEasyMotionState();
	
				}
			}
			
			else if(currentState==PlayerEasyMotionState.Hall_Walk_Rup_Ani)
			{
				if(transform.localPosition.x<-280)
				   {
				    	NextEasyMotionState();
				   }
			}
			else if(currentState==PlayerEasyMotionState.Hall_Stand_Rup_Ani)
			{
				if(realTime!=-1 && Time.realtimeSinceStartup-realTime>stopTime)
				{
					NextEasyMotionState();
	
				}
			}
		
		
  		}
		
 
		void Update () {
 			
			if(currentState!=PlayerEasyMotionState.Other)
			{
  			   
			isNeedToChangeState();
			    if(currentState==PlayerEasyMotionState.Hall_Walk_Rdw_Ani)
				{
				    transform.localPosition=new Vector3(transform.localPosition.x+Time.deltaTime*speedX,transform.localPosition.y-Time.deltaTime*3,transform.localPosition.z);
 				}
				else if(currentState==PlayerEasyMotionState.Hall_Walk_Rup_Ani)
				{
					transform.localPosition=new Vector3(transform.localPosition.x-Time.deltaTime*speedX,transform.localPosition.y+Time.deltaTime*3,transform.localPosition.z);
				}
			
 		   }
		
		}
}
