using UnityEngine;
using System.Collections;
using DataPersist;
using AssemblyCSharp;
using System.Collections.Generic;
using LilyHeart;

public class FriendPlayerInfo : MonoBehaviour {
	
	private static float addFriendTime=-1;
	
	string userId=string.Empty;
	
	int Noseat=-1;
	bool friendDisEnabled=false;
 	// Use this for initialization
	void Start () {
  		
	}
	// Update is called once per frame
	void Update () {
	
	}
	
	void LateUpdate()
	{
		if(addFriendTime!=-1 )
		{
			if(Time.realtimeSinceStartup - addFriendTime>10)
			{
				if(friendDisEnabled==false)
				{
   					transform.FindChild("Button_red").GetComponent<BoxCollider>().enabled=true;
 				}
				addFriendTime=-1;
 			}
 		}
	}
  
	
	void BtnClose()
	{
		if(Util.isMatch())
			MatchInforController.Singleton.DisAbleAllButtons(true);
		else
			ActorInforController.Singleton.DisAbleAllButtons(true);

		Destroy(gameObject);
	}
	void toAddFriend(GameObject btn)
	{
  		BtnClose();
	 
		btn.GetComponent<BoxCollider>().enabled=false;
 		addFriendTime=Time.realtimeSinceStartup;
		
		User.Singleton.RequestFriend(userId);
 		
	}
	 
    void SetLabelValue(Transform label,string val)
	{
		//Debug.LogWarning(label.name);
 		UILabel uilabel=label.GetComponent<UILabel>();
		uilabel.text=val;
		
		Transform subLabel=label.FindChild(label.name);
		if(subLabel!=null)
		{
			UILabel subuilabel=subLabel.GetComponent<UILabel>();
			subuilabel.text=val;
		}
	}
	void sendGit(GameObject btn)
	{
		if(Util.isMatch())
		{
			MatchInforController.Singleton.DisAbleAllButtons(true);
			MatchInforController.Singleton.ShowGiftView(btn);
		}
		else
		{
			ActorInforController.Singleton.DisAbleAllButtons(true);
			ActorInforController.Singleton.ShowGiftView(btn);
		}
 		Destroy(btn.transform.parent.gameObject);
 	}
	public void OnInitDetailInformation(PlayerInfo iteminfor)
	{
		TableInfo infor=Room.Singleton.PokerGame.TableInfo;
		if(infor==null)
			return;
 				
		string picstr =HonorHelper.GetHonorPicString(HonorHelper.GetHonorRecuise(iteminfor as UserData,HonorType.Citizen),iteminfor.Avator);
 		SetLabelValue(transform.FindChild("title"),UtilityHelper.GetHonorName(iteminfor as UserData));
		UISprite TitleIcon=transform.FindChild("TitleIcon").GetComponent<UISprite>();
		TitleIcon.spriteName= picstr;
 		
   		userId=(iteminfor as UserData).UserId;
		
		if(Time.realtimeSinceStartup - addFriendTime>10)
		{
 			addFriendTime=-1;
		}
		//remove UserType.Guest add friend limit
 		if(addFriendTime !=-1 )/*|| User.Singleton.UserData.UserType == UserType.Guest || (iteminfor as UserData).UserType==UserType.Guest)*/
 		{
  			 GrayButton("Button_red");
 		} 
		
		
//		PlayerInfo playitem=infor.GetPlayer(iteminfor.NoSeat);
//		if(playitem!=null)
//		{
//			//remove UserType.Guest friend limit
//	 		if( playitem.NoSeat!= User.Singleton.UserData.NoSeat)//User.Singleton.UserData.UserType!=UserType.Guest &&
//			{
//				if(((Player)User.Singleton).Friends!=null)
//				{
//	 				foreach(UserData user in  ((Player)User.Singleton).Friends)
//					{
//						if((playitem as UserData).UserId==user.UserId || playitem.Avator == (byte)PlayerAvator.Guest)
//						{
//							GrayButton("Button_red");
//							friendDisEnabled=true;
//							break;
//						}
//					}
//				}
//			}
//			else
//			{
//				// makeFriend.gameObject.SetActiveRecursively(false);
//	 		}
//			
//			=playitem.NoSeNoseatat;
//		}
		
		Noseat=iteminfor.NoSeat;
		if(User.Singleton.Friends!=null)
		{
			foreach(UserData user in  (User.Singleton.Friends))
			{
				if((iteminfor as UserData).UserId == user.UserId)
				{
					GrayButton("Button_red");
					friendDisEnabled=true;
					break;
				}
			}
		}
 		
		
 		Transform Button_green=transform.FindChild("Button_green");
		Button_green.GetComponent<TF_ButtonMessage>().target=transform.parent.gameObject;
		Button_green.GetComponent<TF_ButtonMessage>().param=transform.name;
		UIButtonMessage Button_greenmess=Button_green.GetComponent<UIButtonMessage>();
		Button_greenmess.functionName="sendGit";
		Button_greenmess.target=gameObject;
		
		if(User.Singleton.UserData.NoSeat==-1  || User.Singleton.UserData.NoSeat==iteminfor.NoSeat)
		{
			GrayButton("Button_red");
			friendDisEnabled=true;
		} 
		
		if(User.Singleton.UserData.NoSeat==-1)
		{
			GrayButton("Button_green");

		}
		
		UserData  userdata=(iteminfor as UserData);
		
		 
 		 
		SetLabelValue(transform.FindChild("name"),iteminfor.Name);
		SetLabelValue(transform.FindChild("level"),"Lv."+iteminfor.Level.ToString());
		Debug.Log("playitem.Chips "+iteminfor.Chips +" "+iteminfor.Name);
		
		SetLabelValue(transform.FindChild("chip"),iteminfor.Chips >= 100 ? string.Format("{0:0,00}", iteminfor.Chips) : iteminfor.Chips.ToString());
  		
		
		for(int i=0;i<6;i++)
		{
			if (((int)userdata.OwnRoomTypes & 1) != 0)
			{
						HightSprite("1");

			}
		     if (((int)userdata.OwnRoomTypes & 2) != 0)
			{
						HightSprite("6");

			}
			 if (((int)userdata.OwnRoomTypes & 4) != 0)
			{
						HightSprite("5");

			}
			 if (((int)userdata.OwnRoomTypes & 8) != 0)
			{
						HightSprite("3");

			}
			 if (((int)userdata.OwnRoomTypes & 16) != 0)
			{
						HightSprite("4");

			}
			 if (((int)userdata.OwnRoomTypes & 32) != 0)
			{
						HightSprite("2");

			}
		 
 		}
 		 
		if(userdata.Jade)
			ChangePic("Pic_dragon","TableBtn_dragon");
 
		if(userdata.LineAge) 
			ChangePic("Pic_hat","TableBtn_hat");
		  
		if(userdata.VIP>0 )
 			ChangePic("Pic_vip","TableBtn_Vip");
 	     
		if(User.Singleton.UserData.VIP==0 || User.Singleton.UserData.NoSeat==iteminfor.NoSeat)
		{
			//if(Util.isMatch())
				GrayButton("Button_out");
		}
		if(Util.isMatch())
				GrayButton("Button_out");
 		Transform role=transform.FindChild("avatar");
		UISprite sprite=role.GetComponent<UISprite>();
		
		Debug.Log(iteminfor.Avator);
		if(iteminfor.Avator==(byte)PlayerAvator.DaHeng)
		{
		     sprite.spriteName="DaHeng";
		}
		else if(iteminfor.Avator==(byte)PlayerAvator.Dalaopo)
		{
			sprite.spriteName="Dalaopo";
		}
		else if(iteminfor.Avator==(byte)PlayerAvator.European)
		{
			sprite.spriteName="European";
		}
		else if(iteminfor.Avator==(byte)PlayerAvator.Luoli)
		{
			sprite.spriteName="Luoli";
		}
		else if(iteminfor.Avator==(byte)PlayerAvator.Songer)
		{
			sprite.spriteName="Songer";	
		}
		else if(iteminfor.Avator==(byte)PlayerAvator.Yitaitai)
		{
			sprite.spriteName="Yitaitai";
		}
		else if(iteminfor.Avator==(byte)PlayerAvator.Qianjing)
		{
			sprite.spriteName="Qianjing";
 		}
		else if(iteminfor.Avator==(byte)PlayerAvator.Guest)
		{
			sprite.spriteName="Guest";
		}
		else if(iteminfor.Avator==(byte)PlayerAvator.Captain)
		{
			sprite.spriteName="Captain";
		}
	 	else if ((iteminfor.Avator == (byte)PlayerAvator.AGe) 
			|| (iteminfor.Avator == (byte)PlayerAvator.Princess)
			|| (iteminfor.Avator == (byte)PlayerAvator.General)
			|| (iteminfor.Avator == (byte)PlayerAvator.Queen))
		{
			sprite.spriteName = ((PlayerAvator)iteminfor.Avator).ToString();
		}
	}
	
	void HightSprite(string picName)
	{
		transform.FindChild("house/"+picName).GetComponent<UISprite>().color=Color.white;
	}
	
	void ChangePic(string pic,string picName)
	{
		transform.FindChild(pic).GetComponent<UISprite>().spriteName=picName;
	} 
	
	
	void GrayButton(string picname)
	{
		Transform Button_green=transform.FindChild(picname);
		Button_green.GetComponent<BoxCollider>().enabled=false;
		Transform backGround=Button_green.FindChild("Background");
		
		if(backGround!=null)
		{
			UISlicedSprite sprites=backGround.GetComponent<UISlicedSprite>();
			sprites.color=Color.gray;
		}
		
	}
	
	void clickOut(GameObject btn)
	{
 		PhotonClient.Singleton.KickPlayer(Noseat);
		BtnClose();
		 
	}
	
	
}
