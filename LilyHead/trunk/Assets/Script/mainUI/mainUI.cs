using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using DataPersist;
using System.Collections.Generic;
using System;
using DataPersist.CardGame;
using AssemblyCSharp.Helper;
using LilyHeart;

public class mainUI : MonoBehaviour {
	
	private string tRoomId="";
	private string tSearchUsers="";
	private string tPassword="";
	private string tRaiseMoney="";
	private string tBroadcastMessage="";
	private string tBroadcastMessageInTable="";
	private string tNickname="";
	private bool tShowGameSetting=false;
	private string tBigBlind="";
	private string tMaxPlayers="";
	private string tThinkingTime="";
	private bool tOnlyFriend=false;
	private string tLivingRoomType="";
	private string tBuyRoomType="";
	private string tReceiver="";
	private string tChip="";
	private string tFeedback="";
	private string tGiftReceiverNoSeat="";
	private string tGiftType="";
	private string tNoSeat="";
	private string tFriendId="";
	private AudioSource audioSource;
	
	string money = string.Empty;
	string gameNumber = string.Empty;
	string gameWinNumber = string.Empty;
	string matchNumber = string.Empty;
	string level = string.Empty;
	
	private List<UserData> friends=new List<UserData>();
	private List<Message> messages;
	
	private const long gameId = 427916;
	private const string gameGuid = "2f33f47e6b654b98";
	private const string apiKey = "ae710c9ada45455ca8f99830944d78";
	
	public GUIStyle style;
	
	void Awake()
	{
		MusicManager.Singleton.BgAudio=gameObject.AddComponent<AudioSource>();
		audioSource=gameObject.AddComponent<AudioSource>();
	}
	
	// Use this for initialization
	void Start () {
		messages=new List<Message>();
		MusicManager.Singleton.PlayBgMusic();
	}
	
	IEnumerator MySaveFunction()
	{
		var level = new Playtomic_PlayerLevel();
		level.Name = "this is a level";
		level.PlayerName = "Ben";
		level.Data = "Asdfasdfasdfasdf";
		
		yield return StartCoroutine(Playtomic.PlayerLevels.Save(level));
		var response = Playtomic.PlayerLevels.GetResponse("Save");
		
		if(response.Success)
		{
			Debug.Log("Level saved!");
		}
		else
		{
			Debug.Log("Level failed to save because of " + response.ErrorCode + ": " + response.ErrorDescription);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(GlobalManager.Singleton.Messages.Count>0)
		{
			Message message=GlobalManager.Singleton.Messages.Dequeue();
			if(message is PlayerMessage)
			{
				PlayerMessage playerMessage=message as PlayerMessage;
				Debug.Log ("Avator:"+playerMessage.Avator+"NickName"+playerMessage.NickName+"Level:"+playerMessage.Level+"Honor:"+playerMessage.Honor+"Chip:"+playerMessage.Chips);
			}
			messages.Add(message);
		}

		while(Room.Singleton.PokerGame.PlayerActions.Count>0)
		{
			PlayerAction playerAction=Room.Singleton.PokerGame.PlayerActions.Dequeue();
			GlobalManager.Log(playerAction.Id+","+playerAction.NoSeat+","+playerAction.TypeAction);
		}
		StartCoroutine(NetworkUpdate());
		Debug.Log (Time.deltaTime);
	}
	
	IEnumerator NetworkUpdate()
	{
		PhotonClient.Singleton.Update();
		yield return null;
	}
	
	//startGame button immediately
	void OnstartGame()
	{
	}
	
	void OnFriend()
	{
	}
	
	void OnGameRace()
	{
	}
	
	void OnSetting()
	{
	}
	
	void OnShop()
	{
	}
	
	void OnGUI()
	{
		switch(User.Singleton.GameStatus)
		{
//		case GameStatus.Logout:
//			Application.LoadLevel("LaunchTable");
//			break;
//		case GameStatus.Offline:
//			Application.LoadLevel("LoginMain");
//			break;
		case GameStatus.Connected:
		case GameStatus.InRoom:
			//Application.LoadLevel("cardFirend");
			int ty=100;
			foreach(Achievement item in GlobalManager.Singleton.Achievements)
			{
				GUI.Label(new Rect(0,ty+=50,100,30),item.Name,style);
			}
			int y=100;
			int x=300;
			foreach(UserData userData in Room.Singleton.RoomData.Users)
			{
				GUI.Label(new Rect(100,y+=50,200,50),userData.UserId+" "+userData.Password);
			}
			tRoomId=GUI.TextField(new Rect(350,200,200,30),tRoomId);
			if(GUI.Button(new Rect(600,200,150,30),"Join"))
			{
				User.Singleton.JoinRoom(tRoomId);
				//PhotonClient.Singleton.JoinRoom(tRoomId);
			}
			if(GUI.Button(new Rect(300,100,150,30),"Join Game"))
			{
				if(Room.Singleton.RoomData.Owner.UserId==User.Singleton.UserData.UserId)
				{
					tShowGameSetting=true;
				}
				else
				{
					User.Singleton.JoinGame();
				}
			}
			if(tShowGameSetting)
			{
				GUI.Window(0,new Rect(200,150,320,240),windowId=>
				{ 
					GUI.Label(new Rect(30,20,100,30),"BigBlind");
					tBigBlind=GUI.TextField(new Rect(150,20,100,30),tBigBlind);
					GUI.Label(new Rect(30,70,100,30),"MaxPlayer");
					tMaxPlayers=GUI.TextField(new Rect(150,70,100,30),tMaxPlayers);
					GUI.Label(new Rect(30,120,100,30),"ThinkingTime");
					tThinkingTime=GUI.TextField(new Rect(150,120,100,30),tThinkingTime);
					tOnlyFriend=GUI.Toggle(new Rect(30,170,100,30),tOnlyFriend,"OnlyFriend");
					if(GUI.Button(new Rect(100,200,100,30),"Join"))
					{
						User.Singleton.JoinGame(Convert.ToInt32(tBigBlind),Convert.ToInt32(tMaxPlayers),Convert.ToInt32(tThinkingTime),tOnlyFriend);
						tShowGameSetting=false;
					}
					if(GUI.Button(new Rect(200,200,100,30),"Close"))
					{
						tShowGameSetting=false;
					}
				},"hehe");
			}
			if(GUI.Button(new Rect(200,100,80,30),"Logout"))
			{
				Player.Singleton.Logout();
			}
			tSearchUsers=GUI.TextField(new Rect(350,250,200,30),tSearchUsers);
			if(GUI.Button(new Rect(600,250,200,30),"Search"))
			{
				Player.Singleton.SearchUser(tSearchUsers);
			}
			if(GlobalManager.Singleton.SearchUsers!=null)
			{
				foreach(UserData user in GlobalManager.Singleton.SearchUsers)
				{
					if(GUI.Button(new Rect(x+=100,300,100,30),user.NickName))
					{
						User.Singleton.RequestFriend(user.UserId);
					}
				}
			}
			
			if(Player.Singleton.Friends!=null)
			{
				x=100;
				foreach(UserData user in Player.Singleton.Friends)
				{
					if(GUI.Button(new Rect(x+=120,350,100,30),user.UserId))
					{
						User.Singleton.InviteFriendToRoom(user.UserId);
					}
					if(GUI.Button(new Rect(x,400,100,30),"delete"))
					{
						User.Singleton.DeleteFriend(user.UserId);
					}
				}
			}
			
			x=400;
			for(int i=0;i<messages.Count;i++)
			{
				if(GUI.Button(new Rect(200,x+=50,100,30),messages[i].Title))
				{
					messages[i].ProcessMessage();
					messages.RemoveAt(i);
				}
			}
			
			tPassword=GUI.TextField(new Rect(50,200,100,30),tPassword);
			tLivingRoomType=GUI.TextField(new Rect(50,250,100,30),tLivingRoomType);
			if(GUI.Button(new Rect(180,200,100,30),"Save"))
			{
				User.Singleton.Save(User.Singleton.UserData.Avator,tPassword.getMD5(),(RoomType)Convert.ToInt32(tLivingRoomType));
			}

			tBuyRoomType=GUI.TextField(new Rect(50,300,100,30),tBuyRoomType);
			if(GUI.Button(new Rect(180,300,100,30),"Buy"))
			{
				//Shop.Singleton.BuyRoom((RoomType)Convert.ToInt32(tBuyRoomType));
				//Shop.Singleton.BuyChip(Convert.ToInt32(tBuyRoomType));
			}
			
			if (GUI.Button(new Rect(0,0,100,100),"quick start"))
            {
                User.Singleton.QuickStart();
            }
			
			
//			if (GUI.Button(new Rect(0,100,100,100),"find password"))
//            {
//                PhotonClient.Singleton.FindPassword("109924764@qq.com");
//            }
			
			if(GUI.Button(new Rect(500,450,100,30),"Setting"))
			{
				User.Singleton.GameStatus=GameStatus.InSetting;
			}
			
			tBroadcastMessage=GUI.TextField(new Rect(150,500,200,30),tBroadcastMessage);
			if(GUI.Button(new Rect(360,500,100,30),"BroadcastMessage"))
			{
				User.Singleton.BroadcastMessage(tBroadcastMessage);
			}
			
			tNickname=GUI.TextField(new Rect(350,50,200,30),tNickname);
			if(GUI.Button(new Rect(560,50,200,30),"Search"))
			{
				friends=User.Singleton.SearchFriends(tNickname);
			}
			int x2=350;
			foreach(UserData friend in friends)
			{
				GUI.Label(new Rect(x2+=150,100,100,30),friend.NickName);
			}
			
			
			int x3 = 150;
			money =GUI.TextField(new Rect(x3, 550, 80, 30), money);
			if (money != null && !string.Empty.Equals(money))
				User.Singleton.UserData.Chips = Convert.ToInt64(money);
			gameNumber = GUI.TextField(new Rect(x3+100, 550, 80, 30), gameNumber);
			if (gameNumber != null && !string.Empty.Equals(gameNumber))
				User.Singleton.UserData.HandsPlayed = Convert.ToInt64(gameNumber);
			gameWinNumber = GUI.TextField(new Rect(x3 + 200, 550, 80, 30), gameWinNumber);
			if (gameWinNumber != null && !string.Empty.Equals(gameWinNumber))
				User.Singleton.UserData.HandsWon = Convert.ToInt64(gameWinNumber);
			matchNumber = GUI.TextField(new Rect(x3 + 300, 550, 80, 30), matchNumber);
			if (matchNumber != null && !string.Empty.Equals(matchNumber))
				User.Singleton.UserData.CareerWins = Convert.ToInt32(matchNumber);
			level = GUI.TextField(new Rect(x3 + 400, 550, 80, 30), level);
			if (level != null && !string.Empty.Equals(level))
				User.Singleton.UserData.Level = Convert.ToInt32(level);
			
			
			if (GUI.Button(new Rect(x3 + 500,550,100,30),"HONOR"))
			{
				string result = UtilityHelper.GetHonorName(User.Singleton.UserData);
				Debug.Log(result);
				GUI.Label(new Rect(x3 + 600, 550, 100, 30), result);
			}
			
			tReceiver=GUI.TextField(new Rect(500,500,100,30),tReceiver);
			tChip=GUI.TextField(new Rect(610,500,100,30),tChip);
			if(GUI.Button(new Rect(720,500,100,30),"Send"))
			{
				User.Singleton.SendChip(tReceiver,Convert.ToInt64(tChip));
			}
			
			
			tFeedback=GUI.TextField(new Rect(830,500,100,30),tFeedback);
			if(GUI.Button(new Rect(1000,500,100,30),"Feedback"))
			{
				HelpManager.Feedback(tFeedback);
			}
			
			if(GUI.Button(new Rect(1110,500,100,30),"Get Version"))
			{
				SoundHelper.PlaySound("Music/Other/EveryoneFold",audioSource);
				GlobalManager.Singleton.GetClientVersion();
			}
			break;
		case GameStatus.InGame:
		case GameStatus.Sit:
			if(GUI.Button(new Rect(100,100,100,30),"Fold"))
			{
				User.Singleton.Fold();
			}
			if(GUI.Button(new Rect(210,100,100,30),"Call"))
			{
				User.Singleton.Call();
			}
			tRaiseMoney=GUI.TextField(new Rect(320,100,100,30),tRaiseMoney);
			if(GUI.Button(new Rect(430,100,100,30),"Raise"))
			{
				User.Singleton.Raise(Convert.ToInt32(tRaiseMoney));
			}
			if(GUI.Button(new Rect(600,100,100,30),"Stand Up"))
			{
				User.Singleton.StandUp();
			}
			tNoSeat=GUI.TextField(new Rect(710,100,100,30),tNoSeat);
			if(GUI.Button(new Rect(820,100,100,30),"Sit"))
			{
				User.Singleton.Sit(Convert.ToByte(tNoSeat),1000);
			}
			tFriendId=GUI.TextField(new Rect(930,100,100,30),tFriendId);
			if(GUI.Button(new Rect(1040,100,100,30),"Invite"))
			{
				User.Singleton.InviteFriendToGame(tFriendId);
			}
			if(GUI.Button(new Rect(1150,100,100,30),"Leave Game"))
			{
				User.Singleton.LeaveGame();
			}
			
			tGiftReceiverNoSeat=GUI.TextField(new Rect(600,150,100,30),tGiftReceiverNoSeat);
			tGiftType=GUI.TextField(new Rect(820,150,100,30),tGiftType);
			if(GUI.Button(new Rect(930,150,100,30),"Send"))
			{
				if(string.IsNullOrEmpty(tGiftReceiverNoSeat))
				{
					User.Singleton.SendGift(Convert.ToInt32(tGiftType));
				}
				else
				{
					User.Singleton.SendGift(Convert.ToInt32(tGiftReceiverNoSeat),Convert.ToInt32(tGiftType));
				}
			}
			
			if(Room.Singleton.PokerGame.TableInfo!=null)
			{
				GUI.Label(new Rect(50,200,100,30),"TotalPot:"+Room.Singleton.PokerGame.TableInfo.TotalPotAmnt);
				int y1=50;
				foreach(MoneyPot pot in Room.Singleton.PokerGame.TableInfo.Pots)
				{
					GUI.Label(new Rect(50,y1+=50,100,30),"pot:"+pot.Amount);
				}
				int x1=150;
				foreach(PlayerInfo playerInfo in Room.Singleton.PokerGame.TableInfo.PlayersAndBystander)
				{
					GUI.BeginGroup(new Rect(x1+=160,170,150,300));
					GUI.Label(new Rect(0,0,150,30),playerInfo.Avator.ToString());
					GUI.Label(new Rect(0,30,150,30),playerInfo.Name);
					GUI.Label(new Rect(0,60,150,30),"Money:"+playerInfo.MoneySafeAmnt);
					GUI.Label(new Rect(0,90,150,30),"Bet:"+playerInfo.MoneyBetAmnt);
					GUI.Label(new Rect(0,120,150,30),"Action:"+(Room.Singleton.PokerGame.PlayerActionNames.ContainsKey(playerInfo.NoSeat)?Room.Singleton.PokerGame.PlayerActionNames[playerInfo.NoSeat]:""));
					
					//GameCard[] gameCards=playerInfo.NoSeat==User.Singleton.UserData.NoSeat||Room.Singleton.PokerGame.IsWin?playerInfo.Cards:playerInfo.RelativeCards;
					//GameCard[] gameCards=playerInfo.Cards;
					//GUI.Label(new Rect(0,150,150,30),"Cards:"+gameCards[0].ToString()+","+gameCards[1].ToString());
					GUI.Label(new Rect(0,180,150,30),"Current:"+(Room.Singleton.PokerGame.TableInfo.NoSeatCurrPlayer==playerInfo.NoSeat).ToString());
					GUI.Label(new Rect(0,210,150,30),"IsPlaying:"+playerInfo.IsPlaying);
					GUI.Label(new Rect(0,240,150,30),"Gift:"+playerInfo.GiftId);
					GUI.Label(new Rect(0,270,150,30),"NoSeat:"+playerInfo.NoSeat);
					GUI.EndGroup();
				}
				x1=150;
				for(int i=0;i<5;i++)
				{
					if(Room.Singleton.PokerGame.TableInfo.Cards[i].Id!=GameCard.NO_CARD.Id)
					{
						GUI.Label(new Rect(x1+=110,450,100,30),Room.Singleton.PokerGame.TableInfo.Cards[i].ToString());
					}
					else
					{
						GUI.Label(new Rect(x1+=110,450,100,30),GameCard.HIDDEN.ToString());
					}
				}
			}
			
			tBroadcastMessageInTable=GUI.TextField(new Rect(150,500,200,30),tBroadcastMessageInTable);
			if(GUI.Button(new Rect(360,500,100,30),"BroadcastMessage"))
			{
				User.Singleton.BroadcastMessageInTable(tBroadcastMessageInTable);
			}
			
			break;
		case GameStatus.InSetting:
			SettingManager.Singleton.Animation=GUI.Toggle(new Rect(100,100,100,30),SettingManager.Singleton.Animation,"Animation");
			SettingManager.Singleton.ChatBubble=GUI.Toggle(new Rect(100,150,100,30),SettingManager.Singleton.ChatBubble,"ChatBubble");
			SettingManager.Singleton.Music=GUI.Toggle(new Rect(100,200,100,30),SettingManager.Singleton.Music,"Music");
			SettingManager.Singleton.PokerPointer=GUI.Toggle(new Rect(100,250,100,30),SettingManager.Singleton.PokerPointer,"PokerPointer");
			SettingManager.Singleton.Shake=GUI.Toggle(new Rect(100,300,100,30),SettingManager.Singleton.Shake,"Shake");
			SettingManager.Singleton.Sound=GUI.Toggle(new Rect(100,350,100,30),SettingManager.Singleton.Sound,"Sound");
			if(GUI.Button (new Rect(100,400,100,30),"Save"))
			{
				SettingManager.Singleton.SaveFile();
			}
			if(GUI.Button(new Rect(210,400,100,30),"Back"))
			{
				User.Singleton.GameStatus=GameStatus.InRoom;
			}
			
			break;
		case GameStatus.Error:
			GlobalManager.Log(PhotonClient.Singleton.ErrorMessage);
			break;
		}
	}
}
