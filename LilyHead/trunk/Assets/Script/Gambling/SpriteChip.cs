using UnityEngine;
using System.Collections;
using DataPersist;
using AssemblyCSharp;
using AssemblyCSharp.Helper;
using LilyHeart;

public class SpriteChip : MonoBehaviour {
 	
	public float fadeInTime =0.25f; 
	public float fadeOutTime =0.25f; 
	
	public iTween.EaseType  MoveEasyTpe=iTween.EaseType.easeInOutQuad;
	public iTween.EaseType  FromPotMoveEasyTpe=iTween.EaseType.easeInOutQuad;
	
	public  ChipType chitype=ChipType.BetTurnEndChip;
	
	public float moveTime=1.25f;
 	
	public int index=-1;
	public int NoSeat=-1;
	
	private ActorInfor actor;
	
	public float delayerTime=0;
	
	public bool isMoving=false;
	
	const float zpix = 0.0f;
	public Vector3 OrignPosition=new Vector3(7,126,zpix);
	public Vector3 DestinationPosition=new Vector3(0,158,zpix);
	public Vector3 DestinationSpritePosition=new Vector3(-33,123,zpix);
	
 	public Vector3 SpriteChipPosition1=new Vector3(64,-106,zpix);
 	public Vector3 SpriteLabelChipPosition1=new Vector3(114,-105,zpix);
	
	public Vector3 SpriteChipPosition2=new Vector3(-273,-53,zpix);
 	public Vector3 SpriteLabelChipPosition2=new Vector3(-222,-52,zpix);
	
	public Vector3 SpriteChipPosition3=new Vector3(-247,3,zpix);
 	public Vector3 SpriteLabelChipPosition3=new Vector3(-299,5,zpix);
	
	public Vector3 SpriteChipPosition4=new Vector3(-247,63,zpix);
 	public Vector3 SpriteLabelChipPosition4=new Vector3(-298,65,zpix);
	
	public Vector3 SpriteChipPosition5=new Vector3(-120,124,zpix);
 	public Vector3 SpriteLabelChipPosition5=new Vector3(-173,125,zpix);
	
	public Vector3 SpriteChipPosition6=new Vector3(132,125,zpix);
 	public Vector3 SpriteLabelChipPosition6=new Vector3(179,125,zpix);
	
	public Vector3 SpriteChipPosition7=new Vector3(250,64,zpix);
 	public Vector3 SpriteLabelChipPosition7=new Vector3(299,66,zpix);
	
	public Vector3 SpriteChipPosition8=new Vector3(247,3,zpix);
 	public Vector3 SpriteLabelChipPosition8=new Vector3(298,4,zpix);
	
	public Vector3 SpriteChipPosition9=new Vector3(181,-56,zpix);
 	public Vector3 SpriteLabelChipPosition9=new Vector3(232,-55,zpix);
	
	
	public Vector3 WonPosition1=new Vector3(0,-230,-10);
	public Vector3 WonPosition2=new Vector3(-237,-230,-10);
	public Vector3 WonPosition3=new Vector3(-408,-140,-10);
	public Vector3 WonPosition4=new Vector3(-408,68,-10);
	public Vector3 WonPosition5=new Vector3(-140,152,-10);
	public Vector3 WonPosition6=new Vector3(147,152,-10);
	public Vector3 WonPosition7=new Vector3(410,66,-10);
	public Vector3 WonPosition8=new Vector3(410,-140,-10);
	public Vector3 WonPosition9=new Vector3(242,-230,-10);
	 
	
	public Vector3 WonSprtitePosition1=new Vector3(-42,-231,-10);
	public Vector3 WonSprtitePosition2=new Vector3(-277,-231,-10);
	public Vector3 WonSprtitePosition3=new Vector3(-449,-142,-10);
	public Vector3 WonSprtitePosition4=new Vector3(-449,67,-10);
	public Vector3 WonSprtitePosition5=new Vector3(-183,150,-10);
	public Vector3 WonSprtitePosition6=new Vector3(105,153,-10);
	public Vector3 WonSprtitePosition7=new Vector3(363,64,-10);
	public Vector3 WonSprtitePosition8=new Vector3(363,-143,-10);
	public Vector3 WonSprtitePosition9=new Vector3(198,-227,-10);
	
	
	public Vector3 Match_SpriteChipPosition1=new Vector3(64,-106,zpix);
 	public Vector3 Match_SpriteLabelChipPosition1=new Vector3(114,-105,zpix);
	
	public Vector3 Match_SpriteChipPosition2=new Vector3(-273,-53,zpix);
 	public Vector3 Match_SpriteLabelChipPosition2=new Vector3(-222,-52,zpix);
	
	public Vector3 Match_SpriteChipPosition3=new Vector3(-247,3,zpix);
 	public Vector3 Match_SpriteLabelChipPosition3=new Vector3(-299,5,zpix);
	
	public Vector3 Match_SpriteChipPosition4=new Vector3(-247,63,zpix);
 	public Vector3 Match_SpriteLabelChipPosition4=new Vector3(-298,65,zpix);
	
	public Vector3 Match_SpriteChipPosition5=new Vector3(-120,124,zpix);
 	public Vector3 Match_SpriteLabelChipPosition5=new Vector3(-173,125,zpix);
	
 	
	
	public Vector3 Match_WonPosition1=new Vector3(0,-230,-10);
	public Vector3 Match_WonPosition2=new Vector3(-237,-230,-10);
	public Vector3 Match_WonPosition3=new Vector3(-408,-140,-10);
	public Vector3 Match_WonPosition4=new Vector3(-408,68,-10);
	public Vector3 Match_WonPosition5=new Vector3(-140,152,-10);
 	 
 	public Vector3 Match_WonSprtitePosition1=new Vector3(-42,-231,-10);
	public Vector3 Match_WonSprtitePosition2=new Vector3(-277,-231,-10);
	public Vector3 Match_WonSprtitePosition3=new Vector3(-449,-142,-10);
	public Vector3 Match_WonSprtitePosition4=new Vector3(-449,67,-10);
	public Vector3 Match_WonSprtitePosition5=new Vector3(-183,150,-10);
 
	
	public long ChipValues=-1;
	
	private bool[] winners;
	
 	public AudioSource AudioSource {get;set;}
	public void OnInit(long ch,ActorInfor ac,int No,int dex,ChipType type,float delayerTim,bool [] winrs)
	{
		actor=ac;
		ChipValues=ch;
		NoSeat=No;
		index=dex;
		chitype=type;
		delayerTime =delayerTim;
		winners=winrs;
	}
	
	public enum ChipType{
		WonPotChip,
		BetTurnEndChip,
		None,
		 
	}
 
	void playWinSoundAction()
	{
 	   SoundHelper.PlaySound("Music/Other/selfwin",AudioSource,0);
 	}
	void Awake()
	{
 		AudioSource=gameObject.AddComponent<AudioSource>();
	}
	
 	
	void setChipSprite()
	{
	 
 		transform.FindChild("Spritechip").GetComponent<UISprite>().spriteName=Util.GetChipSpriteByChip(ChipValues);
		transform.FindChild("spritelabchip").GetComponent<UISprite>().spriteName= Util.GetChipSpriteLabelByChip(ChipValues);	
 		Util.SetLabelValue(transform.FindChild("spritelabchip"),"lab",Util.getLableMoneyK_MModel(ChipValues));
		
 		
 		Debug.Log(chitype);
		if(ChipType.WonPotChip==chitype)
		{
			Util.fadeOutWidts(gameObject);
 			Invoke("DelayToAction",delayerTime); 
		}
		 
		
	}
	
	void DelayToAction()
	{
		Util.FadeWidtsInWithTime(gameObject);
		Invoke("StartToMove",fadeInTime);
		showWinAnmationTips(actor);
		StartCoroutine(ChangeRoleMoney(1.3f,actor));
		isMoving=true;
		playWinSoundAction();
		DoWonAction();
	}
	
	void DoWonAction()
	{
		TableInfo info=Room.Singleton.PokerGame.TableInfo;
		if(info!=null)
		{
			PlayerInfo infoitem=info.GetPlayer(actor.NoSeat);
			if(infoitem!=null)
			{
				if(HandStrengthHelper.GetCurrentHandStrength(infoitem.Cards,info.Cards).ToString()!="Error")
				{
					string bestCardValue = LocalizeHelper.Translate(HandStrengthHelper.GetCurrentHandStrength(infoitem.Cards,info.Cards).ToString());
					if(Util.isMatch())
					{
						MatchInforController.Singleton.transform.FindChild("wintitlebg").gameObject.SetActiveRecursively(true);
 						Util.FadeWidtsInWithTime(MatchInforController.Singleton.transform.FindChild("wintitlebg").gameObject,0.25f);
						Util.SetLabelValue(MatchInforController.Singleton.transform.FindChild("wintitlebg"),"wintitlebg",bestCardValue); 

					}
					else
					{
						
						ActorInforController.Singleton.transform.FindChild("wintitlebg").gameObject.SetActiveRecursively(true);
						Util.FadeWidtsInWithTime(ActorInforController.Singleton.transform.FindChild("wintitlebg").gameObject,0.25f);
						Util.SetLabelValue(ActorInforController.Singleton.transform.FindChild("wintitlebg"),"wintitlebg",bestCardValue); 

					}
					
				}
 				
//				if(info.Cards[0].ToString()!="--" && info.Cards[3].ToString()!="--" && info.Cards[4].ToString()!="--" && infoitem.Cards[0].ToString()!="--")
//				{
//			    	bool[] cards=Room.Singleton.PokerGame.GetBestFiveCards(infoitem.Cards,info.Cards);
				if(winners!=null)
				{
					if(Util.isMatch())
					{
						MatchInforController.Singleton.ShowWinnerCards(winners,index);
					} 
					else
 						ActorInforController.Singleton.ShowWinnerCards(winners,index);
				}
				//}
				
			}
		}
	}
	
	void StartToMove()
	{
		TheWonTatallChipToMove();
	}
	
	void DestorySelf()
	{
		Destroy(gameObject);
	}
	
	// Use this for initialization
	void Start () {
	
		
		if(chitype==ChipType.WonPotChip)
		{
			Util.fadeOutWidts(gameObject); 
 			transform.FindChild("spritelabchip").localPosition=OrignPosition;
			transform.FindChild("Spritechip").localPosition=DestinationSpritePosition;
 		}
		else
		{
			
		    Vector3 spritechipposition=(Vector3)this.GetType().GetField((Util.isMatch()?"Match_SpriteChipPosition":"SpriteChipPosition")+index).GetValue(this);
		    Vector3 spritelabelchipposition=(Vector3)this.GetType().GetField((Util.isMatch()?"Match_SpriteLabelChipPosition":"SpriteLabelChipPosition")+index).GetValue(this);
			transform.FindChild("Spritechip").localPosition = spritechipposition;
			transform.FindChild("spritelabchip").localPosition = spritelabelchipposition;
		} 
		//
		setChipSprite();
 
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	 
	void FadeinCallWhenFinished()
	{
		
 	}
	void FadeOutCallWhenFinished()
	{
		Destroy(gameObject);
 	}
	/// <summary>
	/// Fadeins the out widts.
	/// </summary>
	void fadeOutWidtsWithTime(GameObject target,float time)
	{
		 
		Transform[] trs=target.GetComponentsInChildren<Transform>();
		foreach(Transform tr in trs)
		{
		    UIWidget widget=tr.GetComponent<UIWidget>();
			if(widget!=null)
			{
				Color mColor=widget.color;
				mColor.a=0;
			    TweenColor.Begin(tr.gameObject, time, mColor);
			}
		}
  	}
	
	void TheMoveChipToWonPotCompleted()
	{
 		 Destroy(gameObject);
	}
	 
	public void TheChipToWonPot()
	{
 		
		iTween.MoveTo(transform.FindChild("Spritechip").gameObject,iTween.Hash("position",DestinationPosition,"time",moveTime,"looptype","none","easetype",MoveEasyTpe,"islocal",true,"oncomplete","TheMoveChipToWonPotCompleted","oncompletetarget",gameObject));
 		fadeOutWidtsWithTime(transform.FindChild("spritelabchip").gameObject,0.25f);
	} 
	 
	
    void TheWonTatallChipToMove()
	{
	     Vector3 DesinationPosition=(Vector3)this.GetType().GetField((Util.isMatch()?"Match_WonPosition":"WonPosition")+index).GetValue(this);
	     Vector3 DesinationPosition1=(Vector3)this.GetType().GetField((Util.isMatch()?"Match_WonSprtitePosition":"WonSprtitePosition")+index).GetValue(this);
		
		 iTween.MoveTo(transform.FindChild("spritelabchip").gameObject,iTween.Hash("position",DesinationPosition,"time",moveTime,"looptype","none","easetype",FromPotMoveEasyTpe,"islocal",true,"oncomplete","TheTotallChipMoveToWinnerCompleted","oncompletetarget",gameObject));
		 iTween.MoveTo(transform.FindChild("Spritechip").gameObject,iTween.Hash("position",DesinationPosition1,"time",moveTime,"looptype","none","easetype",FromPotMoveEasyTpe,"islocal",true));
	}
	
	void TheTotallChipMoveToWinnerCompleted()
	{
		SetRoleName(actor.name);
  		fadeOutWidtsWithTime(gameObject,fadeOutTime);
		Invoke("FadeOutCallWhenFinished",fadeOutTime);
	}
	void DestoryAnmationTips(string name)
	{
	    Transform[] trs=transform.parent.GetComponentsInChildren<Transform>();
		foreach(Transform tr in trs)
		{
			if(tr.name==name)
			{
				Destroy(tr.gameObject);
			}
		}
 	}
	void SetRoleName(string str)
	{
	 
  		Transform name=transform.parent.FindChild("name");
 		UILabel labl=name.GetComponent<UILabel>();
		labl.color=Color.white;
 		labl.text=str;
  	}
	
	IEnumerator ChangeRoleMoney(float tim,ActorInfor ac)
	{
		yield return new WaitForSeconds(tim);
 		if(NoSeat==-1)
			 yield return null;
	 
		Util.SetLabelValue(transform.parent,"Label_money",Util.getLableMoneyK_MModel(ChipValues+Util.GetNumLabelValue(transform.parent,"Label_money")));
 		
	}
	
	void showWinAnmationTips(ActorInfor act)
	{
		
 		//StartCoroutine(ChangeRoleMoney(1.35f,acinfor));
  		SetRoleName(LocalizeHelper.Translate("YINGJIA"));
 		DestoryAnmationTips(transform.name+"WinerTips");
 		
		GameObject prefabobject=Resources.Load("prefab/Gambling/Bling") as GameObject;
		GameObject winningtip=Instantiate(prefabobject,new Vector3(0,0,12),Quaternion.identity) as GameObject;
		winningtip.transform.parent= transform.parent;
		winningtip.name=act.RoleName+"WinerTips";
 		winningtip.transform.localScale=new Vector3(1,1,1);
		
		if(Util.isMatch())
		{
			switch(act.gamblingNo)
			{
				case 1:
					winningtip.transform.localPosition=new Vector3(0,-152,12);
				 	break;
				case 2:
					winningtip.transform.localPosition=new Vector3(-406,-40,12);
	
					break;
				case 3:
					winningtip.transform.localPosition=new Vector3(-180,218,12);
	
					break;
				case 4:
					winningtip.transform.localPosition=new Vector3(187,218,12);
	
					break;
				case 5:
					winningtip.transform.localPosition=new Vector3(406,-40,12);
	
					break;
			}
		}
		else
		{
			switch(act.gamblingNo)
			{
				case 1:
					winningtip.transform.localPosition=new Vector3(0,-152,12);
				 	break;
				case 2:
					winningtip.transform.localPosition=new Vector3(-239,-155,12);
	
					break;
				case 3:
					winningtip.transform.localPosition=new Vector3(-409,-66,12);
	
					break;
				case 4:
					winningtip.transform.localPosition=new Vector3(-409,146,12);
	
					break;
				case 5:
					winningtip.transform.localPosition=new Vector3(-142,226,12);
	
					break;
				case 6:
					winningtip.transform.localPosition=new Vector3(146,226,12);
	
					break;
				case 7:
					winningtip.transform.localPosition=new Vector3(405,142,12);
	
					break;
				case 8:
					winningtip.transform.localPosition=new Vector3(405,-66,12);
	
					break;
				case 9:
					winningtip.transform.localPosition=new Vector3(240,-155,12);
	
					break;
			}
			
		}
		
	}
}
