using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GamblingBtnsAction : MonoBehaviour {

	// Use this for initialization
	
	 

	
	void Start () {
	
	}
	
	/// <summary>
	/// Sets the button box.
	/// </summary>
	/// <param name='btn'>
	/// Button.
	/// </param>
	void setBtnBox(GameObject btn)
	{
		
	    Transform spriteBoxtrs = btn.transform.FindChild("GIFBox");
  		if(spriteBoxtrs)
		{
			 UISprite  sprite= spriteBoxtrs.GetComponent<UISprite>();
			if(sprite.spriteName=="GIFBox")
			{
				sprite.spriteName="GIFHook";
			}
			else
				sprite.spriteName="GIFBox";
  		}
		 
	}
	
	
	void chagebgPicture(GameObject btn)
	{
		
		
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	/// <summary>
	/// Covers the plate. btn1
	/// </summary>
	void CoverPlate(GameObject btn)
	{
		
		setBtnBox(btn);
		//count--;
	    //cardRotation(card1);
        
	}
	
	
	/// <summary>
	/// Passes the or cover palte. btn2
	/// </summary>
	/// <param name='btn'>
	/// Button.
	/// </param>
	void passOrCoverPlate(GameObject btn)
	{
		setBtnBox(btn);
		//senderCardsAnimation();
	}
	/// <summary>
	/// Passes the plate.
	/// </summary>
	/// <param name='btn'>
	/// Button.
	/// </param>
	void passPlate(GameObject btn)
	{
		setBtnBox(btn);

	}
	
	/// <summary>
	/// Frees the call.
	/// </summary>
	/// <param name='btn'>
	/// Button.
	/// </param>
	void FreeCall(GameObject btn)
	{
		setBtnBox(btn);

	}
	
	
	
	
	/// <summary>
	/// Filling the specified btn.
	/// </summary>
	/// <param name='btn'>
	/// Button.
	/// </param>
	void Filling(GameObject btn)
	{
		Transform greenbtnTrs=transform.FindChild("Button_GreenBtn");
		Transform sliderTrs=transform.FindChild("sliderchip");

		greenbtnTrs.gameObject.active=true;
		sliderTrs.gameObject.active=true;
		greenbtnTrs.gameObject.SetActiveRecursively(true);
 		sliderTrs.gameObject.SetActiveRecursively(true);
		DisableAllChips();
	}
 
	void DisableAllChips()
	{
 		Transform sliderTrs=transform.FindChild("sliderchip");
 		
		Transform quanjin=sliderTrs.transform.FindChild("QuanjinStarAnim");
		Transform Quanjinpic=sliderTrs.transform.FindChild("Quanjin (GIFWhole_pic)");
		if(quanjin && Quanjinpic)
		{
		//	Quanjinpic.gameObject.active=false;
			Quanjinpic.gameObject.SetActiveRecursively(false);
		  //  quanjin.gameObject.active=false;
		    quanjin.gameObject.SetActiveRecursively(false);
		}
		
	}
	
	
	void showAllChips()
	{
		 
		 
		 
	       Transform sliderTrs=transform.FindChild("sliderchip");
 		   UISlider sld=sliderTrs.GetComponent<UISlider>();
		   if(sld.sliderValue == 1)
		   {
			
			Transform quanjin=sliderTrs.transform.FindChild("QuanjinStarAnim");
		    Transform Quanjinpic=sliderTrs.transform.FindChild("Quanjin (GIFWhole_pic)");
		    if(quanjin && Quanjinpic)
		    {
			   Quanjinpic.gameObject.active=true;
			   Quanjinpic.gameObject.SetActiveRecursively(true);
		       quanjin.gameObject.active=true;
		      quanjin.gameObject.SetActiveRecursively(true);
		    }
		   }
		   else
			 DisableAllChips();
	    	
		 
 		
		
	}
	
	
	void cardRoation2complete()
	{
		
	}
	
	/// <summary>
	/// Chips the be sure button.
	/// </summary>
	/// <param name='btn'>
	/// Button.
	/// </param>
	void ChipBeSureBtn(GameObject btn)
	{
		
		
		Transform greenbtnTrs=transform.FindChild("Button_GreenBtn");
		Transform sliderTrs=transform.FindChild("sliderchip");
		
		greenbtnTrs.gameObject.active=false;
		sliderTrs.gameObject.active=false;
		greenbtnTrs.gameObject.SetActiveRecursively(false);
		
		sliderTrs.gameObject.SetActiveRecursively(false);

	}
	
	
	
}
