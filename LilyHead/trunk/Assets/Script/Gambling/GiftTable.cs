using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using DataPersist;
using LilyHeart;

public class GiftTable : TF_ParamScript {
	
	public GameObject UIGrid;
	void CloseView()
	{
		Destroy(gameObject);
	}
	void BuySomeOne()
	{
		
		int Noseat= -1;
		if(Util.isMatch())
		{
			Noseat=MatchInforController.Singleton.transform.FindChild(transform.name).GetComponent<PockerPlayer>().NoSeat;

		}
		else
			Noseat=ActorInforController.Singleton.transform.FindChild(transform.name).GetComponent<PockerPlayer>().NoSeat;
		if(Noseat!=-1)
		{
			TableInfo info=Room.Singleton.PokerGame.TableInfo;
			if(info!=null)
			{
				PlayerInfo playinf=info.GetPlayer(User.Singleton.UserData.NoSeat);
				if(playinf!=null)
				{
		 	  		GiftGrid giftgrid=UIGrid.GetComponent<GiftGrid>();
		 			 
	 	 			int Id =  giftgrid.Id;
					Debug.Log(Id);
					Gift gift =GiftGrid.GiftDictionary[Id];
					
				    Debug.LogWarning(gift.price+";"+playinf.MoneySafeAmnt);
					if(gift.price<playinf.Chips)
					{ 
					  	 Debug.Log(gift.price); 
 					 	User.Singleton.SendGift(Noseat,Id);
					}
					else
					{
						GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new BuyIndDialog());
						PopUpTips();
					}
					
					CloseView();
				}
			}
		}
	 
 	}
	void PopUpTips()
	{
		
		Transform tips2=transform.parent.FindChild("tips2");
		if(tips2==null)
		{
 			GameObject prefab=Resources.Load("prefab/Gambling/tips2") as GameObject;
			GameObject item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
 			item.transform.parent=transform.parent;
			item.name="tips2";
			item.transform.localPosition=new Vector3(0,200,-6);
			item.transform.localScale =new Vector3(1,1,1);
			
		}
		
	}
	void BuyToAll()
	{
  		GiftGrid giftgrid=UIGrid.GetComponent<GiftGrid>();
		 
		int Id =  giftgrid.Id;
		
		Gift gift =GiftGrid.GiftDictionary[Id];
		TableInfo info=Room.Singleton.PokerGame.TableInfo;
		PlayerInfo playinf=info.GetPlayer(User.Singleton.UserData.NoSeat);
		if((gift.price)*(info.Players.Count)<playinf.Chips)
		{ 
 		 	  User.Singleton.SendGift(Id);
		}
		else
		{
			GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new BuyIndDialog());
			PopUpTips();
		}
		CloseView();
	
 	}
    protected override void DealWithStringParam()
	{
		transform.name=tf_strParam;
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
