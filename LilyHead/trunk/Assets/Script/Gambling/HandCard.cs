using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using DataPersist;
using LilyHeart;

public class HandCard : MonoBehaviour {

	// Use this for initialization
	
	public float fadeInTime =0.1f; 
	
	public float moveTime=0.1f;
	public float RotateTime=0.1f;
	
	public bool SmallCard=true;
	public bool isleft=true;
	
	public bool needToSenderCardAnimation=true;
	public string cardvalue=string.Empty;
	
	public float waitForMoveTime=0;
	
	public  iTween.EaseType  MoveEasyTpe=iTween.EaseType.easeInOutQuad;
	public double waitForRotateTime=0.35f;
	public Vector3 smallHandLeftCardRotation=new Vector3(0,0,25);
	public Vector3 smallHandLeftCardScale=new Vector3(44,51,1);
	
	public Vector3 smallHandRightCardRotation=new Vector3(0,0,0);
	public Vector3 smallHandRightCardScale=new Vector3(44,51,1);
	
	public Vector3 BigHandHandLeftCardRotation=new Vector3(0,0,5);
	public Vector3 BigHandHandLeftCardScale=new Vector3(90,116,1);
	
	public Vector3 BigHandHandRightCardRotation=new Vector3(0,0,-11);
	public Vector3 BigHandHandRightCardScale=new Vector3(90,116,1);
	
	public Vector3 roleLeftLocationSmallPosition1=new Vector3(77,-166,0);
	public Vector3 rolerightLocationSmallPosition1=new Vector3(92,-166,0);

	public Vector3 roleLeftLocationPosition1=new Vector3(75,-182,0);
 	public Vector3 roleLeftLocationPosition2=new Vector3(-148,-64,0);
	public Vector3 roleLeftLocationPosition3=new Vector3(-324,-38,0);
	public Vector3 roleLeftLocationPosition4=new Vector3(-324,110,0);
	public Vector3 roleLeftLocationPosition5=new Vector3(-236,169,0); 
	public Vector3 roleLeftLocationPosition6=new Vector3(229,180,0); 
	public Vector3 roleLeftLocationPosition7=new Vector3(312,113,0); 
	public Vector3 roleLeftLocationPosition8=new Vector3(311,-37,0); 
	public Vector3 roleLeftLocationPosition9=new Vector3(130,-61,0); 
 	
	public Vector3 roleRoghtLocationPosition1=new Vector3(107,-188,-1);
	public Vector3 roleRoghtLocationPosition2=new Vector3(-138,-64,-1);
	public Vector3 roleRoghtLocationPosition3=new Vector3(-310,-38,-1);
	public Vector3 roleRoghtLocationPosition4=new Vector3(-309,110,-1);
	public Vector3 roleRoghtLocationPosition5=new Vector3(-222,169,-1); 
	public Vector3 roleRoghtLocationPosition6=new Vector3(244,180,-1); 
	public Vector3 roleRoghtLocationPosition7=new Vector3(324,113,-1); 
	public Vector3 roleRoghtLocationPosition8=new Vector3(324,-37,-1); 
	public Vector3 roleRoghtLocationPosition9=new Vector3(141,-61,-1); 
	
	public Vector3 RoleBigFaceCardPosition1=new Vector3(0,0,0);
	public Vector3 RoleBigFaceCardPosition2=new Vector3(0,0,0);
	public Vector3 RoleBigFaceCardPosition3=new Vector3(0,0,0);
	public Vector3 RoleBigFaceCardPosition4=new Vector3(0,0,0);
	public Vector3 RoleBigFaceCardPosition5=new Vector3(0,0,0);
	public Vector3 RoleBigFaceCardPosition6=new Vector3(0,0,0);
	public Vector3 RoleBigFaceCardPosition7=new Vector3(0,0,0);
	public Vector3 RoleBigFaceCardPosition8=new Vector3(0,0,0);
	public Vector3 RoleBigFaceCardPosition9=new Vector3(0,0,0);
	
	
	public Vector3 Match_roleLeftLocationPosition1=new Vector3(75,-182,0);
 	public Vector3 Match_roleLeftLocationPosition2=new Vector3(-148,-64,0);
	public Vector3 Match_roleLeftLocationPosition3=new Vector3(-324,-38,0);
	public Vector3 Match_roleLeftLocationPosition4=new Vector3(-324,110,0);
	public Vector3 Match_roleLeftLocationPosition5=new Vector3(-236,169,0); 
  	
	public Vector3 Match_roleRoghtLocationPosition1=new Vector3(107,-188,-1);
	public Vector3 Match_roleRoghtLocationPosition2=new Vector3(-138,-64,-1);
	public Vector3 Match_roleRoghtLocationPosition3=new Vector3(-310,-38,-1);
	public Vector3 Match_roleRoghtLocationPosition4=new Vector3(-309,110,-1);
	public Vector3 Match_roleRoghtLocationPosition5=new Vector3(-222,169,-1); 
	
	public Vector3 Match_RoleBigFaceCardPosition1=new Vector3(0,0,0);
	public Vector3 Match_RoleBigFaceCardPosition2=new Vector3(0,0,0);
	public Vector3 Match_RoleBigFaceCardPosition3=new Vector3(0,0,0);
	public Vector3 Match_RoleBigFaceCardPosition4=new Vector3(0,0,0);
	public Vector3 Match_RoleBigFaceCardPosition5=new Vector3(0,0,0);
	
	
	public GameObject BigBackCard;
	public GameObject BigFaceCard;
	
	public int Noseat=-1;
	
	public AudioSource AudioSource {get;set;}
	
	public int index=1;
	
	IEnumerator moveToActor(float tim)
	{
		yield return new WaitForSeconds(tim);
		
		Util.FadeWidtsInWithTime(gameObject,fadeInTime);
		senderCards();
	}
	public void setDetailInfor(int inde,bool left,int No,bool needToSenderCardAnim,string cardval)
	{
	 	index=inde;
		isleft=left;
		Noseat=No;
		needToSenderCardAnimation=needToSenderCardAnim;
		cardvalue=cardval;
	}
	void Start () {
			//Debug.LogWarning(SmallCard +" "+index+" " +Noseat);
 		if(SmallCard == true)
		{  
			//Debug.LogWarning(SmallCard +" "+index+" " +Noseat);
			//Debug.LogWarning(smallHandLeftCardRotation);
			//Debug.LogWarning(smallHandRightCardRotation);
			BigBackCard.GetComponent<UISprite>().spriteName="GIFBackBrand_2";	

			if(isleft==true)
			{
				//
				BigBackCard.transform.localPosition=new Vector3(0,0,0);
				BigFaceCard.transform.localPosition=new Vector3(0,0,0);
				BigBackCard.transform.localRotation=Quaternion.Euler(smallHandLeftCardRotation.x,smallHandLeftCardRotation.y,smallHandLeftCardRotation.z);
				BigBackCard.transform.localScale=smallHandLeftCardScale;
				BigFaceCard.transform.localScale=new Vector3(53,68,0);
 			}
			else
			{
				BigBackCard.transform.localPosition=new Vector3(0,0,-1);
				BigFaceCard.transform.localPosition=new Vector3(0,0,-1);
				BigBackCard.transform.localRotation=Quaternion.Euler(smallHandRightCardRotation.x,smallHandRightCardRotation.y,smallHandRightCardRotation.z);
				BigBackCard.transform.localScale=smallHandRightCardScale;
				BigFaceCard.transform.localScale=new Vector3(53,68,0);

			}
			
  	 	}
		else
		{  	
 
			//Debug.LogError("LLLLL");
			if(isleft==true)
			{
				BigBackCard.transform.localPosition=new Vector3(0,0,0);
				BigFaceCard.transform.localPosition=new Vector3(0,0,0);

				BigBackCard.transform.localRotation=Quaternion.Euler(BigHandHandLeftCardRotation.x,BigHandHandLeftCardRotation.y,BigHandHandLeftCardRotation.z);
				BigBackCard.transform.localScale=BigHandHandLeftCardScale;
 			}
			else
			{
				BigBackCard.transform.localPosition=new Vector3(0,0,-1);
				BigFaceCard.transform.localPosition=new Vector3(0,0,-1);

				BigBackCard.transform.localRotation=Quaternion.Euler(BigHandHandRightCardRotation.x,BigHandHandRightCardRotation.y,BigHandHandRightCardRotation.z); 
				BigBackCard.transform.localScale=BigHandHandRightCardScale;

			}
		}
		if(needToSenderCardAnimation==true)
		{
		
			Util.fadeOutWidts(gameObject);
			StartCoroutine(moveToActor(waitForMoveTime));
		}
		else
		{
			if(isleft)
			{
				
	  			Vector3 locateposition=(Vector3)this.GetType().GetField((TableState.Singleton.gametabletype==TableState.GameTableType.GameTableType_Normal?"roleLeftLocationPosition":"Match_roleLeftLocationPosition")+index).GetValue(this);
 				if(index == 1 && SmallCard==true)
					locateposition=roleLeftLocationSmallPosition1;
				               
				transform.localPosition=locateposition;
 			}
			else
			{
				Vector3 locateposition=(Vector3)this.GetType().GetField((TableState.Singleton.gametabletype==TableState.GameTableType.GameTableType_Normal?"roleRoghtLocationPosition":"Match_roleRoghtLocationPosition")+index).GetValue(this);
				if(index == 1 && SmallCard==true)
					locateposition=rolerightLocationSmallPosition1;
				
                transform.localPosition=locateposition;
			}
			Util.FadeWidtsInWithTime(gameObject);
			
			//Debug.Log(cardvalue);
			//Debug.Log(transform.localRotation);
			if(!string.IsNullOrEmpty(cardvalue))
			{
				Destroy(BigBackCard.gameObject);
				if(!SmallCard)
				{
					if(isleft)
					{
						BigFaceCard.transform.localRotation=Quaternion.Euler(BigHandHandLeftCardRotation.x,BigHandHandLeftCardRotation.y,BigHandHandLeftCardRotation.z);
 					}
					else
					{
						BigFaceCard.transform.localRotation=Quaternion.Euler(BigHandHandRightCardRotation.x,BigHandHandRightCardRotation.y,BigHandHandRightCardRotation.z);
					}
				}
				else{
					
 					Vector3 locateposition=(Vector3)this.GetType().GetField((Util.isMatch()?"Match_RoleBigFaceCardPosition":"RoleBigFaceCardPosition")+index).GetValue(this);
					 BigFaceCard.transform.localPosition=locateposition;
					if(!isleft)
 						BigFaceCard.transform.transform.localPosition =new Vector3(10+locateposition.x,locateposition.y,0);
					
 					BigFaceCard.transform.localRotation=Quaternion.Euler(0,0,0); 
				}
				
				BigFaceCard.GetComponent<UISprite>().spriteName=cardvalue.ToLower();
				
 			}
		}
 		
 		
 	}
	 
	void cardRoation2complete()
	{
		
	}
	void cardRoation1complete()
	{
 
		if(isleft)
 		    iTween.RotateTo(BigFaceCard,iTween.Hash("rotation",new Vector3(0,0,8),"time",0.5f,"easetype","linear","oncomplete","cardRoation2complete","oncompletetarget",gameObject));
		else 
            iTween.RotateTo(BigFaceCard,iTween.Hash("rotation",new Vector3(0,0,-9),"time",0.5f,"easetype","linear","oncomplete","cardRoation2complete","oncompletetarget",gameObject));
 
	}
	/// <summary>
	/// Bigs the card sender finished.
	/// </summary>
	IEnumerator BigCardSenderFinished(double time)
	{
	    yield return new WaitForSeconds((float)time);  
 	    PlayerInfo myitem=Room.Singleton.PokerGame.TableInfo.GetPlayer(User.Singleton.UserData.NoSeat);
		
  	     
		Util.FadeWidtOutWithTime(BigBackCard.transform,0.2f);
		
		UISprite sprite=BigFaceCard.GetComponent<UISprite>();
  
		sprite.spriteName=(isleft?myitem.Cards[0].ToString().ToLower():myitem.Cards[1].ToString().ToLower());
		if(isleft)
		{
  			iTween.RotateTo(BigBackCard,iTween.Hash("rotation",new Vector3(0,90,8),"time",RotateTime,"easetype","linear","oncomplete","cardRoation1complete","oncompletetarget",gameObject));
 		}
		else
			iTween.RotateTo(BigBackCard,iTween.Hash("rotation",new Vector3(0,90,-9),"time",RotateTime,"easetype","linear","oncomplete","cardRoation1complete","oncompletetarget",gameObject));			
  
	}
	
	void senderCardFinished()
	{
 		
		if(!SmallCard && !isleft)
		{			
 			bigHandCardRotate();
 		}
			  
	}
	 
    public void bigHandCardRotate()
	{
		Transform lefthandcard= transform.parent.transform.FindChild("lefthandcard");
		if(lefthandcard!=null)
		{
			//Debug.Log("lefthandcard");
			Destroy(lefthandcard.gameObject);
		}
		GameObject prefab=Resources.Load("prefab/Gambling/cardAnim") as GameObject;
		GameObject cardAnim=Instantiate(prefab,new Vector3(0,0,-12),Quaternion.identity) as GameObject;
			
		cardAnim.transform.parent=transform.parent;
		cardAnim.transform.localScale=new Vector3(1,1,1);
		cardAnim.name="cardAnim";
  		
		//Debug.LogError(Noseat +" " + SmallCard  +" "+ isleft);
		  Destroy(gameObject);
		//StartCoroutine(BigCardSenderFinished(waitForRotateTime));
	}
	
	void senderCards()
	{
 		SoundHelper.PlaySound("Music/Other/senderCards",AudioSource,0);
 		if(isleft)
		{
			//Debug.Log("index +"+index);
  			Vector3 locateposition=(Vector3)this.GetType().GetField((TableState.Singleton.gametabletype==TableState.GameTableType.GameTableType_Normal?"roleLeftLocationPosition":"Match_roleLeftLocationPosition")+index).GetValue(this);
			Debug.Log(locateposition);
			if(index == 1 && SmallCard==true)
				locateposition=roleLeftLocationSmallPosition1;
    			iTween.MoveTo(gameObject,iTween.Hash("position",new Vector3(locateposition.x,locateposition.y,locateposition.z),"time",moveTime,"looptype","none","easetype",MoveEasyTpe,"islocal",true,"oncomplete","senderCardFinished","oncompletetarget",gameObject));
		}
		else
		{
			Vector3 locateposition=(Vector3)this.GetType().GetField((TableState.Singleton.gametabletype==TableState.GameTableType.GameTableType_Normal?"roleRoghtLocationPosition":"Match_roleRoghtLocationPosition")+index).GetValue(this);
			if(index == 1 && SmallCard==true)
				locateposition=rolerightLocationSmallPosition1;
			//Debug.Log(locateposition);
			//Debug.Log(moveTime.ToString());
  			iTween.MoveTo(gameObject,iTween.Hash("position",new Vector3(locateposition.x,locateposition.y,-10),"time",moveTime,"looptype","none","easetype",MoveEasyTpe,"islocal",true,"oncomplete","senderCardFinished","oncompletetarget",gameObject));
		}
 
	}
	
	void Awake()
	{   
		AudioSource=gameObject.AddComponent<AudioSource>();
		if(needToSenderCardAnimation==true)
		{
			Util.fadeOutWidts(gameObject);
	
		}
	}
	 
	public void close()
	{
		Destroy(gameObject);
	}
	 
	 
	
	void RotateSmallCardWithoutRole1Complete()
	{
 					
		Vector3 locateposition=(Vector3)this.GetType().GetField((Util.isMatch()?"Match_RoleBigFaceCardPosition":"RoleBigFaceCardPosition")+index).GetValue(this);
		 BigFaceCard.transform.localPosition=locateposition;
		if(isleft)
		{			
 			iTween.RotateTo(BigFaceCard,iTween.Hash("rotation",new Vector3(0,0,0),"time",0.25f,"looptype",iTween.LoopType.none,"easetype",iTween.EaseType.easeInOutQuart));
  		}
 		else 
		{			
			BigFaceCard.transform.localPosition =new Vector3(10+locateposition.x,locateposition.y,0);
 			iTween.RotateTo(BigFaceCard,iTween.Hash("rotation",new Vector3(0,0,0),"time",0.25f,"looptype",iTween.LoopType.none,"easetype",iTween.EaseType.easeInOutQuart));
		}
	}
	
	IEnumerator DelayerbackbigCard(float tm,Transform BigBackCard)
	{
		yield return new WaitForSeconds(tm);
		
  		iTween.RotateTo(BigBackCard.gameObject,iTween.Hash("rotation",new Vector3(0,-90,0),"time",0.25f,"looptype","none","easetype","easeInOutQuad","oncomplete","RotateSmallCardWithoutRole1Complete","oncompletetarget",gameObject));
	} 
	public void GameOverRotateCard(string cardvalue)
	{
		if(SmallCard)
		{
			//Debug.LogError("GameOverRotateCard");
			Transform[] TRs=transform.parent.GetComponentsInChildren<Transform>(); 
			foreach(Transform tr in TRs)
			if(tr.name==gameObject.name && tr!=transform)
			{
				Destroy(tr.gameObject);
			}
			
 	 		UISprite sprite=BigFaceCard.GetComponent<UISprite>();
	 		sprite.spriteName=cardvalue.ToLower();
 			if(BigBackCard!=null)
			{	
				Util.FadeWidtsOutWithTime(BigBackCard.transform,0.25f);
				StartCoroutine(DelayerbackbigCard(0.15f,BigBackCard.transform));
			}
		}
 	}
	// Update is called once per frame
	void Update () {
	
	}
}
