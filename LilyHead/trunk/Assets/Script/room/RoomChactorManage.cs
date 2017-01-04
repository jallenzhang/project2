using UnityEngine;
using System.Collections;
using System;
using AssemblyCSharp;
using DataPersist;
using DataPersist.CardGame;
using System.Collections.Generic;
using EasyMotion2D;
using System.Timers;
using AssemblyCSharp.Helper;
using LilyHeart;
 
public class RoomChactorManage : MonoBehaviour {
	
	public Transform WalkCamera;
	public Transform FrontseatCamera;
	public Transform BackSeatCamera;
 
	//private List<string> walkList = new List<string>();
	
	void Start () {
		//remove UserType.Guest IN Room Chactor
//		if(User.Singleton.UserData.UserType!=UserType.Guest)
				 Oninit();
		UtilityHelper.RoomDataChangedEvent += CharactorInfoChanged;
  	}
	//LeftSection up-66  down -108  left -288  -154 
	public int GetRandomNum(int min,int max)
    {
 	     System.Random random=new System.Random();
         return random.Next(min,max);
    }
	 
	void RemoveOneEasyMotion(int Noseat)
	{
		 Transform parentTran=null;
		 if(Noseat>=5)
			parentTran=FrontseatCamera;
		 else
			parentTran=BackSeatCamera;
		
		if (parentTran == null)
			return;
		
		Transform easyRole=parentTran.FindChild(Noseat.ToString());
		if(easyRole!=null)
 			Destroy(easyRole.gameObject);
	}
	
	void addOneEasyMotion(PlayerInfo iteminfo)
	{
		Transform parentTran=null;
		if(iteminfo.NoSeat>=5)
			parentTran=FrontseatCamera;
		else
			parentTran=BackSeatCamera;
	
		if (parentTran == null)
		{
			Debug.Log("Here the camera is null");
			return;
		}
			
	    Transform oldrole=parentTran.FindChild(iteminfo.NoSeat.ToString ());
		if(oldrole!=null)
		{
			return;
		}
		
	 //   PlayerInfo iteminfo=Room.Singleton.PokerGame.TableInfo.GetPlayer(Noseat);

	    //Debug.Log("prefab/HallAnimationPrefab/"+parentTran.name+iteminfo.Avator);
		
	    GameObject roleAn=Resources.Load("prefab/HallAnimationPrefab/"+parentTran.name+GetEasyMotionName(iteminfo)) as GameObject;
		GameObject roleColne = null;
		if(roleAn!=null)
		{
		 	roleColne=Instantiate(roleAn,new Vector3(0,0,0),Quaternion.identity) as GameObject;
			roleColne.transform.parent=parentTran;
			roleColne.name=iteminfo.NoSeat.ToString();
			
			roleColne.GetComponent<GameObjectPosition>().index=iteminfo.NoSeat+1;
			roleColne.GetComponent<EasyMotion2D.SpriteRenderer>().depth=(iteminfo.NoSeat>=5?(9-iteminfo.NoSeat):iteminfo.NoSeat); 
		}
	 
	}
 
	void Update () {
	 
		 
	}
	
	string GetEasyMotionName(PlayerInfo playitem)
	{
		    string easymotionName=null;
			if(playitem.Avator==(byte)PlayerAvator.DaHeng)
			{
			     easymotionName="ArbTy";
			}
			else if(playitem.Avator==(byte)PlayerAvator.Dalaopo)
			{
				easymotionName="RusWif";
			}
			else if(playitem.Avator==(byte)PlayerAvator.European)
			{
				easymotionName="EurPrin";
			}
			else if(playitem.Avator==(byte)PlayerAvator.Luoli)
			{
				easymotionName="JapLoli";
			}
			else if(playitem.Avator==(byte)PlayerAvator.Songer)
			{
				easymotionName="BlaSta";
			}
			else if(playitem.Avator==(byte)PlayerAvator.Yitaitai)
			{
				easymotionName="Song";
			}
			else if(playitem.Avator==(byte)PlayerAvator.Qianjing)
			{
				easymotionName="MaGir";
	 		}
			 
			else if(playitem.Avator==(byte)PlayerAvator.Captain)
			{
				easymotionName="Pirate";
			}
			 
			 return easymotionName;
	}
	
 	 
	
	void DestoryWalkAnmaitionEasyMotion(UserData playitem,Transform parentTran)
	{
		if (WalkCamera != null &&  playitem != null)
		{
			Transform roleColne=WalkCamera.FindChild(playitem.UserId);
			if(roleColne!=null)
			{
				Destroy(roleColne.gameObject);
			}
		}
 	}
	
	void GetWalkAnimationEasyMotionResouce(UserData playitem,Transform parentTran) 
	{
 		Transform  Role=null;
 		if(Role==null)
		{
			Debug.Log("playitem");
			Debug.Log("playitem:"+playitem.Avator);
 			string easymotionName=null;
			if(playitem.Avator==(byte)PlayerAvator.DaHeng)
			{
			     easymotionName="ArbTy";
			}
			else if(playitem.Avator==(byte)PlayerAvator.Dalaopo)
			{
				easymotionName="RusWif";
			}
			else if(playitem.Avator==(byte)PlayerAvator.European)
			{
				easymotionName="EurPrin";
			}
			else if(playitem.Avator==(byte)PlayerAvator.Luoli)
			{
				easymotionName="JapLoli";
			}
			else if(playitem.Avator==(byte)PlayerAvator.Songer)
			{
				easymotionName="BlaSta";
			}
			else if(playitem.Avator==(byte)PlayerAvator.Yitaitai)
			{
				easymotionName="Song";
			}
			else if(playitem.Avator==(byte)PlayerAvator.Qianjing)
			{
				easymotionName="MaGir";
	 		}
			 
			else if(playitem.Avator==(byte)PlayerAvator.Captain)
			{
				easymotionName="Pirate";
			}
			else
				return;
			
			Debug.Log("prefab/HallAnimationPrefab/"+parentTran.name+easymotionName);
  			GameObject roleAn=Resources.Load("prefab/HallAnimationPrefab/"+parentTran.name+easymotionName) as GameObject;
			GameObject roleColne=Instantiate(roleAn,new Vector3(0,0,0),Quaternion.identity) as GameObject; 
 			RoomChactor chacor=roleColne.GetComponent<RoomChactor>();
			chacor.EasyMotionName=easymotionName;
			
			//Debug.Log("prefab/HallAnimationPrefab/"+parentTran.name+easymotionName);
			
 			roleColne.transform.parent=parentTran;
			roleColne.name=playitem.UserId;
  			roleColne.transform.localPosition=new Vector3(GetRandomNum(-200,-160),GetRandomNum(-90,-80),0);
 			 
 		}
		 
	}
	void DidLeaveGameEventFinished(UserData user)
	{
		Debug.Log("DidLeaveGameEventFinished:"+user.Name+":"+user.UserId);
		DestoryWalkAnmaitionEasyMotion(user,WalkCamera);
	}
	
	public void Oninit()
	{
		PhotonClient.JoinRoomEvent+=DidJoinRoomEvent;
		PhotonClient.PlayerJoinedEvent+=DidPlayerJoinedEvent;
		PhotonClient.InRoomPlayerLeavedEvent+=DidInRoomPlayerLeavedEvent;
		PhotonClient.LeaveRoomEventFinished+=DidLeaveGameEventFinished;
		PhotonClient.JoinRoomEventFinished+=DidJoinRoomEventFinished;
		
		DidJoinRoomEvent();
		
	}
	public void UnRegseterEvent()
	{
		PhotonClient.PlayerJoinedEvent-=DidPlayerJoinedEvent;
		//PhotonClient.PlayerLeavedEvent-=DidPlayerLeavedEvent;
		PhotonClient.LeaveRoomEventFinished-=DidLeaveGameEventFinished;
		PhotonClient.JoinRoomEventFinished-=DidJoinRoomEventFinished;
		PhotonClient.InRoomPlayerLeavedEvent-=DidInRoomPlayerLeavedEvent;
		PhotonClient.JoinRoomEvent-=DidJoinRoomEvent;
	}
	void OnDestroy()
	{
 		UnRegseterEvent();
		UtilityHelper.RoomDataChangedEvent -= CharactorInfoChanged;
	}
	void DidJoinRoomEventFinished(UserData user)
	{
		Debug.Log("DidJoinRoomEventFinished");
		Transform walk=WalkCamera.FindChild(user.UserId);
		if(walk==null)
		{
			GetWalkAnimationEasyMotionResouce(user,WalkCamera);
		}
		//UpdateAnimation();
	}
	
	public void CharactorInfoChanged()
	{
//		if (Room.Singleton.RoomData.Users != null)
//		{
//			int k = -1;
//			for(int i = 0; i < Room.Singleton.RoomData.Users.Count; i++) 
//			{
//				if (Room.Singleton.RoomData.Users[i].UserId == User.Singleton.UserData.UserId)
//				{
//					k = i;
//					break;
//				}
//			}
//			
//			if (k != -1)
//			{
//				Room.Singleton.RoomData.Users[k] = User.Singleton.UserData;
//				if (WalkCamera != null)
//				{
//					Transform walk=WalkCamera.FindChild(User.Singleton.UserData.UserId);
//					if(walk==null)
//					{
//	 					GetWalkAnimationEasyMotionResouce(User.Singleton.UserData,WalkCamera);
//					}
//					else
//					{
//						DestoryWalkAnmaitionEasyMotion(User.Singleton.UserData,WalkCamera);
//						GetWalkAnimationEasyMotionResouce(User.Singleton.UserData,WalkCamera);
//					}
//				}
//			}
//		}
		Debug.Log("CharactorInfoChanged");
		if(Room.Singleton.RoomData.Users != null)
		{
			for(int i = 0; i < Room.Singleton.RoomData.Users.Count; i++) 
			{
				UserData iUserData = Room.Singleton.RoomData.Users[i];
				if (WalkCamera != null&&iUserData!=null)
				{
					Transform walk=WalkCamera.FindChild(iUserData.UserId);
					if(walk==null)
					{
						GetWalkAnimationEasyMotionResouce(iUserData,WalkCamera);
						//DestoryWalkAnmaitionEasyMotion(iUserData,WalkCamera);
					}
				}
			}
		}
		UpdateAnimation();
	}
	
	void UpdateWalkAnimation()
	{
		if (Room.Singleton.RoomData.Users != null)
		{
			foreach(UserData user in Room.Singleton.RoomData.Users) 
			{
				Debug.Log("user.UserId:"+user.UserId+"user.Name:"+user.Name);
				Debug.Log("user.IsSitting:"+user.IsSitting);
				Debug.Log("user.Avator:"+user.Avator.ToString());
				
				if(!user.IsSitting)
				{
					if (WalkCamera != null)
					{
						Transform walk=WalkCamera.FindChild(user.UserId);
						if(walk==null)
						{
		 					GetWalkAnimationEasyMotionResouce(user,WalkCamera);
						}
//						else
//						{
//							DestoryWalkAnmaitionEasyMotion(user,WalkCamera);
//							GetWalkAnimationEasyMotionResouce(user,WalkCamera);
//						}
					}
					
					Transform parentTran=FrontseatCamera;
					
					for(int noseat=0;noseat<9;noseat++)
					{
						Transform easyRole=parentTran.FindChild(noseat.ToString());
						
						TableInfo info=Room.Singleton.PokerGame.TableInfo;
						if(info!=null)
						{
							PlayerInfo item=info.GetPlayer(noseat);
							if(easyRole!=null&&item!=null)
							{
								DestoryWalkAnmaitionEasyMotion(item as UserData,WalkCamera);
							}
						}
					}
				}
	 		}
		}
	}
	
	void DidJoinRoomEvent()
	{
		Debug.Log("DidJoinRoomEvent");
		PhotonClient.JoinRoomEvent-=DidJoinRoomEvent;
		//Debug.Log(Room.Singleton.RoomData.Users.Count);
		UpdateAnimation();
		TableInfo info=Room.Singleton.PokerGame.TableInfo;
		if(info == null)
		{
			for(int i=0;i<9;i++)
			{
			 	RemoveOneEasyMotion(i);
			}
		}
		//UpdateWalkAnimation();
//	
//		
//		TableInfo info=Room.Singleton.PokerGame.TableInfo;
////	   
////		Debug.Log(Room.Singleton.RoomData.GameType);
//		if(info!=null)
//		{
//		    Debug.Log(info.Name());
//		    Debug.Log(Room.Singleton.RoomData.Owner.UserId);
//			Debug.Log(User.Singleton.UserData.UserId);
//			if (Room.Singleton.RoomData.GameType!=GameType.System) {//&& (User.Singleton.UserData.UserId  == Room.Singleton.RoomData.RoomId)
//				//in self room	
//				UpdateAnimationInfor();
//	 		}
//		}
//		else
//		{
//			for(int i=0;i<9;i++)
//			{
//			 	RemoveOneEasyMotion(i);
//			}
//		}
    		
  	}
	
	void  UpdateAnimationInfor()
	{
		TableInfo info=Room.Singleton.PokerGame.TableInfo;
		if (info != null)
			Debug.Log("Room.Singleton.PokerGame.TableInfo.Name()" + Room.Singleton.PokerGame.TableInfo.Name());
		Debug.Log("Room.Singleton.RoomData.RoomId" + Room.Singleton.RoomData.Owner.UserId);
		
		if (info == null)//|| Room.Singleton.PokerGame.TableInfo.Name() != Room.Singleton.RoomData.Owner.UserId
		{
			for(int i=0;i<9;i++)
			{
			 	RemoveOneEasyMotion(i);
			}
		}
		
		if (info != null)
		{
			Debug.Log("UpdateAnimationInfor"+info.Players.Count);
			for(int noseat=0;noseat<9;noseat++)
			{
				if(info.Players.Count>0)
				{
					PlayerInfo item=info.GetPlayer(noseat);
					if(item!=null)
					{
					    UserData iUserData =  Room.Singleton.RoomData.Users.Find(r=>r.Name == item.Name);
						if(iUserData!=null&&iUserData.Avator!=item.Avator)
							iUserData.Avator = item.Avator;
						addOneEasyMotion(item);
						DestoryWalkAnmaitionEasyMotion((item as UserData),WalkCamera);
						Debug.Log("DestoryWalkAnmaitionEasyMotion");
					}
					else
					{
						RemoveOneEasyMotion(noseat);
					} 
				}
				else
				{
					 RemoveOneEasyMotion(noseat);
				}
			}
		}
	}
	
	void DidPlayerJoinedEvent(int Noseat)
	{
 		Debug.Log("Room DidPlayerJoinedEvent");
		Debug.Log("DidJoinRoomEventFinished");
		TableInfo info=Room.Singleton.PokerGame.TableInfo;
		if(info!=null)
		{
			PlayerInfo item=info.GetPlayer(Noseat);
			UserData user = null;
			if(item != null)
			{
				user = item as UserData;
				Transform walk=WalkCamera.FindChild(user.UserId);
				if(walk!=null)
				{
					DestoryWalkAnmaitionEasyMotion(user,WalkCamera);
				}
			}
		}
		UpdateAnimation();
	}
	void UpdateAnimation()
	{
		Debug.Log("UpdateAnimationUpdateAnimation");
		UpdateWalkAnimation();
		UpdateAnimationInfor();
	}
	void DidInRoomPlayerLeavedEvent()
	{
 		Debug.Log("Room DidPlayerLeavedEvent");
		UpdateAnimation();
//		 RemoveOneEasyMotion(noseat);

	}
	
	                            
}
