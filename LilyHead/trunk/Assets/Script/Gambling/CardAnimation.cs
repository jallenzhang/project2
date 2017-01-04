using UnityEngine;
using System.Collections;
using DataPersist;
 using AssemblyCSharp;
using LilyHeart;

public class CardAnimation : MonoBehaviour {

	// Use this for initialization
	public AudioSource AudioSource {get;set;}
	void playsound()
	{
				SoundHelper.PlaySound("Music/Other/PublicCardsOpen",AudioSource,0);
	
	}
	void SetCardValue()
	{
	    TableInfo infor = Room.Singleton.PokerGame.TableInfo;
		if(infor!=null)
		{
		    PlayerInfo iteminfo= infor.GetPlayer(User.Singleton.UserData.NoSeat);
			if(iteminfo!=null)
			{
				Transform BigCardFace1=transform.FindChild("BigCardFace1");
				Transform BigCardFace2=transform.FindChild("BigCardFace2");
				if(BigCardFace1 ==null || BigCardFace2==null)
				{
					Destroy(gameObject);
				}
				else
				{
					BigCardFace1.GetComponent<UISprite>().spriteName=iteminfo.Cards[0].ToString().ToLower();
					BigCardFace2.GetComponent<UISprite>().spriteName=iteminfo.Cards[1].ToString().ToLower();
				}
				
				Invoke("playsound",0.35f);
			}
		}
	}
	void Awake()
	{   
		AudioSource=gameObject.AddComponent<AudioSource>();
	}
	void Start () {
	
		SetCardValue();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
