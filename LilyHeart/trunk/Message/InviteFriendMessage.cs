using System;
using UnityEngine;
using DataPersist;
using System.Collections;
//using AssemblyCSharp.Helper;

namespace LilyHeart
{
	public class InviteFriendMessage:PlayerMessage
	{
		public const string DESTINATION_ROOM="Room";
		public const string DESTINATION_GAME="Game";
		private string destinationType;
		private string destinationId;
		
		public string DestinationType {get {return destinationType;}}
		public string DestinationId {get {return destinationId;}}
		
		public InviteFriendMessage (UserData userData,string content):base(userData)
		{
			string[] values=content.Split('|');
			this.destinationType=values[0];
			this.destinationId=values[1];
			Title="Friend Invite";
//			if (this.destinationType==DESTINATION_GAME)
//				Title = LocalizeHelper.Translate("INVITE_FRIEND_2_PROMPT_MESSAGE");
//			else
//			{
//				if (GlobalManager.Singleton.version == KindOfVersion.Ultimate)
//					Title = LocalizeHelper.Translate("INVITE_FRIEND_1_PROMPT_MESSAGE");
//				else
//					Title = LocalizeHelper.Translate("SIMPLE_VERSION_LIMITED_1");
//			}
			if (this.destinationType==DESTINATION_GAME)
				Title = "INVITE_FRIEND_2_PROMPT_MESSAGE";
			else
			{
				if (GlobalManager.Singleton.version == KindOfVersion.Ultimate)
					Title = "INVITE_FRIEND_1_PROMPT_MESSAGE";
				else
					Title = "SIMPLE_VERSION_LIMITED_1";
			}
		}
		
		IEnumerable loadEmptyScene()
		{
			AsyncOperation async = Application.LoadLevelAsync("LoadingTable");
			yield return async;
		}
		
		void GotoGame(bool iswork,TypeState gameState)
		{
			PhotonClient.JoinGameFinished-=GotoGame;
		}
		
		public override void ProcessMessage()
		{
			if(this.destinationType==DESTINATION_GAME)
			{
				PhotonClient.JoinGameFinished+=GotoGame;
				PhotonClient.Singleton.CurrentAction=PhotonClient.Singleton.TryJoinGame;
				PhotonClient.Singleton.InviterId=sender.UserId;
			}			
			//if(this.destinationType!=DESTINATION_GAME)
				//PhotonClient.GotoBackgroundSceneEvent += GotoFriend();
			User.Singleton.JoinRoom(destinationId);
		}
		
		public override bool Equals (object obj)
		{
			if(obj is InviteFriendMessage)
			{
				InviteFriendMessage message=obj as InviteFriendMessage;
				return this.destinationId==message.DestinationId&&this.DestinationType==message.DestinationType;
			}
			
			return base.Equals (obj);
		}
	}
}

