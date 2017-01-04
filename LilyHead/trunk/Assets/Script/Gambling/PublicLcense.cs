using UnityEngine;
using System.Collections;
using DataPersist;
using AssemblyCSharp;
using LilyHeart;

public class PublicLcense : MonoBehaviour {

	// Use this for initialization
	
	public float delayTime=0.5f;
	public float fadeInTime=0.5f;
	public float animationFaceRotatetime=0.15f;
	public float animationBackRotatetime=0.15f;
	public Vector3 FaceCardRotateVector=new Vector3(0,0,0); 
	public Vector3 BackCardRotateVector=new Vector3(0,90,0); 
	public int index=1;
	
	public GameObject SpriteBackCard;
	public GameObject SpriteFaceCard;
	
	const float zipx=10;
	public Vector3 locationPosition1=new Vector3(-168,31,zipx);
	public Vector3 locationPosition2=new Vector3(-79,31,zipx);
	public Vector3 locationPosition3=new Vector3(8,31,zipx);
	public Vector3 locationPosition4=new Vector3(95,31,zipx);
	public Vector3 locationPosition5=new Vector3(183,31,zipx);
	public AudioSource AudioSource {get;set;}
	
	void Start () {
	
		fadeinWidtWithTimes(gameObject,fadeInTime);
		switch(index)
		{
			case 1:
					transform.localPosition=locationPosition1;
			 	break;
			case 2:
					transform.localPosition=locationPosition2;
			 	break;
			case 3:
					transform.localPosition=locationPosition3;
			 	break;
			case 4:
					transform.localPosition=locationPosition4;

				break;
			case 5:
					transform.localPosition=locationPosition5;
				break;
		}
		if(delayTime==-1)
		{
			Destroy(SpriteBackCard);
			SpriteFaceCard.transform.localRotation=Quaternion.Euler(0,0,0);
			TableInfo info=Room.Singleton.PokerGame.TableInfo;
			if(info!=null)
			{
				int tempindex=index-1;
				if(index-1<0)
					tempindex=0;
				
				if(info.Cards!=null)
				{
					SpriteFaceCard.GetComponent<UISprite>().spriteName=info.Cards[tempindex].ToString().ToLower();
				}
 			}
		}
		//StartCoroutine(BackCardRotation(delayTime));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void Awake()
	{   
		AudioSource=gameObject.AddComponent<AudioSource>();
		fadeOutWidts(gameObject);
			//	PhotonClient.GotoBackgroundSceneEvent += GotoRoom;

	}
	
	void fadeOutWidts(GameObject target)
	{
		
		Transform[] trs=target.GetComponentsInChildren<Transform>();
		foreach(Transform tr in trs)
		{
		    UIWidget widget=tr.GetComponent<UIWidget>();
			if(widget!=null)
			{
				Color mColor=widget.color;
				mColor.a=0;
		    	widget.color=mColor;
			}
		}
		
 	}
	void OnDestroy()
	{
		//PhotonClient.GotoBackgroundSceneEvent -= GotoRoom;
		//iTween.Stop();
		//Debug.LogWarning("OnDes  "+index);
		CancelInvoke();
 	}
//	void GotoRoom()
//	{
//		 
//		iTween.Stop();
//	}
	public void close()
	{
		Destroy(gameObject);
	}
	void FadeinCallWhenFinished()
	{
		if(delayTime!=-1)
			StartCoroutine(BackCardRotation(delayTime));
	}
	void fadeinWidtWithTimes(GameObject target,float time)
	{
 
		TweenColor colow=null;
		Transform[] trs=target.GetComponentsInChildren<Transform>();
		 foreach(Transform tr in trs)
		 {
		    UIWidget widget=tr.GetComponent<UIWidget>();
			if(widget)
			{
				Color mColor=widget.color;
 				mColor.a=1;
 			    colow=TweenColor.Begin(tr.gameObject, time, mColor);
			}
		  }
		
 		colow.eventReceiver=gameObject;
		colow.callWhenFinished="FadeinCallWhenFinished";
 	}
 	 

	void BigCardFaceRoationcomplete()
	{
        if(transform.name=="PublicCard3" ||transform.name=="PublicCard4" ||transform.name=="PublicCard5")
		{
 			transform.parent.FindChild("wintitlebg").GetComponent<WinAndActionTitle>().fadeOutgameobject();
 		}
   	}
	 
	void BigCardBackRoationcomplete()
	{
		//Debug.Log("BigCardBackRoationcomplete");
		SoundHelper.PlaySound("Music/Other/PublicCardsOpen",AudioSource,0);
 		iTween.RotateTo(SpriteFaceCard,iTween.Hash("rotation",new Vector3(FaceCardRotateVector.x,FaceCardRotateVector.y,FaceCardRotateVector.z),"time",animationFaceRotatetime,"easetype","linear","oncomplete","BigCardFaceRoationcomplete","oncompletetarget",gameObject));
	}
	
    IEnumerator BackCardRotation(float time)
	{
		yield return new WaitForSeconds(time);
 		 
    
		TableInfo infor=Room.Singleton.PokerGame.TableInfo;
		
		if(infor == null)
			yield return 0;
		
		
		int tempindex=index-1;
		if(index-1<0)
			tempindex=0;
			
		if(infor.Cards!=null)
		{
			string carvalue=infor.Cards[tempindex].ToString().ToLower();
 			SpriteFaceCard.GetComponent<UISprite>().spriteName=carvalue;
 	  		iTween.RotateTo(SpriteBackCard,iTween.Hash("rotation",new Vector3(BackCardRotateVector.x,BackCardRotateVector.y,BackCardRotateVector.z),"time",animationBackRotatetime,"easetype","linear","oncomplete","BigCardBackRoationcomplete","oncompletetarget",gameObject));
 		}
 	}
}
