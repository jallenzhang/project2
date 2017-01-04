using UnityEngine;
using System.Collections;
using DataPersist;
using AssemblyCSharp;
using DataPersist.CardGame;
using LilyHeart;

public class GamblingAnimation : MonoBehaviour {
	
	
 	public GameObject card1;
	public GameObject card2;
	public GameObject card3;
	public GameObject card4;
	public GameObject card5;
	
	int count=5;
 	int cardnum=1;
	int currentcard=1;
	
	int cardfinishedsender=0;
	
 	
       
    void RotationCard()
	{
		count--;
	    cardRotation(card1);
	}
	
	
	
	void cardFaceRoationcomplete()
	{
		count--;
 		switch(5-count)
		{
		    case 2:
		       cardRotation(card2);
			break;
			 case 3:
		        cardRotation(card3);
		    break;
			  case 4:
			   cardRotation(card4);
			break;
			 case 5:
			 cardRotation(card5);
			break;
 		}
 	}
	 
	void cardBackRoationcomplete(Transform face)
	{
		iTween.RotateTo(face.gameObject,iTween.Hash("rotation",new Vector3(0,0,0),"time",0.15f,"easetype","linear","oncomplete","cardFaceRoationcomplete","oncompletetarget",gameObject));
	}
	
	void cardRotation(GameObject card)
	{
		Transform[] trs=card.GetComponentsInChildren<Transform>();
		Transform Cardface=null;
		Transform back=null;
		
		foreach(Transform tr in trs)
		{
			if(tr.name=="SpriteFace")
			{
				Cardface=tr;
			}
			
			if(tr.name=="SpriteBack")
			{
				back=tr;
			}
		} 
		if(Cardface !=null)
		{
			iTween.RotateTo(back.gameObject,iTween.Hash("rotation",new Vector3(0,90,0),"time",0.15f,"easetype","linear","oncomplete","cardBackRoationcomplete","oncompletetarget",gameObject,"oncompleteparams",Cardface));
		} 

	}
	 
	
	void senderCardFinished(GameObject card)
	{
		{
			Transform role = transform.Find("RoleInfo_" + currentcard);
			if(role)
			{
 				Transform  smallcard=getTheCard(role,currentcard,cardnum); 
				smallcard.gameObject.active=true;
  				Destroy(card);
			}
		}
		if(currentcard==1)
			   BigCardSenderFinished();
		
		if(cardnum>2)
			 return;
		
		TableInfo info=Room.Singleton.PokerGame.TableInfo;
		Debug.Log("info.Players.Count:"+info.Players.Count);
		if(cardfinishedsender<=info.Players.Count)
		{
 		    while(true)
 			{
				currentcard++;
			    if(currentcard>9)
 					currentcard=1;
 			  
 				Transform role = transform.Find("RoleInfo_" + currentcard);
				if(role.gameObject.active==true)
				{
		 			 Transform  smallcard=getTheCard(role,currentcard,cardnum);
		 			 senderCards(smallcard.gameObject);
					 cardfinishedsender++;
				     break;
	 	 		 }
	         }
   			
		}
		else
		{
		 
			cardnum++;
			senderCardsAnimation();
		}
	}
	
	
	/// <summary>
	/// Cards the roation1complete.
	/// </summary>
	/// <param name='nextone'>
	/// Nextone.
	/// </param>
    void cardRoation1complete(Transform nextone)
	{
		if(nextone.name=="BigCardFace1")
 		    iTween.RotateTo(nextone.gameObject,iTween.Hash("rotation",new Vector3(0,0,8),"time",0.5f,"easetype","linear","oncomplete","cardRoation2complete","oncompletetarget",gameObject));
		else 
            iTween.RotateTo(nextone.gameObject,iTween.Hash("rotation",new Vector3(0,0,-9),"time",0.5f,"easetype","linear","oncomplete","cardRoation2complete","oncompletetarget",gameObject));

	}
	
 	
	/// <summary>
	/// Bigs the card sender finished.
	/// </summary>
	void BigCardSenderFinished()
	{
		
		Transform  role = transform.Find("RoleInfo_1");
		Transform backcard=role.transform.FindChild("BigCardBack"+cardnum);  
 	    Transform facecard=role.transform.FindChild("BigCardFace"+cardnum);  
		facecard.gameObject.active=true;
   		if(backcard!=null && facecard!=null)
		{
			if(backcard.name=="BigCardBack1")
			{
 			   iTween.RotateTo(backcard.gameObject,iTween.Hash("rotation",new Vector3(0,90,8),"time",0.5f,"easetype","linear","oncomplete","cardRoation1complete","oncompletetarget",gameObject,"oncompleteparams",facecard));
 			}
			else  
			{
 				iTween.RotateTo(backcard.gameObject,iTween.Hash("rotation",new Vector3(0,90,-9),"time",0.5f,"easetype","linear","oncomplete","cardRoation1complete","oncompletetarget",gameObject,"oncompleteparams",facecard));			
 			}
		}

	}
	
	/// <summary>
	/// Senders the cards.
	/// </summary>
	/// <param name='body'>
	/// Body.
	/// </param>
 	void senderCards(GameObject body)
	{
 		GameObject smallCard=createCard(currentcard);
 		iTween.MoveTo(smallCard,iTween.Hash("position",body.transform.localPosition,"time",0.15f,"looptype","none","easetype","easeInOutQuad","islocal",true,"oncomplete","senderCardFinished","oncompletetarget",gameObject,"oncompleteparams",smallCard));

		if(cardnum==1)
  			iTween.RotateTo(smallCard.gameObject,iTween.Hash("rotation",new Vector3(0,0,25),"time",0.15f,"looptype","none","easetype","easeInOutQuad"));
 	 	else
			iTween.RotateTo(smallCard.gameObject,iTween.Hash("rotation",new Vector3(0,0,0),"time",0.15f,"looptype","none","easetype","easeInOutQuad"));
		
	 	if(currentcard==1)
	    	iTween.ScaleTo(smallCard.gameObject,iTween.Hash("scale",new Vector3(90,116,0),"time",0.15f,"looptype","none","easetype","easeInOutQuad"));

	}
	
	GameObject createCard(int count)
	{
		GameObject prefab=Resources.Load(count==1?"prefab/orignBigCard":"prefab/orignCard") as GameObject;
		GameObject smallCard=Instantiate(prefab,new Vector3(0,130.0f,0f),Quaternion.identity) as GameObject;
		smallCard.transform.parent=transform;
		smallCard.transform.localScale=   new Vector3(44,51,1);
		smallCard.transform.localPosition = new Vector3(0,130,-1);
 		 
 		UISprite widget = smallCard.GetComponent<UISprite>();
		Color mColor=widget.color;
		mColor.a=0;
 		widget.color=mColor;
		
		mColor.a=1;
 		TweenColor.Begin(smallCard, 0.1f, mColor);
		return smallCard;
	}
	
	/// <summary>
	/// Gets the card.
	/// </summary>
	/// <returns>
	/// The the card.
	/// </returns>
	/// <param name='parent'>
	/// Parent.
	/// </param>
	/// <param name='name'>
	/// Name.
	/// </param>
	Transform getTheCard(Transform parent,int count,int cardn)
	{
		Transform child=null;
		if(count==1)
 		   child=parent.FindChild("BigCardBack"+cardn);  
		else
		   child=parent.FindChild("smalCard"+cardn);  
		
		return child;
	}
	
	/// <summary>
	/// Senders the cards animation.
	/// </summary>
	public void senderCardsAnimation()
	{
 		   TableInfo info=Room.Singleton.PokerGame.TableInfo;
		    currentcard=(info.NoSeatSmallBlind+1);
		    cardfinishedsender=1;
  		
		    Transform role = transform.Find("RoleInfo_"+currentcard);
			if(role)
			{
			      Transform  smallcard=getTheCard(role,currentcard,cardnum);
			      if(smallcard)
				  {
	 				  senderCards(smallcard.gameObject);
				  }
   		    }
  	}
 	
	/// <summary>
	/// Fadeins the out widts.
	/// </summary>
	void fadeOutWidt(GameObject target)
	{
		  UIWidget widget = target.GetComponent<UIWidget>();
		  Color mColor=widget.color;
		  mColor.a=0;
 		  TweenColor.Begin(target, 0.1f, mColor);
	}
	/// <summary>
	/// Fadeins the widts.
	/// </summary>
	/// <param name='target'>
	/// Target.
	/// </param>
	void fadeinWidt(GameObject target)
	{
		  UIWidget widget = target.GetComponent<UIWidget>();
		  Color mColor=widget.color;
		  mColor.a=1;
 		  TweenColor.Begin(target, 0.1f, mColor);
 
	}
	
	
	/// <summary>
	/// Getchips the specified parent.
	/// </summary>
	/// <param name='parent'>
	/// Parent.
	/// </param>
 	Transform getclonechips(Transform parent)
	{
 		Transform chip=parent.transform.FindChild("Spritechip");
 		GameObject clonechip=Instantiate(chip.gameObject,new Vector3(0,0,0) ,Quaternion.identity) as GameObject;
		clonechip.transform.parent=chip.parent;
		clonechip.transform.localScale=new Vector3(34,40,1) ;
		clonechip.transform.localPosition=chip.localPosition;
		chip.gameObject.active=false;
		return  clonechip.transform;
	}
	
	Transform getlabchip(Transform parent)
	{
 		Transform chip=parent.transform.FindChild("spritelabchip");
 		return  chip;
	}
 	
	/// <summary>
	/// Gets the chip parent role.
	/// </summary>
	/// <returns>
	/// The chip parent role.
	/// </returns>
	/// <param name='index'>
	/// Index.
	/// </param>
	Transform getChipParentRole(int index)
	{
	   
		Transform role = transform.Find("RoleInfo_"+index);
		return role;
 	}
	
	/// <summary>
	/// Senders the card finished.
	/// </summary>
	/// <param name='obj'>
	/// Object.
	/// </param>
	/// int
 
	void senderChipFinished(Transform obj)
	{
 		Destroy(obj.gameObject);
		
		//fadeinOutWidts(obj[1].gameObject);
 	}
	
	/// <summary>
	/// Senderchips this instance.
	/// </summary>
	void  SenderChipsAnimation()
	{
		for(int i=1;i<=9;i++)
		{
			Transform clonechip =getclonechips(getChipParentRole(i));
  			Transform labchip =getlabchip(getChipParentRole(i));
			
 			fadeOutWidts(labchip.gameObject);
			if(clonechip!=null)
			    iTween.MoveTo(clonechip.gameObject,iTween.Hash("position",new Vector3(0,160.3491f,0) ,"time",0.4f,"looptype","none","easetype","easeInOutQuad","islocal",true,"oncomplete","senderChipFinished","oncompletetarget",gameObject,"oncompleteparams",clonechip));
		} 
 	}
	
	/// <summary>
	/// Fadeins the out widts.
	/// </summary>
	void fadeOutWidts(GameObject target)
	{
		
		Transform[] trs=target.GetComponentsInChildren<Transform>();
		 foreach(Transform tr in trs)
		 {
		    UIWidget widget=tr.GetComponent<UIWidget>();
			Color mColor=widget.color;
			mColor.a=0;
		    TweenColor.Begin(tr.gameObject, 0.2f, mColor);
		  }
		  
	}
	/// <summary>
	/// Fadeins the widts.
	/// </summary>
	/// <param name='target'>
	/// Target.
	/// </param>
	void fadeinWidts(GameObject target)
	{
		
	 
	}
	
	void resetRolesCard()
	{
		for(int i=1;i<=9;i++)
		{
			Transform role = transform.Find("RoleInfo_"+i);
			if(role)
			{ 
			      Transform  smallcard1=getTheCard(role,i,1);
			      Transform  smallcard2=getTheCard(role,i,2);
 				 
				if(i==1)
				{
					Transform face1=role.FindChild("BigCardFace1");   
					Transform face2=role.FindChild("BigCardFace2");  
					face1.localRotation=Quaternion.Euler(0,90,8);
					face2.localRotation=Quaternion.Euler(0,90,-9);
					smallcard1.localRotation= Quaternion.Euler(0, 0, 8);
					smallcard2.localRotation= Quaternion.Euler(0, 0, -9);
     
				} 
				smallcard1.gameObject.active=false;
			    smallcard2.gameObject.active=false;
				 
				
  		    }
		}
		 cardnum=1;
	     currentcard=1;
	}
	
 	// Use this for initialization
	void Start () {
	
	}
 	// Update is called once per frame
	void Update () {
	
	}
}
